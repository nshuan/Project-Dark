using System;
using System.IO;
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
        
        private LevelManifest levelManifest;
        
        private void Awake()
        {
            levelManifest = LevelManifest.Instance;
            btnAddLevel.onClick.RemoveAllListeners();
            btnAddLevel.onClick.AddListener(AddLevel);
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
    }
}