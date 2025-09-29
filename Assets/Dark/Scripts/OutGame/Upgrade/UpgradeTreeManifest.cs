using System;
using System.Collections.Generic;
using InGame.CharacterClass;
using InGame.Upgrade;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade
{
    [CreateAssetMenu(menuName = "Dark/Upgrade/Upgrade Tree Manifest", fileName = "UpgradeTreeManifest")]
    public class UpgradeTreeManifest : SerializedScriptableObject
    {
        [NonSerialized, OdinSerialize] public Dictionary<CharacterClass, UpgradeTreeInfo> upgradeTreeMap = new Dictionary<CharacterClass, UpgradeTreeInfo>();
        
        public static UpgradeTreeConfig GetTreeConfig(CharacterClass characterClass)
        {
            var instance = Resources.Load<UpgradeTreeManifest>("UpgradeTreeManifest");
            var config = instance.upgradeTreeMap.GetValueOrDefault(characterClass).config;
            Resources.UnloadAsset(instance);
            return config;
        }
        
        public static UIUpgradeTree GetTreePrefab(CharacterClass characterClass)
        {
            var instance = Resources.Load<UpgradeTreeManifest>("UpgradeTreeManifest");
            var prefab = instance.upgradeTreeMap.GetValueOrDefault(characterClass).prefab;
            Resources.UnloadAsset(instance);
            return prefab;
        }
    }

    [Serializable]
    public class UpgradeTreeInfo
    {
        public UpgradeTreeConfig config;
        public UIUpgradeTree prefab;
    }
}