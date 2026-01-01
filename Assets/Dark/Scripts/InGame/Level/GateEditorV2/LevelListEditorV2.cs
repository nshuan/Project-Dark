using System;
using UnityEngine;

namespace InGame.GateEditorV2
{
    public class LevelListEditorV2 : MonoBehaviour
    {
        [SerializeField] private RectTransform contentHolder;
        [SerializeField] private LevelListItemEditorV2 itemPrefab;
        
        private LevelManifest levelManifest;
        
        private void Awake()
        {
            levelManifest = LevelManifest.Instance;
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
                Destroy(child.gameObject);
            }

            foreach (var level in allLevels)
            {
                var newLevelItem = Instantiate(itemPrefab, contentHolder);
                newLevelItem.name = level.name;
                newLevelItem.UpdateUI(level);
            }
        }
    }
}