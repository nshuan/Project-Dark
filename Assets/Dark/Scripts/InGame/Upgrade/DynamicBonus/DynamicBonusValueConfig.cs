using System;
using System.Collections.Generic;
using Dark.Tools.GoogleSheetTool;
using Dark.Tools.Utils;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace InGame.Upgrade.DynamicBonus
{
    public class DynamicBonusValueConfig : SerializedScriptableObject
    {
        public static string FilePath = "Assets/Dark/Resources/UpgradeDynamicBonusValueConfig.asset";

        public Dictionary<NodeBonusTypeV2, List<UpgradeDynamicBonusValueInfo>> bonusInfos =
            new Dictionary<NodeBonusTypeV2, List<UpgradeDynamicBonusValueInfo>>();

        public float GetBonus1Stage(NodeBonusTypeV2 bonusType, int index)
        {
            if (bonusInfos == null) return 0;
            if (!bonusInfos.TryGetValue(bonusType, out var bonusInfo)) return 0;
            if (bonusInfo.Count <= index) return 0;
            index = Math.Clamp(index, 0, bonusInfos[bonusType].Count - 1);
            return bonusInfos[bonusType][index].bonus1Stage;
        }

        public float[] GetBonus5Stage(NodeBonusTypeV2 bonusType, int index)
        {
            if (bonusInfos == null) return new []{ 0f, 0, 0, 0, 0 };
            if (!bonusInfos.TryGetValue(bonusType, out var bonusInfo)) return new []{ 0f, 0, 0, 0, 0 };
            if (bonusInfo.Count <= index) return new []{ 0f, 0, 0, 0, 0 };
            index = Math.Clamp(index, 0, bonusInfos[bonusType].Count - 1);
            return bonusInfos[bonusType][index].bonus5Stages;
        }
        
        #region SINGLETON

        private static DynamicBonusValueConfig instance;

        public static DynamicBonusValueConfig Instance
        {
            get
            {
                if (instance == null)
                    instance = Resources.Load<DynamicBonusValueConfig>("UpgradeDynamicBonusValueConfig");

                return instance;
            }
        }
        #endregion
        
#if UNITY_EDITOR
        [MenuItem("Dark/Upgrade Tree/Generate Dynamic Bonus Value Config")]
        public static void CreateInstance()
        {
            AssetDatabaseUtils.CreateSOInstance<DynamicBonusValueConfig>(FilePath);
        }
#endif
    }

    [Serializable]
    public class UpgradeDynamicBonusValueInfo
    {
        public int index;
        public float bonus1Stage;
        public float[] bonus5Stages;
    }
}