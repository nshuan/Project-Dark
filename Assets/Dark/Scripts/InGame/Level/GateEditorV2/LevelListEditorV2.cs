using System;
using System.IO;
using Dark.Scripts.Common.UIWarning;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.GateEditorV2
{
    public class LevelListEditorV2 : MonoBehaviour
    {
        [SerializeField] private RectTransform contentHolder;
        [SerializeField] private LevelListItemEditorV2 itemPrefab;
        [SerializeField] private Button btnAddLevel;
        [SerializeField] private Button btnDeleteLevel;
        [SerializeField] private UIPopupWarning popupConfirm;
        
        private LevelManifest levelManifest;
        
        private void Awake()
        {
            levelManifest = LevelManifest.Instance;
            btnAddLevel.onClick.RemoveAllListeners();
            btnAddLevel.onClick.AddListener(AddLevel);
            btnDeleteLevel.onClick.RemoveAllListeners();
            btnDeleteLevel.onClick.AddListener(DeleteSelectingLevel);
        }

        private void Start()
        {
            RefreshLevels();
        }

        private void RefreshLevels()
        {
            if (!levelManifest) return;
            var allLevels = levelManifest.GetAllLevels();
            if (allLevels == null) return;

            foreach (Transform child in contentHolder)
            {
                if (child.name != btnAddLevel.transform.name)
                    Destroy(child.gameObject);
            }

            foreach (var level in allLevels)
            {
                var newLevelItem = Instantiate(itemPrefab, contentHolder);
                newLevelItem.name = level.name;
                newLevelItem.UpdateUI(level);
                newLevelItem.btnClick.onClick.AddListener(() =>
                {
                    foreach (Transform child in contentHolder)
                    {
                        if (child.name != btnAddLevel.transform.name)
                        {
                            if (child.TryGetComponent<LevelListItemEditorV2>(out var levelItem))
                            {
                                levelItem.Selecting = false;
                                levelItem.btnClick.targetGraphic.color = Color.white;
                            }
                        }
                    }
                    
                    newLevelItem.Selecting = true;
                    newLevelItem.btnClick.targetGraphic.color = Color.cyan;
                });
            }
            
            btnAddLevel.transform.SetAsLastSibling();
        }

        private void AddLevel()
        {
            var newLevel = levelManifest.GetAllLevels()[^1].level + 1;
            var newLevelAsset = ScriptableObject.CreateInstance<LevelConfig>();
            newLevelAsset.level = newLevel;
#if UNITY_EDITOR
            AssetDatabase.CreateAsset(newLevelAsset, Path.Combine(LevelManifest.LevelPath, $"Level {newLevel}.asset"));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            levelManifest.Validate();
            EditorUtility.SetDirty(levelManifest);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
            RefreshLevels();
            foreach (Transform child in contentHolder)
            {
                if (child.TryGetComponent<LevelListItemEditorV2>(out var levelItem))
                {
                    if (levelItem.txtLevel.text == $"Level {newLevelAsset.level}")
                    {
                        levelItem.btnClick.onClick?.Invoke();
                        break;
                    }
                }
            }
        }

        public void DeleteSelectingLevel()
        {
            popupConfirm.Setup(
                "Remember to save your changes!",
                "Confirm delete level?",
                () =>
                {
                    LevelGateEditorV2.Instance.ClearAllWaves();
            
                    foreach (Transform child in contentHolder)
                    {
                        if (child.name != btnAddLevel.transform.name)
                        {
                            if (child.TryGetComponent<LevelListItemEditorV2>(out var levelItem))
                            {
                                if (levelItem.Selecting)
                                {
                                    Destroy(levelItem.gameObject);
#if UNITY_EDITOR
                                    DeleteLevel(levelItem.Config);
#endif
                                }
                            }
                        }
                    }
                    
                    popupConfirm.gameObject.SetActive(false);
                }, () =>
                {
                    popupConfirm.gameObject.SetActive(false);
                });
            popupConfirm.gameObject.SetActive(true);
        }
        
#if UNITY_EDITOR
        public void DeleteLevel(LevelConfig level)
        {
            var wavePath = Path.Combine(LevelManifest.WavePath, $"Level {level.level}");
            var levelPath = Path.Combine(LevelManifest.LevelPath, $"Level {level.level}.asset");
            AssetDatabase.DeleteAsset(wavePath);
            AssetDatabase.DeleteAsset(levelPath);
            AssetDatabase.Refresh();
            
            LevelManifest.Instance.Validate();
        }
#endif
    }
}