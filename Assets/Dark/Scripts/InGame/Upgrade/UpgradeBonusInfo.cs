using System;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame
{
    [Serializable]
    public class UpgradeBonusInfo
    {
        #region Teleport

        public bool upgradedShortMoveToTower;
        public bool unlockedLongMoveToTower;

        #endregion
        
        #region Bonus

        public int damePlus = 0;
        public float dameMultiply = 0f;
        
        [Space]
        public float criticalRatePlus = 0f;
        public int criticalDame = 0;

        [Space] 
        public float cooldownPlus = 0f;
        
        [Space]
        public int hpPlus = 0;
        public float hpMultiply = 0f;

        #endregion

        #region Skill

        public TowerCounterConfig unlockedTowerCounter;
        public UpgradeBonusSkillInfo skillBonus = new UpgradeBonusSkillInfo();

        #endregion
        
        #region Effect

        [Space] 
        [NonSerialized, OdinSerialize] public Dictionary<PassiveTriggerType, List<PassiveType>> effectsMapByTriggerType;
        [NonSerialized, OdinSerialize] public Dictionary<PassiveType, float> effectsBonusCooldownMapByType;
        [NonSerialized, OdinSerialize] public Dictionary<PassiveType, float> effectsBonusChanceMapByType;
        [NonSerialized, OdinSerialize] public Dictionary<PassiveType, float> effectsBonusSizeMapByType;
        [NonSerialized, OdinSerialize] public Dictionary<PassiveType, float> effectsBonusValueMapByType;
        [NonSerialized, OdinSerialize] public Dictionary<PassiveType, float> effectsBonusStaggerMapByType;

        #endregion
    }

    [Serializable]
    public class UpgradeBonusSkillInfo
    {
        public int skillDamePlus = 0;
        public float skillDameMultiply = 0f;
        
        [Space] 
        public float skillCooldownPlus = 0f;
        public float skillCooldownMultiply = 0f;

        [Space] 
        public float skillSizeMultiply = 0f;
        public float skillRangeMultiply = 0f;
        
        [Space] 
        public int bulletPlus = 0;

        [Space] 
        public float staggerMultiply = 0f;

        [Space] 
        public bool unlockedChargeDame;
        public bool unlockedChargeBullet;
        public bool unlockedChargeSize = true;
        public bool unlockedChargeRange;
        [NonSerialized, OdinSerialize] public List<IProjectileHit> projectileHitActions = new List<IProjectileHit>();
    }
}