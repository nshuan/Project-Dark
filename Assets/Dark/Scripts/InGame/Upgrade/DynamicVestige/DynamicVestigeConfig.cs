using System;
using System.Collections.Generic;
using Dark.Tools.Utils;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace InGame.Upgrade.DynamicCost
{
    public class DynamicVestigeConfig : SerializedScriptableObject
    {
        public static string FilePath = "Assets/Dark/Resources/UpgradeDynamicVestigeConfig.asset";
        
        public List<UpgradeDynamicVestigeInfo> costInfos = new List<UpgradeDynamicVestigeInfo>();

        public int GetCost1Stage(int indexVestige)
        {
            if (costInfos == null) return 0;
            if (costInfos.Count <= indexVestige) return 0;
            indexVestige = Math.Clamp(indexVestige, 0, costInfos.Count - 1);
            return costInfos[indexVestige].cost1Stage;
        }

        public int[] GetCost5Stage(int indexVestige)
        {
            if (costInfos == null) return new []{ 0, 0, 0, 0, 0 };
            indexVestige = Math.Clamp(indexVestige, 0, costInfos.Count - 1);
            return costInfos[indexVestige].cost5Stages;
        }
        
        public int GetCost1Echoes(int indexEchoes)
        {
            if (costInfos == null) return 0;
            if (costInfos.Count <= indexEchoes) return 0;
            indexEchoes = Math.Clamp(indexEchoes, 0, costInfos.Count - 1);
            return costInfos[indexEchoes].cost1Echoes;
        }

        public int[] GetCost5Echoes(int indexEchoes)
        {
            if (costInfos == null) return new []{ 0, 0, 0, 0, 0 };
            indexEchoes = Math.Clamp(indexEchoes, 0, costInfos.Count - 1);
            return costInfos[indexEchoes].cost5Echoes;
        }
        
        #region SINGLETON

        private static DynamicVestigeConfig instance;

        public static DynamicVestigeConfig Instance
        {
            get
            {
                if (instance == null)
                    instance = Resources.Load<DynamicVestigeConfig>("UpgradeDynamicVestigeConfig");

                return instance;
            }
        }
        #endregion
        
#if UNITY_EDITOR
        [MenuItem("Dark/Upgrade Tree/Generate Dynamic Vestige Config")]
        public static void CreateInstance()
        {
            AssetDatabaseUtils.CreateSOInstance<DynamicVestigeConfig>(FilePath);
        }

        [Button]
        public void SortByIndexAscending()
        {
            costInfos?.Sort((item1, item2) => item1.index.CompareTo(item2.index));
        }
        
        [Button]
        public void SortByIndexDescending()
        {
            costInfos?.Sort((item1, item2) => item2.index.CompareTo(item1.index));
        }
#endif
    }

    [Serializable]
    public class UpgradeDynamicVestigeInfo
    {
        public int index;
        public int cost1Stage;
        public int[] cost5Stages;
        public int cost1Echoes;
        public int[] cost5Echoes;
    }
}