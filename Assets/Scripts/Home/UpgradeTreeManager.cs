using System;
using Core;
using InGame.Upgrade;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Home
{
    public class UpgradeTreeManager : Singleton<UpgradeTreeManager>
    {
        private const string UpgradeTreePath = "DummyUpgradeTreeConfig";
        
        private UpgradeTreeConfig upgradeTree;
        
        public UpgradeBonusInfo BonusInfo { get; private set; }

        public UpgradeTreeManager()
        {
            upgradeTree = Resources.Load<UpgradeTreeConfig>(UpgradeTreePath);
        }
    }

    [Serializable]
    public class UpgradeBonusInfo
    {
        public int damePlus = 0;
        public float dameMultiply = 0f;
        
        [Space]
        public int skillDamePlus = 0;
        public float skillDameMultiply = 0f;
        
        [Space]
        public float criticalRatePlus = 0f;
        public int criticalDame = 0;

        [Space] 
        public float cooldownPlus = 0f;

        [Space] 
        public float skillCooldownPlus = 0f;
        public float skillCooldownMultiply = 0f;

        [Space] 
        public float skillSizeMultiply = 0f;
        public float skillRangeMultiply = 0f;

        [Space] 
        public int bulletPlus = 0;

        [Space]
        public int hpPlus = 0;
        public float hpMultiply = 0f;

        [Space] 
        public float staggerMultiply = 0f;
    }
}