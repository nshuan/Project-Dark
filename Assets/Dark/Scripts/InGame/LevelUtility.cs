using System;
using System.Collections.Generic;
using InGame.ChargeConfig;
using InGame.Upgrade;
using UnityEngine;

namespace InGame
{
    public class LevelUtility
    {
        public static UpgradeBonusInfo BonusInfo { get; set; } = new UpgradeBonusInfo();
        public static PlayerStats PlayerStats { get; set; }
        public static PlayerSkillConfig CurrentSkill { get; set; }
        public static Dictionary<ChargeType, PlayerChargeConfig> ChargeConfigMap { get; set; }
        
        public static int BasePlayerDamageWithBonus
        {
            get
            {
                if (BonusInfo == null) return PlayerStats.damage;
                return (int)((PlayerStats.damage + BonusInfo.damePlus) * (1f + BonusInfo.dameMultiply));
            }
        }

        public static float BasePLayerCooldownWithBonus
        {
            get
            {
                if (BonusInfo == null) return PlayerStats.cooldown;
                return (PlayerStats.cooldown + BonusInfo.cooldownPlus) * (1f + BonusInfo.cooldownMultiplier);
            }
        }

        /// <summary>
        /// Player_Damage = [ Base_Damage + Total (Dame_Plus) ] * [1 + Total (Dame_Multiple) ]
        /// Bullet_Dame = [ Player_Damage + Dame_Per_Bullet + Total (Skill_Dame_Plus) ] * [ 1 + Total (Skill_Dame_Multiple) ]
        /// 
        /// Crit_Dame_Multiplier = Crit_Dame_Base + Total (Crit_Dame)
        /// 
        /// Crit: Bullet_Damage_Dealt = Bullet_Dame * [ Crit_Dame_Base + Total (Crit_Dame) ] * [1 + ( Charge_Dame_Max / Charge_Dame_Time ) * Charge_Time ]
        /// Non-Crit: Bullet_Damage_Dealt = Bullet_Dame * [1 + ( Charge_Dame_Max / Charge_Dame_Time ) * Charge_Time ]
        /// </summary>
        /// <param name="playerDamage"></param>
        /// <param name="skillDamage"></param>
        /// <param name="criticalDameMultiplier"></param>
        /// <param name="chargeDameMultiplier"></param>
        /// <returns></returns>
        public static (int, int) GetPlayerBulletDamage(float chargeDameMultiplier)
        {
            var bulletDamage = Mathf.RoundToInt((BasePlayerDamageWithBonus + CurrentSkill.damePerBullet + BonusInfo.skillBonus.skillDamePlus) * (1 + BonusInfo.skillBonus.skillDameMultiply));
            var criticalDameMultiplier = PlayerStats.criticalDamage + BonusInfo.criticalDame;
            return LevelTemporaryUtility.FilterPlayerBulletDamage(
                Mathf.RoundToInt(bulletDamage * chargeDameMultiplier),
                Mathf.RoundToInt(bulletDamage * criticalDameMultiplier * chargeDameMultiplier), 
                BonusInfo);
        }

        /// <summary>
        /// Tỷ lệ random = Crit_Rate_Base + Total (Crit_Rate_Plus)
        /// </summary>
        /// <param name="baseCriticalRate"></param>
        /// <returns></returns>
        public static float GetCriticalRate()
        {
            return PlayerStats.criticalRate + BonusInfo.criticalRatePlus;
        }

        /// <summary>
        /// Number_Of_Bullet = Bullet + Total (Bullet_Plus) + Max [ RoundDown (Charge_Time / Charge_Bullet_Interval), Max_Bullet_Add ]
        /// </summary>
        /// <param name="skillId"></param>
        /// <param name="baseBulletNum"></param>
        /// <param name="chargeBulletNum"></param>
        /// <returns></returns>
        public static int GetNumberOfBullets(int chargeBulletNum)
        {
            return CurrentSkill.numberOfBullets + BonusInfo.skillBonus.bulletPlus + chargeBulletNum;
        }

        /// <summary>
        /// Player_Cooldown = Base_Cooldown + Total (Cooldown_Plus)
        /// Skill_Cooldown = [Skill_Cooldown_Base - Total (Skill_Cooldown_Plus) ] * [ 1 - Total (Skill_Cooldown_Multiple) ]
        /// Final_Cooldown = Skill_Cooldown * (1 - Player_Cooldown)
        /// </summary>
        /// <param name="playerCooldown"></param>
        /// <param name="baseSkillCooldown"></param>
        /// <returns></returns>
        public static float GetSkillCooldown(bool isCharge)
        {
            var baseCooldown = isCharge ? CurrentSkill.chargeCooldown : CurrentSkill.cooldown;
            return LevelTemporaryUtility.FilterSkillCooldown(Mathf.Max(0f,
                (baseCooldown - BonusInfo.skillBonus.skillCooldownPlus) * (1 - BonusInfo.skillBonus
                                                                                 .skillCooldownMultiply)
                                                                             * Mathf.Clamp(
                                                                                 1 - BasePLayerCooldownWithBonus,
                                                                                 0f, 1f)), BonusInfo);
        }

        /// <summary>
        /// Skill_Range = Range * [1 + Total (Skill_Range_Multiple) ] * [ 1 + ( Charge_Range_Max / Charge_Range_Time ) * Charge_Time ]
        /// </summary>
        /// <param name="baseRange"></param>
        /// <param name="chargeRange"></param>
        /// <returns></returns>
        public static float GetSkillRange(float chargeRange, Vector2 direction)
        {
            // Calculate the ratio: true_range / skill_range
            var magnitude = direction.magnitude;
            direction.x = Mathf.Abs(direction.x) / magnitude;
            direction.y = Mathf.Abs(direction.y) / magnitude;
            var angle = Mathf.Atan2(direction.y, direction.x);
            var ratio = GameConst.IsoRatio
                        / Mathf.Sqrt(Mathf.Pow(GameConst.IsoRatio * Mathf.Cos(angle), 2) +
                                     Mathf.Pow(Mathf.Sin(angle), 2));
            
            return CurrentSkill.range * (1 + BonusInfo.skillBonus.skillRangeMultiply) * chargeRange * ratio;
        }

        public static float GetRelativeRange(float maxRange, Vector2 direction)
        {
            var magnitude = direction.magnitude;
            direction.x = Mathf.Abs(direction.x) / magnitude;
            direction.y = Mathf.Abs(direction.y) / magnitude;
            var angle = Mathf.Atan2(direction.y, direction.x);
            var ratio = GameConst.IsoRatio
                        / Mathf.Sqrt(Mathf.Pow(GameConst.IsoRatio * Mathf.Cos(angle), 2) +
                                     Mathf.Pow(Mathf.Sin(angle), 2));
            return maxRange * ratio;
        }

        public static float GetTrueRange(float relativeRange, Vector2 direction)
        {
            var magnitude = direction.magnitude;
            direction.x = Mathf.Abs(direction.x) / magnitude;
            direction.y = Mathf.Abs(direction.y) / magnitude;
            var angle = Mathf.Atan2(direction.y, direction.x);
            var ratio = GameConst.IsoRatio
                        / Mathf.Sqrt(Mathf.Pow(GameConst.IsoRatio * Mathf.Cos(angle), 2) +
                                     Mathf.Pow(Mathf.Sin(angle), 2));
            if (ratio == 0f) ratio = 0.0001f;
            return relativeRange / ratio;
        }

        public static Vector2 GetIntersectionInRangeBound(Vector2 rangeCenter, float trueRange, Vector2 vectorOrigin,
            Vector2 vectorDirection)
        {
            vectorOrigin = vectorOrigin - rangeCenter;
            var a = trueRange;
            var b = trueRange * GameConst.IsoRatio;
            var A = a * a * vectorDirection.y * vectorDirection.y + b * b * vectorDirection.x * vectorDirection.x;
            var B = 2 * (a * a * vectorDirection.y * vectorOrigin.y + b * b * vectorDirection.x * vectorOrigin.x);
            var C = a * a * vectorOrigin.y * vectorOrigin.y + b * b * vectorOrigin.x * vectorOrigin.x - a * a * b * b;
            var delta = B * B - 4 * A * C;
            
            // delta < 0 => no intersection => return vectorOrigin + vectorDirection
            if (delta < 0) return vectorOrigin + rangeCenter + vectorDirection;
            else
            {
                var t = (-B + Mathf.Sqrt(delta)) / (2 * A);
                if (t < 0) t = (-B - Mathf.Sqrt(delta)) / (2 * A);
                return vectorOrigin + rangeCenter + vectorDirection * t;
            }
        }
        
        /// <summary>
        /// Skill_Size = Size * [1 + Total (Skill_Size_Multiple) ] * [ 1 + ( Charge_Size_Max / Charge_Size_Time ) * Charge_Time ]
        /// </summary>
        /// <param name="baseSize"></param>
        /// <param name="chargeSize"></param>
        /// <returns></returns>
        public static float GetSkillSize(float chargeSize)
        {
            return CurrentSkill.size * (1 + BonusInfo.skillBonus.skillSizeMultiply) * chargeSize;
        }

        /// <summary>
        /// Total_Stagger = Stagger * [1 + Total (Stagger_Multiple) ]
        /// </summary>
        /// <param name="baseStagger"></param>
        /// <returns></returns>
        public static float GetBulletStagger()
        {
            return CurrentSkill.stagger * (1 + BonusInfo.skillBonus.staggerMultiply);
        }

        public static float GetDropRate(float baseDropRate)
        {
            return (baseDropRate + BonusInfo.dropRatePlus) * (1f + BonusInfo.dropRateMultiply);
        }

        #region Charge

        // Should not be less than 0.1f, too small
        public static float GetChargeStepTime()
        {
            return Mathf.Max((CurrentSkill.chargeStepTime - BonusInfo.chargeBonus.stepTime) * (1f - Mathf.Clamp(BonusInfo.chargeBonus.stepTimeMul, 0f, 1f)), 0.1f);
        }

        public static float GetChargeBulletPerStep()
        {
            return CurrentSkill.chargeBulletStep + BonusInfo.chargeBonus.bulletPerStep;
        }

        public static int GetChargeBulletMaxStep()
        {
            return (int)ChargeConfigMap[ChargeType.Bullet].value + BonusInfo.chargeBonus.bulletMaxStep;
        }
        
        public static float GetChargeDamePerStep()
        {
            return CurrentSkill.chargeDameStep + BonusInfo.chargeBonus.damePerStep;
        }

        public static int GetChargeDameMaxStep()
        {
            return CurrentSkill.chargeDameMaxStep + BonusInfo.chargeBonus.dameMaxStep;
        }
        
        public static float GetChargeSizePerStep()
        {
            return CurrentSkill.chargeSizeStep + BonusInfo.chargeBonus.sizePerStep;
        }

        public static int GetChargeSizeMaxStep()
        {
            return CurrentSkill.chargeSizeMaxStep + BonusInfo.chargeBonus.sizeMaxStep;
        }
        
        public static float GetChargeRangePerStep()
        {
            return CurrentSkill.chargeRangeStep + BonusInfo.chargeBonus.rangePerStep;
        }

        public static int GetChargeRangeMaxStep()
        {
            return CurrentSkill.chargeRangeMaxStep + BonusInfo.chargeBonus.rangeMaxStep;
        }

        public static int GetChargeSizeExplodeBullet(int baseBullet)
        {
            return baseBullet + BonusInfo.chargeBonus.maxBulletExplodeChargeSize;
        }
        #endregion
        
        #region Passive

        public static float GetPassiveCooldown(PassiveType passiveType, float baseCooldown)
        {
            if (BonusInfo.passiveBonusCooldownMapByType == null) return Mathf.Max(baseCooldown * (1f - BasePLayerCooldownWithBonus), 0f);
            if (BonusInfo.passiveBonusCooldownMapByType.TryGetValue(passiveType, out var bonus))
                return Mathf.Max(baseCooldown * (1f - bonus) * (1f - BasePLayerCooldownWithBonus), 0f);
            return Mathf.Max(baseCooldown * (1f - BasePLayerCooldownWithBonus), 0f);
        }

        public static float GetPassiveChance(PassiveType passiveType, float baseChance)
        {
            if (BonusInfo.passiveBonusChanceMapByType == null) return baseChance;
            if (BonusInfo.passiveBonusChanceMapByType.TryGetValue(passiveType, out var bonus))
                return Mathf.Min(baseChance * (1f + bonus), 1f);
            return baseChance;
        }
        
        public static float GetPassiveSize(PassiveType passiveType, float baseSize)
        {
            if (BonusInfo.passiveBonusSizeMapByType == null) return baseSize;
            if (BonusInfo.passiveBonusSizeMapByType.TryGetValue(passiveType, out var bonus))
                return baseSize * (1f + bonus);
            return baseSize;
        }

        public static float GetPassiveValue(PassiveType passiveType, float baseValue)
        {
            if (BonusInfo.passiveBonusValueMapByType == null) return baseValue + BasePlayerDamageWithBonus;
            if (BonusInfo.passiveBonusValueMapByType.TryGetValue(passiveType, out var bonus))
                return (baseValue + BasePlayerDamageWithBonus) * (1f + bonus);
            return baseValue + BasePlayerDamageWithBonus;
        }

        public static float GetPassiveStagger(PassiveType passiveType, float baseStagger)
        {
            if (BonusInfo.passiveBonusStaggerMapByType == null) return baseStagger;
            if (BonusInfo.passiveBonusStaggerMapByType.TryGetValue(passiveType, out var bonus))
                return baseStagger * (1f + bonus);
            return baseStagger;
        }

        #endregion

        #region Move Towers

        public static float GetTeleCooldown(float baseCooldown)
        {
            return Mathf.Max((baseCooldown - BonusInfo.moveCooldownPlus) * (1f - BonusInfo.moveCooldownMultiplier) * (1f - BasePLayerCooldownWithBonus), 0f);
        }
        
        public static float GetDashCooldown(float baseCooldown)
        {
            return Mathf.Max((baseCooldown - BonusInfo.dashCooldownPlus) * (1f - BonusInfo.dashCooldownMultiplier) * (1f - BasePLayerCooldownWithBonus), 0f);
        }

        public static float GetDashSize(float baseSize)
        {
            return (1f + BonusInfo.dashSizeMultiplier) * (baseSize + BonusInfo.dashSizePlus);
        }

        public static int GetDashDamage(int baseDamage)
        {
            return (int)((1f + BonusInfo.dashDamageMultiplier) * (baseDamage + BonusInfo.dashDamagePlus + BasePlayerDamageWithBonus));
        }

        public static float GetFlashCooldown(float baseCooldown)
        {
            return Mathf.Max((baseCooldown - BonusInfo.flashCooldownPlus) * (1f - BonusInfo.flashCooldownMultiplier) * (1f - BasePLayerCooldownWithBonus), 0f);
        }

        public static float GetFlashSize(float baseSize)
        {
            return (1f + BonusInfo.flashSizeMultiplier) * (baseSize + BonusInfo.flashSizePlus);
        }

        public static int GetFlashDamage(int baseDamage)
        {
            return (int)((1f + BonusInfo.flashDamageMultiplier) * (baseDamage + BonusInfo.flashDamagePlus + BasePlayerDamageWithBonus));
        }

        #endregion

        #region Tower

        /// <summary>
        /// HP = [ Player_HP + Total (HP_Plus) ] * [ 1 + Total (HP_Multiple) ]
        /// </summary>
        /// <param name="baseHealth"></param>
        /// <returns></returns>
        public static int GetTowerHp()
        {
            return (int)((1f + BonusInfo.hpMultiply) * (PlayerStats.hp + BonusInfo.hpPlus));
        }

        public static int GetTowerCounterDamage(int baseDamage)
        {
            return (int)((BasePlayerDamageWithBonus + baseDamage) * (1f +BonusInfo.towerCounterDamagePlus));
        }

        public static float GetTowerCounterCooldown(NodeTowerCounter.CounterType counterType, float baseCooldown)
        {
            var bonus = BonusInfo.towerCounterCooldownPlus.GetValueOrDefault(counterType, 0f);
            return Mathf.Max((baseCooldown - bonus) * (1f - BasePLayerCooldownWithBonus), 0f);
        }

        public static float GetTowerCounterRange(NodeTowerCounter.CounterType counterType, float baseRange)
        {
            return baseRange;
        }
        
        public static int GetTowerAutoRegen(int maxHp)
        {
            return (int)(BonusInfo.toleranceRegenPercentPerSecond);
        }

        public static int GetTowerRegenOnKill(int maxHp)
        {
            return (int)(BonusInfo.toleranceRegenPercentWhenKill * maxHp);
        }
        
        #endregion
    }
}