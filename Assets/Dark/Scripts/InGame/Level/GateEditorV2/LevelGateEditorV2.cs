using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;
using Dark.Scripts.Common.UIWarning;
using Dark.Scripts.SceneNavigation;
using Dark.Scripts.Utils;
using Data;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.GateEditorV2
{
    public class LevelGateEditorV2 : MonoSingleton<LevelGateEditorV2>
    {
#if UNITY_EDITOR
        public Camera cam;
        public RectTransform parentWaveButtons;
        public Button prefabWaveButton;
        public RectTransform parentWaves;
        public LevelWavePrefabEditorV2 prefabWave;
        public Button btnSave;
        public Button btnAddWave;
        public Button btnDeleteWave;
        public UIPopupWarning popupConfirm;
        public Button btnPlayLevel;
        public TMP_Dropdown drdMapType;
        public TMP_Dropdown drdBackgroundType;
        public Button btnEditTower;
        public LevelTowerPositionEditorV2 towerPositionEditor;

        [Space] [Header("Display")] 
        public TextMeshProUGUI txtLevel;
        
        public CharacterClass.CharacterClass ClassType { get; set; }
        
        private LevelManifest levelManifest;
        private LevelConfig currentLevel;
        private LevelMapType currentMapType;
        private int currentBgIndex;
        
        protected override void Awake()
        {
            base.Awake();
            
            levelManifest = LevelManifest.Instance;
            btnSave.onClick.RemoveAllListeners();
            btnSave.onClick.AddListener(SaveLevel);
            btnAddWave.onClick.RemoveAllListeners();
            btnAddWave.onClick.AddListener(AddNewWave);
            btnDeleteWave.onClick.RemoveAllListeners();
            btnDeleteWave.onClick.AddListener(DeleteSelectingWave);
            btnPlayLevel.onClick.RemoveAllListeners();
            btnPlayLevel.onClick.AddListener(PlaySelectingLevel);
            drdMapType.onValueChanged.RemoveAllListeners();
            var options = new List<LevelMapType>();
            foreach (LevelMapType mapType in Enum.GetValues(typeof(LevelMapType)))
            {
                options.Add(mapType);
            }
            drdMapType.options = options.Select(mt => 
                new TMP_Dropdown.OptionData(mt.ToString())).ToList();
            drdMapType.onValueChanged.AddListener(ChangeMapType);
            drdBackgroundType.onValueChanged.RemoveAllListeners();
            var optionsBg = new List<string>()
            {
                "Background 1", "Background 2", "Background 3"
            };
            drdBackgroundType.options = optionsBg.Select(mt => 
                new TMP_Dropdown.OptionData(mt)).ToList();
            drdBackgroundType.onValueChanged.AddListener(ChangeBgType);
            btnEditTower.onClick.RemoveAllListeners();
            btnEditTower.onClick.AddListener(() =>
            {
                if (!currentLevel) return;
                towerPositionEditor.Setup(LevelTowerEditorV2.Instance.GetTowers(currentMapType));
                towerPositionEditor.gameObject.SetActive(true);
            });
        }
        
        public void LoadLevel(int levelId)
        {
            if (!levelManifest) return;
            currentLevel = levelManifest.GetTrueLevel(ClassType, levelId);
            if (!currentLevel) return;

            currentMapType = currentLevel.mapType;
            drdMapType.value = (int)currentLevel.mapType;
            LevelTowerEditorV2.Instance.SetPosition(currentMapType, currentLevel.towerPositions);
            currentBgIndex = currentLevel.backgroundIndex;
            drdBackgroundType.value = currentLevel.backgroundIndex;
            // Destroy all old wave buttons
            ClearAllWaves();
            
            if (currentLevel.waveInfo == null) return;
            foreach (var waveInfo in currentLevel.waveInfo)
            {
                AddNewWave(waveInfo);
            }
            SelectWave(0);
            
            txtLevel?.SetText($"Level: {levelId}");
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
                    LevelManager.Instance.LoadLevel(currentLevel.level);
                });
            });
        }

        public void ChangeMapType(int index)
        {
            currentMapType = (LevelMapType)index;
            LevelBackgroundVariantEditorV2.Instance.SetMapType((LevelMapType)index);
            LevelTowerEditorV2.Instance.SetPosition(currentMapType, null);
        }
        
        public void ChangeBgType(int index)
        {
            currentBgIndex = index;
            LevelBackgroundVariantEditorV2.Instance.SetBackgroundType(currentBgIndex);
        }
        
        #region Waves

        public void ClearAllWaves()
        {
            foreach (Transform child in parentWaveButtons)
            {
                if (child.name != btnAddWave.transform.name)
                    Destroy(child.gameObject);
            }

            foreach (Transform child in parentWaves)
            {
                Destroy(child.gameObject);
            }
        }

        public void AddNewWave()
        {
            if (!currentLevel) return;
            var waveList = currentLevel.waveInfo != null ? currentLevel.waveInfo.ToList() : new List<WaveInfo>();
            waveList.Sort((wave1, wave2) => wave1.waveIndex.CompareTo(wave2.waveIndex));

            var newWave = 0;
            if (waveList.Count > 0) newWave = waveList[^1].waveIndex + 1;
            var newWaveInfo = new WaveInfo()
            {
                waveIndex = newWave
            };
            WaveConfig newWaveAsset;
#if UNITY_EDITOR
            if (!Directory.Exists(Path.Combine(WaveFolderPath, $"Level {currentLevel.level}")))
            {
                Directory.CreateDirectory(Path.Combine(WaveFolderPath, $"Level {currentLevel.level}"));
            }
            var filePath = Path.Combine(WaveFolderPath,
                $"Level {currentLevel.level}/Level_{currentLevel.level}_Wave_{newWave + 1}.asset");
            if (File.Exists(filePath))
            {
                newWaveAsset = AssetDatabase.LoadAssetAtPath<WaveConfig>(filePath);
            }
            else
            {
                newWaveAsset = ScriptableObject.CreateInstance<WaveConfig>();
                AssetDatabase.CreateAsset(newWaveAsset, filePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            
            newWaveInfo.waveConfig = newWaveAsset;
            waveList.Add(newWaveInfo);
            currentLevel.waveInfo = waveList.ToArray();
            EditorUtility.SetDirty(currentLevel);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
            AddNewWave(newWaveInfo);
        }

        public string WaveFolderPath
        {
            get
            {
                if (ClassType == CharacterClass.CharacterClass.Knight) return LevelManifest.KnightWavePath;
                return LevelManifest.ArcherWavePath;
            }
        }
        
        public string LevelFolderPath
        {
            get
            {
                if (ClassType == CharacterClass.CharacterClass.Knight) return LevelManifest.KnightLevelPath;
                return LevelManifest.ArcherLevelPath;
            }
        }
        
        public void AddNewWave(WaveInfo waveInfo)
        {
            var wave = Instantiate(prefabWave, parentWaves);
            wave.UpdateUI(waveInfo.waveConfig);
            var button = Instantiate(prefabWaveButton, parentWaveButtons);
            button.onClick.AddListener(() => SelectWave(button.transform.GetSiblingIndex()));
            var txtWave = button.GetComponentInChildren<TextMeshProUGUI>();
            txtWave?.SetText($"Wave {waveInfo.waveIndex}");
            btnAddWave.transform.SetAsLastSibling();
            SelectWave(button.transform.GetSiblingIndex());
        }

        public void SelectWave(int waveIndex)
        {
            if (waveIndex < 0 || waveIndex >= parentWaves.childCount) return;
            
            // Hide all buttons and waves
            foreach (Transform child in parentWaveButtons)
            {
                if (child.name == btnAddWave.transform.name) continue;
                var button = child.GetComponent<Button>();
                button.targetGraphic.color = Color.white;
            }
            foreach (Transform child in parentWaves)
            {
                child.gameObject.SetActive(false);
                if (child.TryGetComponent<LevelWavePrefabEditorV2>(out var wave))
                {
                    wave.Selecting = false;
                }
            }

            var selectButton = parentWaveButtons.GetChild(waveIndex).GetComponent<Button>();
            var selectWave = parentWaves.GetChild(waveIndex);
            selectButton.targetGraphic.color = Color.cyan;
            selectWave.gameObject.SetActive(true);
            if (selectWave.TryGetComponent<LevelWavePrefabEditorV2>(out var selectWaveScript))
            {
                selectWaveScript.Selecting = true;
            }
        }

        public void DeleteSelectingWave()
        {
            popupConfirm.Setup(
                "Remember to save your changes!",
                "Confirm delete wave?",
                () =>
                {
                    foreach (Transform child in parentWaves)
                    {
                        if (child.TryGetComponent<LevelWavePrefabEditorV2>(out var wave))
                        {
                            if (wave.Selecting)
                            {
                                DeleteWave(child.GetSiblingIndex());
                            }
                        }
                    }
                    
                    LoadLevel(currentLevel.level);
                    popupConfirm.gameObject.SetActive(false);
                }, () =>
                {
                    popupConfirm.gameObject.SetActive(false);
                });
            popupConfirm.gameObject.SetActive(true);
        }
        
        public void DeleteWave(int waveIndex)
        {
            if (!currentLevel) return;
            if (waveIndex < 0 || waveIndex >= parentWaves.childCount) return;
            
            var newWaveInfos = new List<WaveInfo>();
            WaveConfig configToDelete = null;
            foreach (var waveInfo in currentLevel.waveInfo)
            {
                if (waveInfo.waveIndex != waveIndex)
                {
                    newWaveInfos.Add(waveInfo);
                }
                else
                {
                    configToDelete = waveInfo.waveConfig;
                }
            }
            currentLevel.waveInfo = newWaveInfos.ToArray();
            
#if UNITY_EDITOR
            if (configToDelete)
            {
                var wavePath = AssetDatabase.GetAssetPath(configToDelete);
                AssetDatabase.DeleteAsset(wavePath);
            }

            for (var i = 0; i < currentLevel.waveInfo.Length; i++)
            {
                currentLevel.waveInfo[i].waveIndex = i;
                var configPath = AssetDatabase.GetAssetPath(currentLevel.waveInfo[i].waveConfig);
                AssetDatabase.RenameAsset(configPath, $"Level_{currentLevel.level}_Wave_{i + 1}");
            }
            EditorUtility.SetDirty(currentLevel);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        #endregion

        #region Save

        public void SaveLevel()
        {
            if (!currentLevel) return;
            foreach (Transform child in parentWaves)
            {
                if (child.TryGetComponent<LevelWavePrefabEditorV2>(out var wave))
                {
                    wave.SaveWave();
                }
            }

            currentLevel.mapType = currentMapType;
            currentLevel.towerPositions = LevelTowerEditorV2.Instance.GetPositions(currentMapType);
            currentLevel.backgroundIndex = currentBgIndex;
                        
#if UNITY_EDITOR
            EditorUtility.SetDirty(currentLevel);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        #endregion
#endif
    }
}