using System;
using Core;
using InGame.Upgrade;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Home
{
    public class UpgradeTreeManager : MonoSingleton<UpgradeTreeManager>
    {
        [SerializeField] private UpgradeTreeConfig upgradeTree;
        
        [field: ReadOnly, NonSerialized, OdinSerialize] public UpgradeBonusInfo BonusInfo { get; private set; }
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