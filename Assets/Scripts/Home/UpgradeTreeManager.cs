using System;
using System.Linq;
using Core;
using InGame;
using InGame.Upgrade;
using InGame.Upgrade.UIDummy;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Home
{
    public class UpgradeTreeManager : SerializedMonoBehaviour
    {
        private const string UpgradeTreePath = "DummyUpgradeTreeConfig";
        
        private DummyUpgradeTreeConfig upgradeTree;

        [ReadOnly, NonSerialized, OdinSerialize] private UpgradeBonusInfo bonusInfo;
        public UpgradeBonusInfo BonusInfo => bonusInfo;

        protected void Awake()
        {   
            upgradeTree = Resources.Load<DummyUpgradeTreeConfig>(UpgradeTreePath);
            bonusInfo = new UpgradeBonusInfo();
            LevelUtility.BonusInfo = BonusInfo;
        }

        public void ActivateTree()
        {
            upgradeTree.ActivateTree(upgradeTree.nodeMapById.Keys.ToList(), ref bonusInfo);
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