using System;
using System.Collections.Generic;
using Dark.Tools.GoogleSheetTool;
using Dark.Tools.Utils;
using Economic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Dark.Scripts.InGame.Upgrade
{
    public class UpgradeRequirementConfig : SerializedScriptableObject
    {
        public static string FilePath = "Assets/Dark/Resources/UpgradeRequirementConfig.asset";
        
        [TableList]
        public List<UpgradeRequirementInfo> requirementInfos = new List<UpgradeRequirementInfo>();

        public (int, int, int) GetRequirements(int indexVestige, int indexEchoes, int indexSigils)
        {
            return (
                GetRequirement(WealthType.Vestige, indexVestige), 
                GetRequirement(WealthType.Echoes, indexEchoes), 
                GetRequirement(WealthType.Sigils, indexSigils)
                );
        }

        public int GetRequirement(WealthType type, int index)
        {
            if (requirementInfos?.Count <= index) return 0;
            index = Math.Clamp(index, 0, requirementInfos.Count - 1);
            return type switch
            {
                WealthType.Vestige => requirementInfos[index].vestige,
                WealthType.Echoes => requirementInfos[index].echoes,
                WealthType.Sigils => requirementInfos[index].sigils,
                _ => 0
            };
        }
        
        #region SINGLETON

        private static UpgradeRequirementConfig instance;

        public static UpgradeRequirementConfig Instance
        {
            get
            {
                if (instance == null)
                    instance = Resources.Load<UpgradeRequirementConfig>("UpgradeRequirementConfig");

                return instance;
            }
        }
        #endregion
        
#if UNITY_EDITOR
        [MenuItem("Dark/Upgrade Tree/Generate Requirement Config")]
        public static void CreateInstance()
        {
            AssetDatabaseUtils.CreateSOInstance<UpgradeRequirementConfig>(UpgradeRequirementConfig.FilePath);
        }

        [Button]
        public void SortByIndexAscending()
        {
            requirementInfos?.Sort((item1, item2) => item1.index.CompareTo(item2.index));
        }
        
        [Button]
        public void SortByIndexDescending()
        {
            requirementInfos?.Sort((item1, item2) => item2.index.CompareTo(item1.index));
        }
#endif
    }
    
    [Serializable]
    public class UpgradeRequirementInfo
    {
        [HorizontalGroup("Index")] public int index;
        [HorizontalGroup("Vestige")] public int vestige;
        [HorizontalGroup("Echoes")] public int echoes;
        [HorizontalGroup("Sigils")] public int sigils;
    }
}