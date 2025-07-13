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

        public Dictionary<int, UpgradeBonusSkillInfo> skillBonusMapById = new Dictionary<int, UpgradeBonusSkillInfo>();

        #endregion
        
        #region Effect

        [Space] 
        [NonSerialized, OdinSerialize] public Dictionary<EffectTriggerType, List<EffectType>> effectsMapByTriggerType;
        [NonSerialized, OdinSerialize] public Dictionary<EffectType, float> effectsBonusCooldownMapByType;
        [NonSerialized, OdinSerialize] public Dictionary<EffectType, float> effectsBonusChanceMapByType;
        [NonSerialized, OdinSerialize] public Dictionary<EffectType, float> effectsBonusSizeMapByType;
        [NonSerialized, OdinSerialize] public Dictionary<EffectType, float> effectsBonusValueMapByType;
        [NonSerialized, OdinSerialize] public Dictionary<EffectType, float> effectsBonusStaggerMapByType;

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