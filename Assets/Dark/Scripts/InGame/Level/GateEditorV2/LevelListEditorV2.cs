using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dark.Scripts.Common.UIWarning;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.GateEditorV2
{
    public class LevelListEditorV2 : MonoBehaviour
    {
        [SerializeField] private GameObject groupSelectClass;
        [SerializeField] private TMP_Dropdown drdSelectClass;
        [SerializeField] private Button btnLoadLevels;
        [SerializeField] private RectTransform contentHolder;
        [SerializeField] private LevelListItemEditorV2 itemPrefab;
        [SerializeField] private Button btnAddLevel;
        [SerializeField] private Button btnDeleteLevel;
        [SerializeField] private UIPopupWarning popupConfirm;
        
        private LevelManifest levelManifest;
        private CharacterClass.CharacterClass currentClass;
        
        private void Awake()
        {
            levelManifest = LevelManifest.Instance;
            btnAddLevel.onClick.RemoveAllListeners();
            btnAddLevel.onClick.AddListener(AddLevel);
            btnDeleteLevel.onClick.RemoveAllListeners();
            btnDeleteLevel.onClick.AddListener(DeleteSelectingLevel);
            drdSelectClass.onValueChanged.RemoveAllListeners();
            var options = new List<CharacterClass.CharacterClass>();
            foreach (CharacterClass.CharacterClass mapType in Enum.GetValues(typeof(CharacterClass.CharacterClass)))
            {
                options.Add(mapType);
            }
            drdSelectClass.options = options.Select(mt => 
                new TMP_Dropdown.OptionData(mt.ToString())).ToList();
            drdSelectClass.onValueChanged.AddListener((index) => currentClass = (CharacterClass.CharacterClass)index);
            drdSelectClass.value = 0;
            btnLoadLevels.onClick.RemoveAllListeners();
            btnLoadLevels.onClick.AddListener(LoadLevels);
        }
        
        private void LoadLevels()
        {
            groupSelectClass.gameObject.SetActive(false);
            RefreshLevels(currentClass);    
        }
        
        private void RefreshLevels(CharacterClass.CharacterClass classType)
        {
            if (!levelManifest) return;
#if UNITY_EDITOR
            levelManifest.Validate();
#endif
            var allLevels = levelManifest.GetAllLevels(classType);
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
            var newLevel = levelManifest.GetAllLevels(LevelGateEditorV2.Instance.classType)[^1].level + 1;
            var newLevelAsset = ScriptableObject.CreateInstance<LevelConfig>();
            newLevelAsset.level = newLevel;
#if UNITY_EDITOR
            AssetDatabase.CreateAsset(newLevelAsset, Path.Combine(LevelGateEditorV2.Instance.LevelFolderPath, $"Level {newLevel}.asset"));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            levelManifest.Validate();
            EditorUtility.SetDirty(levelManifest);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
            RefreshLevels(currentClass);
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
            var wavePath = Path.Combine(LevelGateEditorV2.Instance.WaveFolderPath, $"Level {level.level}");
            var levelPath = Path.Combine(LevelGateEditorV2.Instance.LevelFolderPath, $"Level {level.level}.asset");
            AssetDatabase.DeleteAsset(wavePath);
            AssetDatabase.DeleteAsset(levelPath);
            AssetDatabase.Refresh();
            
            LevelManifest.Instance.Validate();
        }
#endif
    }
}