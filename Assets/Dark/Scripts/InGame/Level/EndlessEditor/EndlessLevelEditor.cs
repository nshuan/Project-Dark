using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;
using Dark.Scripts.Common.UIWarning;
using Dark.Scripts.SceneNavigation;
using Dark.Scripts.Utils;
using InGame.EndlessLevel;
using InGame.GateEditorV2;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.EndlessEditor
{
    public class EndlessLevelEditor : MonoSingleton<EndlessLevelEditor>
    {
#if UNITY_EDITOR
        public Camera cam;
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
        private EndlessWaveEditor currentWaveEditor;
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
            var options = new List<TMP_Dropdown.OptionData>();
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
            options = new List<TMP_Dropdown.OptionData>();
            foreach (var pool in allWavePools)
            {
                options.Add(new TMP_Dropdown.OptionData() { text = pool.name });
            }
            drdWavePool.options = options;
            drdWavePool.onValueChanged.AddListener(OnSelectWavePool);
            
            drdWaves.onValueChanged.RemoveAllListeners();
            drdWaves.ClearOptions();
            options = new List<TMP_Dropdown.OptionData>();
            foreach (var wave in allWaves)
            {
                options.Add(new TMP_Dropdown.OptionData() { text = wave.name });
            }
            drdWaves.options = options;
            drdWaves.onValueChanged.AddListener(OnSelectWave);
        }

        private void OnSelectLevel(int index)
        {
            currentLevel = allLevels[index];
            LoadLevel(currentLevel);
        }

        private void OnSelectWaveInfo(int index)
        {
            if (!currentLevel) return;
            if (currentLevel.waveInfo == null) return;
            if (currentLevel.waveInfo.Length == 0) return;
            if (index < 0 || index >= currentLevel.waveInfo.Length)
            {
                OnSelectWaveInfo(0);
                return;
            }
            if (allWavePools == null || allWavePools.Count == 0) return;

            var poolIndex = 0;
            for (var i = 0; i < allWavePools.Count; i++)
            {
                if (allWavePools[i] == currentLevel.waveInfo[index].wavePool)
                {
                    poolIndex = i;
                    break;
                }
            }
            
            drdWavePool.value = poolIndex;
        }

        private void OnSelectWavePool(int index)
        {
            if (allWavePools == null || allWavePools.Count == 0) return;
            if (index < 0 || index >= allWavePools.Count)
            {
                OnSelectWaveInfo(0);
                return;
            }

            var pool = allWavePools[index];
            allWaveInPool = pool.allWaves.ToList();
            if (allWaveInPool == null) return;
            
            var options = new List<TMP_Dropdown.OptionData>();
            for (var i = 0; i < allWaveInPool.Count; i++)
            {
                options.Add(new TMP_Dropdown.OptionData() { text = allWaveInPool[i].name });
            }

            drdWaves.options = options;
            
            // Destroy current wave editor
            if (currentWaveEditor)
            {
                Destroy(currentWaveEditor.gameObject);
                currentWaveEditor = null;
            }
        }

        private void OnSelectWave(int index)
        {
            if (allWaveInPool == null) return;
            if (allWaveInPool.Count == 0) return;
            if (index < 0 || index >= allWaveInPool.Count)
            {
                OnSelectWave(0);
                return;
            }

            if (currentWaveEditor == null)
            {
                currentWaveEditor = Instantiate(prefabWave, parentWaves);
            }
            
            currentWaveEditor.UpdateUI(allWaveInPool[index]);
            currentWaveEditor.gameObject.SetActive(true);
        }
        
        public void LoadLevel(LevelEndlessConfig level)
        {
            currentLevel = level;
            if (!currentLevel) return;
            
            // Add options for all waves in current level
            var options = new List<TMP_Dropdown.OptionData>();
            if (currentLevel.waveInfo != null)
            {
                for (var i = 0; i < currentLevel.waveInfo.Length; i++)
                {
                    options.Add(new TMP_Dropdown.OptionData()
                    {
                        text = $"wave {i}"
                    });
                }
            }
            drdLevelWaveInfo.options = options;
            drdLevelWaveInfo.value = 0;
            
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
            if (!currentLevel) return;
        
                        
#if UNITY_EDITOR
            EditorUtility.SetDirty(currentLevel);
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