using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.GateEditorV2
{
    public class LevelGateEditorV2 : MonoSingleton<LevelGateEditorV2>
    {
        public Camera cam;
        public RectTransform parentWaveButtons;
        public Button prefabWaveButton;
        public RectTransform parentWaves;
        public LevelWavePrefabEditorV2 prefabWave;
        public Button btnSave;
        public Button btnAddWave;

        [Space] [Header("Display")] 
        public TextMeshProUGUI txtLevel;
        
        private LevelManifest levelManifest;
        private LevelConfig currentLevel;
        
        protected override void Awake()
        {
            base.Awake();
            
            levelManifest = LevelManifest.Instance;
            btnSave.onClick.RemoveAllListeners();
            btnSave.onClick.AddListener(SaveLevel);
            btnAddWave.onClick.RemoveAllListeners();
            btnAddWave.onClick.AddListener(AddNewWave);
        }
        
        public void LoadLevel(int levelId)
        {
            if (!levelManifest) return;
            currentLevel = levelManifest.GetTrueLevel(levelId);
            if (!currentLevel) return;
            
            // Destroy all old wave buttons
            ClearAllWaves();
            
            if (currentLevel.waveInfo == null) return;
            foreach (var waveInfo in currentLevel.waveInfo)
            {
                AddNewWave(waveInfo);
            }
            
            txtLevel?.SetText($"Level: {levelId}");
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
            if (!Directory.Exists(Path.Combine(LevelManifest.WavePath, $"Level {currentLevel.level}")))
            {
                Directory.CreateDirectory(Path.Combine(LevelManifest.WavePath, $"Level {currentLevel.level}"));
            }
            var filePath = Path.Combine(LevelManifest.WavePath,
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
        
        public void AddNewWave(WaveInfo waveInfo)
        {
            var wave = Instantiate(prefabWave, parentWaves);
            wave.UpdateUI(waveInfo.waveConfig);
            var button = Instantiate(prefabWaveButton, parentWaveButtons);
            button.onClick.AddListener(() => SelectWave(button.transform.GetSiblingIndex()));
            var txtWave = button.GetComponentInChildren<TextMeshProUGUI>();
            txtWave?.SetText($"Wave {waveInfo.waveIndex}");
            SelectWave(button.transform.GetSiblingIndex());
            btnAddWave.transform.SetAsLastSibling();
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
            }

            var selectButton = parentWaveButtons.GetChild(waveIndex).GetComponent<Button>();
            var selectWave = parentWaves.GetChild(waveIndex);
            selectButton.targetGraphic.color = Color.cyan;
            selectWave.gameObject.SetActive(true);
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
            
                        
#if UNITY_EDITOR
            EditorUtility.SetDirty(currentLevel);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        #endregion
    }
}