using System;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame
{
    [Serializable]
    public class UpgradeBonusInfo
    {
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
        [NonSerialized, OdinSerialize] public Dictionary<int, List<int>> bonusEnemyEffectsMapById;
        [NonSerialized, OdinSerialize] public Dictionary<int, List<int>> bonusProjectileEffectsMapById;
        [NonSerialized, OdinSerialize] public Dictionary<int, float> effectsBonusCooldownMapById;
        [NonSerialized, OdinSerialize] public Dictionary<int, float> effectsBonusChanceMapById;
        [NonSerialized, OdinSerialize] public Dictionary<int, float> effectsBonusSizeMapById;
        [NonSerialized, OdinSerialize] public Dictionary<int, float> effectsBonusValueMapById;
        [NonSerialized, OdinSerialize] public Dictionary<int, float> effectsBonusStaggerMapById;

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
        public bool unlockedChargeProjectile = true;
        [NonSerialized, OdinSerialize] public List<IProjectileHit> projectileHitActions = new List<IProjectileHit>();
    }
}