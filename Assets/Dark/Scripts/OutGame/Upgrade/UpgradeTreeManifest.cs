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
        
        public UpgradeTreeConfig GetTreeConfig(CharacterClass characterClass)
        {
            return upgradeTreeMap.GetValueOrDefault(characterClass).config;
        }
        
        public UIUpgradeTree GetTreePrefab(CharacterClass characterClass)
        {
            return upgradeTreeMap.GetValueOrDefault(characterClass).prefab;
        }
        
        #region SINGLETON

        private static UpgradeTreeManifest instance;

        public static UpgradeTreeManifest Instance
        {
            get
            {
                if (instance == null)
                    instance = Resources.Load<UpgradeTreeManifest>("UpgradeTreeManifest");

                return instance;
            }
        }
        
        #endregion
    }

    [Serializable]
    public class UpgradeTreeInfo
    {
        public UpgradeTreeConfig config;
        public UIUpgradeTree prefab;
    }
}