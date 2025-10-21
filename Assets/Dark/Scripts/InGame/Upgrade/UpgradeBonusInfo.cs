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
        public float moveCooldownPlus = 0f;
        
        public float dashCooldownPlus = 0f;
        public float dashSizePlus = 0f;
        public int dashDamagePlus = 0;
        public float dashDamageMultiplier = 0f;
        
        public float flashCooldownPlus = 0f;
        public float flashSizePlus = 0f;
        public int flashDamagePlus = 0;
        public float flashDamageMultiplier = 0f;

        #endregion
        
        #region Bonus

        public int damePlus = 0;
        public float dameMultiply = 0f;
        
        [Space]
        public float criticalRatePlus = 0f;
        public int criticalDame = 0;

        [Space] 
        public float cooldownPlus = 0f;
        public float cooldownMultiplier = 0f;
        
        [Space]
        public int hpPlus = 0;
        public float hpMultiply = 0f;

        [Space] 
        public float dropRatePlus = 0;
        public float dropRateMultiply = 0;
        
        #endregion

        #region Skill
        
        public UpgradeBonusSkillInfo skillBonus = new UpgradeBonusSkillInfo();
        
        public UpgradeBonusChargeInfo chargeDameBonus = new UpgradeBonusChargeInfo();
        public UpgradeBonusChargeInfo chargeBulletBonus = new UpgradeBonusChargeInfo();
        public UpgradeBonusChargeInfo chargeSizeBonus = new UpgradeBonusChargeInfo();
        public UpgradeBonusChargeInfo chargeRangeBonus = new UpgradeBonusChargeInfo();

        #endregion
        
        #region Tower

        public bool unlockedTowerCounter;
        public float toleranceRegenPercentPerSecond = 0;
        public float toleranceRegenPercentWhenKill = 0;
        public float towerCounterCooldownPlus = 0f;
        public int towerCounterDamagePlus = 0;  

        #endregion
        
        #region Passive

        [Space] 
        [NonSerialized, OdinSerialize] public Dictionary<PassiveTriggerType, List<PassiveType>> passiveMapByTriggerType;
        [NonSerialized, OdinSerialize] public Dictionary<PassiveType, float> passiveBonusSizeMapByType;
        [NonSerialized, OdinSerialize] public Dictionary<PassiveType, float> passiveBonusValueMapByType;
        [NonSerialized, OdinSerialize] public Dictionary<PassiveType, float> passiveBonusCooldownMapByType;
        [NonSerialized, OdinSerialize] public Dictionary<PassiveType, float> passiveBonusChanceMapByType;
        [NonSerialized, OdinSerialize] public Dictionary<PassiveType, float> passiveBonusStaggerMapByType;

        #endregion

        #region Temporary

        public UpgradeBonusTempInfo tempDamageBonusOnMove;
        public UpgradeBonusTempInfo tempDamageBonusOnKill;
        public UpgradeBonusTempInfo tempAtkSpeBonusOnMove;
        public UpgradeBonusTempInfo tempAtkSpeBonusOnKill;

        #endregion
        
        public UpgradeBonusInfo()
        {
            skillBonus = new UpgradeBonusSkillInfo();
            passiveMapByTriggerType = new Dictionary<PassiveTriggerType, List<PassiveType>>();
        }
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

    [Serializable]
    public class UpgradeBonusChargeInfo
    {
        public float maxDameMultiplier;
        public float maxDameChargeTime;

        public int maxBulletAdd;
        public float bulletAddInterval;

        public float maxSizeMultiplier;
        public float maxSizeChargeTime;

        public float maxRangeMultiplier;
        public float maxRangeChargeTime;
    }

    [Serializable]
    public class UpgradeBonusTempInfo
    {
        public float bonusValue;
        public float bonusDuration;
    }
}