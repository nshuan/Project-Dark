using System;
using Dark.Tools.Utils;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace InGame.Upgrade.CheatBonus
{
    public class CheatBonusData : SerializedScriptableObject
    {
        private static string Path = "CheatBonusData";
        public static string FilePath = "Assets/Dark/Resources/CheatBonusData.asset";

        public bool enabled = false;
        [OdinSerialize, NonSerialized] public UpgradeBonusInfo bonus;

        public static (bool, UpgradeBonusInfo) GetBonus()
        {
            var instance = Resources.Load(Path) as CheatBonusData;
            return (instance.enabled, instance?.bonus);
        }
        
        [Button]
        public void Reset()
        {
            bonus = new UpgradeBonusInfo();
        }
        
#if UNITY_EDITOR
        [MenuItem("Dark/Cheat/Generate Cheat Bonus Data")]
        public static void CreateInstance()
        {
            AssetDatabaseUtils.CreateSOInstance<CheatBonusData>(FilePath);
        }
#endif
    }
}