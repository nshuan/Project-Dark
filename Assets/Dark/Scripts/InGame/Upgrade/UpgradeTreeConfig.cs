using System;
using System.Collections.Generic;
using System.Linq;
using InGame.Upgrade.UI;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame.Upgrade
{
    [CreateAssetMenu(menuName = "InGame/Upgrade Tree Config", fileName = "UpgradeTreeConfig")]
    public class UpgradeTreeConfig : SerializedScriptableObject
    {
        [NonSerialized, OdinSerialize] private Dictionary<CharacterClass.CharacterClass, UIUpgradeTree> upgradeTreeMap = new Dictionary<CharacterClass.CharacterClass, UIUpgradeTree>();

        public UIUpgradeTree GetTree(CharacterClass.CharacterClass characterClass)
        {
            return upgradeTreeMap[characterClass];
        }
        
        #region SINGLETON

        private static UpgradeTreeConfig instance;

        public static UpgradeTreeConfig Instance
        {
            get
            {
                if (instance == null)
                    instance = Resources.Load<UpgradeTreeConfig>("UpgradeTreeConfig");

                return instance;
            }
        }
        
        #endregion
    }
}