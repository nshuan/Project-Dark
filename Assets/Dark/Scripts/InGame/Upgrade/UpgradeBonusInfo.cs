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

        public List<int> unlockedMoveToTower; // 1 for Flash and 2 for Dash

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
        
        #region Passive

        [Space] 
        [NonSerialized, OdinSerialize] public Dictionary<PassiveTriggerType, List<PassiveType>> passiveMapByTriggerType;
        [NonSerialized, OdinSerialize] public Dictionary<PassiveType, float> passiveBonusCooldownMapByType;
        [NonSerialized, OdinSerialize] public Dictionary<PassiveType, float> passiveBonusChanceMapByType;
        [NonSerialized, OdinSerialize] public Dictionary<PassiveType, float> passiveBonusSizeMapByType;
        [NonSerialized, OdinSerialize] public Dictionary<PassiveType, float> passiveBonusValueMapByType;
        [NonSerialized, OdinSerialize] public Dictionary<PassiveType, float> passiveBonusStaggerMapByType;

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

        public int bulletMaxHitPlus = 0;

        [Space] 
        public float staggerMultiply = 0f;

        [Space] 
        public bool unlockedChargeDame;
        public bool unlockedChargeBullet;
        public bool unlockedChargeSize;
        public bool unlockedChargeRange;
        [NonSerialized, OdinSerialize] 
        public List<IProjectileHit> projectileHitActions = new List<IProjectileHit>();
        [NonSerialized, OdinSerialize] 
        public List<IProjectileHit> projectileChargeHitActions = new List<IProjectileHit>();
        [NonSerialized, OdinSerialize]
        public List<IProjectileActivate> projectileActivateActions = new List<IProjectileActivate>();
        [NonSerialized, OdinSerialize]
        public List<IProjectileActivate> projectileChargeActivateActions = new List<IProjectileActivate>();

        public List<IProjectileHit> GetProjectileHitActions(bool isCharge)
        {
            return isCharge ? projectileChargeHitActions : projectileHitActions;
        }

        public List<IProjectileActivate> GetProjectileActivateActions(bool isCharge)
        {
            return isCharge ? projectileChargeActivateActions : projectileActivateActions;
        }
    }
}