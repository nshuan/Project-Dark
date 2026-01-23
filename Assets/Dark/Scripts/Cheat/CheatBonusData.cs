using System;
using Dark.Tools.Utils;
using InGame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace Cheat
{
    public class CheatBonusData : SerializedScriptableObject
    {
        private static string Path = "CheatBonusData";
        public static string FilePath = "Assets/Dark/Resources/CheatBonusData.asset";

        public bool enabled = false;
        [OdinSerialize, NonSerialized] public UpgradeBonusInfoV2 bonus;

        public static (bool, UpgradeBonusInfoV2) GetBonus()
        {
            var instance = Resources.Load(Path) as CheatBonusData;
            return (instance.enabled, instance?.bonus);
        }
        
        [Button]
        public void Reset()
        {
            bonus = new UpgradeBonusInfoV2();
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