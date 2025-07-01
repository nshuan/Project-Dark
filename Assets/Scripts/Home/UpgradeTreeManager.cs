using System;
using Core;
using InGame.Upgrade;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Home
{
    public class UpgradeTreeManager : MonoSingleton<UpgradeTreeManager>
    {
        [SerializeField] private UpgradeTreeConfig upgradeTree;
        
    }

    [Serializable]
    public class UpgradeBonusInfo
    {
        public int damePlus;
        public float dameMultiply;
        
        [Space]
        public int skillDamePlus;
        public float skillDameMultiply;
        
        [Space]
        public float criticalRatePlus;
        public int criticalDame;

        [Space] 
        public float cooldownPlus;
        public float cooldownMultiply;

        [Space] 
        public float skillSizeMultiply;
        public float skillRangeMultiply;

        [Space] 
        public int bulletPlus;

        [Space]
        public int hpPlus;
        public float hpMultiply;

        [Space] 
        public float staggerMultiply;
    }
}