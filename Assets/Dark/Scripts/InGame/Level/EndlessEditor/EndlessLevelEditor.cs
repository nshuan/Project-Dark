using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;
using Dark.Scripts.Common.UIWarning;
using Dark.Scripts.SceneNavigation;
using Dark.Scripts.Utils;
using InGame.EndlessLevel;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.EndlessEditor
{
    public class EndlessLevelEditor : MonoSingleton<EndlessLevelEditor>
    {
#if UNITY_EDITOR
        public RectTransform parentWaves;
        public EndlessWaveEditor prefabWave;
        public Button btnSave;
        public UIPopupWarning popupConfirm;
        public Button btnPlayLevel;
        public TMP_Dropdown drdEndlessLevel;
        public TMP_Dropdown drdLevelWaveInfo;
        public TMP_Dropdown drdWavePool;
        public TMP_Dropdown drdWaves;

        [Space] [Header("Display")] 
        public TextMeshProUGUI txtLevel;

        [Space] [Header("Path")] 
        [FolderOnly] public DefaultAsset levelFolderPath;
        [FolderOnly] public DefaultAsset poolFolderPath;
        [FolderOnly] public DefaultAsset waveFolderPath;
        
        public CharacterClass.CharacterClass ClassType { get; set; }

        public List<LevelEndlessConfig> allLevels;
        public List<PoolWaveEndless> allWavePools;
        public List<WaveEndlessConfig> allWaves;
        
        private LevelEndlessConfig currentLevel;
        private WaveEndlessInfo currentWaveInfo;
        private PoolWaveEndless currentPool;
        private EndlessWaveEditor currentWaveEditor;
        private List<WaveEndlessInfo> allWaveInfoInLevel;
        private List<WaveEndlessConfig> allWaveInPool;
        
        protected override void Awake()
        {
            base.Awake();
            
            GetAllLevels();
            GetAllWavePools();
            GetAllWaves();
            
            btnSave.onClick.RemoveAllListeners();
            btnSave.onClick.AddListener(SaveLevel);
            btnPlayLevel.onClick.RemoveAllListeners();
            btnPlayLevel.onClick.AddListener(PlaySelectingLevel);
            
            drdEndlessLevel.onValueChanged.RemoveAllListeners();
            drdEndlessLevel.ClearOptions();
            var options = new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("None")};
            foreach (var level in allLevels)
            {
                options.Add(new TMP_Dropdown.OptionData() { text = level.name });
            }
            drdEndlessLevel.options = options;
            drdEndlessLevel.onValueChanged.AddListener(OnSelectLevel);
            
            drdLevelWaveInfo.onValueChanged.RemoveAllListeners();
            drdLevelWaveInfo.ClearOptions();
            drdLevelWaveInfo.onValueChanged.AddListener(OnSelectWaveInfo);
            
            drdWavePool.onValueChanged.RemoveAllListeners();
            drdWavePool.ClearOptions();
            options = new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("None")};
            foreach (var pool in allWavePools)
            {
                options.Add(new TMP_Dropdown.OptionData() { text = pool.name });
            }
            drdWavePool.options = options;
            drdWavePool.onValueChanged.AddListener(OnSelectWavePool);
            
            drdWaves.onValueChanged.RemoveAllListeners();
            drdWaves.ClearOptions();
            options = new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("None")};
            foreach (var wave in allWaves)
            {
                options.Add(new TMP_Dropdown.OptionData() { text = wave.name });
            }
            drdWaves.options = options;
            drdWaves.onValueChanged.AddListener(OnSelectWave);
        }

        private void OnSelectLevel(int index)
        {
            if (index == 0) LoadLevel(null);
            else LoadLevel(allLevels[index - 1]);
        }

        private void OnSelectWaveInfo(int index)
        {
            if (index <= 0 || !currentLevel || allWaveInfoInLevel == null || allWaveInfoInLevel.Count == 0 || index > allWaveInfoInLevel.Count)
            {
                currentWaveInfo = null;
                drdWavePool.value = 0;
                OnSelectWavePool(0);
                drdWavePool.RefreshShownValue();
                return;
            }

            currentWaveInfo = allWaveInfoInLevel[index - 1];
            
            if (allWavePools == null || allWavePools.Count == 0) return;

            var poolIndex = 0;
            for (var i = 0; i < allWavePools.Count; i++)
            {
                if (allWavePools[i] == currentWaveInfo.wavePool)
                {
                    poolIndex = i + 1;
                    break;
                }
            }
            
            drdWavePool.value = poolIndex;
            OnSelectWavePool(poolIndex);
            drdWavePool.RefreshShownValue();
        }

        private void OnSelectWavePool(int index)
        {
            if (index <= 0 || allWavePools == null || allWavePools.Count == 0 || index > allWavePools.Count)
            {
                currentPool = null;
                if (currentWaveInfo != null) currentWaveInfo.wavePool = currentPool;
                DeselectAnyWave();
                return;
            }

            currentPool = allWavePools[index - 1];
            if (currentWaveInfo != null) currentWaveInfo.wavePool = currentPool;
            allWaveInPool = currentPool.allWaves.ToList();
            if (allWaveInPool == null)
            {
                DeselectAnyWave();
                return;
            }

            var options = new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("None") };
            for (var i = 0; i < allWaveInPool.Count; i++)
            {
                options.Add(new TMP_Dropdown.OptionData() { text = allWaveInPool[i].name });
            }

            drdWaves.options = options;
            DeselectAnyWave();

            void DeselectAnyWave()
            {
                drdWaves.value = 0;
                OnSelectWave(0);
                drdWaves.RefreshShownValue();
            }
        }

        private void OnSelectWave(int index)
        {
            if (index <= 0 || allWaveInPool == null || allWaveInPool.Count == 0 || index > allWaveInPool.Count)
            {
                // Destroy current wave editor
                if (currentWaveEditor)
                {
                    Destroy(currentWaveEditor.gameObject);
                    currentWaveEditor = null;
                }
                
                return;
            }

            if (currentWaveEditor == null)
            {
                currentWaveEditor = Instantiate(prefabWave, parentWaves);
            }
            
            currentWaveEditor.UpdateUI(allWaveInPool[index - 1]);
            currentWaveEditor.gameObject.SetActive(true);
        }
        
        public void LoadLevel(LevelEndlessConfig level)
        {
            currentLevel = level;
            if (!currentLevel)
            {
                allWaveInfoInLevel = null;
                drdLevelWaveInfo.options = new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("None")};
                drdLevelWaveInfo.value = 0;
                OnSelectWaveInfo(0);
                drdLevelWaveInfo.RefreshShownValue();
                return;
            }
            
            allWaveInfoInLevel = new List<WaveEndlessInfo>();
            if (currentLevel.waveInfo != null)
            {
                foreach (var waveInfo in currentLevel.waveInfo)
                {
                    allWaveInfoInLevel.Add(new WaveEndlessInfo()
                    {
                        scaleHp = waveInfo.scaleHp,
                        scaleDmg = waveInfo.scaleDmg,
                        scaleSpe = waveInfo.scaleSpe,
                        expRatio = waveInfo.expRatio,
                        darkRatio = waveInfo.darkRatio,
                        darkUnitValue = waveInfo.darkUnitValue,
                        sigils = waveInfo.sigils,
                        ashes = waveInfo.ashes,
                        wavePool = waveInfo.wavePool
                    });
                }
            }
            
            // Add options for all waves in current level
            var options = new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("None")};
            if (allWaveInfoInLevel != null)
            {
                for (var i = 0; i < allWaveInfoInLevel.Count; i++)
                {
                    options.Add(new TMP_Dropdown.OptionData()
                    {
                        text = $"wave {i}"
                    });
                }
            }
            drdLevelWaveInfo.options = options;
            drdLevelWaveInfo.value = 0;
            OnSelectWaveInfo(0);
            drdLevelWaveInfo.RefreshShownValue();
            
            txtLevel?.SetText($"Level: {level.name}");
        }

        public void PlaySelectingLevel()
        {
            if (!currentLevel) return;
            
#if UNITY_EDITOR
            LevelManager.isLoadFromInit = true;
#endif
            this.DelayCall(0.5f, () =>
            {
                Loading.Instance.QuickLoadScene(SceneConstants.SceneInGame, () =>
                {
                    LevelManager.Instance.LoadEndlessLevel();
                });
            });
        }

        #region Save

        public void SaveLevel()
        {
            if (currentWaveEditor)
            {
                currentWaveEditor.SaveWave();
            }

            if (currentPool)
            {
                currentPool.allWaves = allWaveInPool.ToArray();
                EditorUtility.SetDirty(currentPool);
            }

            if (currentLevel)
            {
                currentLevel.waveInfo = allWaveInfoInLevel.ToArray();
                EditorUtility.SetDirty(currentLevel);
            }
                        
#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        #endregion

        #region Editor

        public void GetAllLevels()
        {
            var folderPath = UnityEditor.AssetDatabase.GetAssetPath(levelFolderPath);
            allLevels = AssetUtility.LoadAllScriptableObjectsInFolder<LevelEndlessConfig>(folderPath).ToList();
        }

        public void GetAllWavePools()
        {
            var folderPath = UnityEditor.AssetDatabase.GetAssetPath(poolFolderPath);
            allWavePools = AssetUtility.LoadAllScriptableObjectsInFolder<PoolWaveEndless>(folderPath).ToList();
        }

        public void GetAllWaves()
        {
            var folderPath = UnityEditor.AssetDatabase.GetAssetPath(waveFolderPath);
            allWaves = AssetUtility.LoadAllScriptableObjectsInFolder<WaveEndlessConfig>(folderPath).ToList();
        }

        #endregion
#endif
    }
}