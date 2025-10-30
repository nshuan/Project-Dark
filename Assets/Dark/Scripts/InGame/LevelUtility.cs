using System;
using System.Collections.Generic;
using InGame.Upgrade;
using UnityEngine;

namespace InGame
{
    public class LevelUtility
    {
        public static UpgradeBonusInfo BonusInfo { get; set; } = new UpgradeBonusInfo();
        public static PlayerStats PlayerStats { get; set; }
        public static PlayerSkillConfig CurrentSkill { get; set; }

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
        public static float GetSkillCooldown()
        {
            return LevelTemporaryUtility.FilterSkillCooldown(Mathf.Max(0f,
                (CurrentSkill.cooldown - BonusInfo.skillBonus.skillCooldownPlus) * (1 - BonusInfo.skillBonus
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

        public static float GetChargeStepTime()
        {
            return CurrentSkill.chargeStepTime;
        }

        public static float GetChargeBulletPerStep()
        {
            return CurrentSkill.chargeBulletStep;
        }

        public static float GetChargeBulletMaxTime()
        {
            return CurrentSkill.chargeBulletTime;
        }
        
        public static float GetChargeDamePerStep()
        {
            return CurrentSkill.chargeDameStep;
        }

        public static float GetChargeDameMaxTime()
        {
            return CurrentSkill.chargeDameTime;
        }
        
        public static float GetChargeSizePerStep()
        {
            return CurrentSkill.chargeSizeStep;
        }

        public static float GetChargeSizeMaxTime()
        {
            return CurrentSkill.chargeSizeTime;
        }
        
        public static float GetChargeRangePerStep()
        {
            return CurrentSkill.chargeRangeStep;
        }

        public static float GetChargeRangeMaxTime()
        {
            return CurrentSkill.chargeRangeTime;
        }
        
        /// <summary>
        /// Get the MaxDameMultiplier of the max MaxDameMultiplierAdd / MaxDameChargeTime
        /// 4 loại charge đều sẽ bonus vào damage, lấy bonus từ loọi charge mà MaxDameMultiplierAdd / MaxDameChargeTime lớn nhất
        /// </summary>
        /// <param name="baseChargeMaxDameMultiplier"></param>
        /// <param name="baseChargeMaxTime"></param>
        /// <returns>(MaxDame, MaxChargeTime)</returns>
        public static (float, float) GetChargeDameMax(float baseChargeMaxDameMultiplier, float baseChargeMaxTime)
        {
            var max = 0f;
            var bonusMD = 0f;
            var resultMCT = baseChargeMaxTime;
            var tempMCT = 0f;
            var temp = 0f;
            if (BonusInfo.chargeBulletBonus?.maxDameChargeTimeMinus > 0)
            {
                tempMCT = (baseChargeMaxTime - BonusInfo.chargeBulletBonus.maxDameChargeTimeMinus) * (1f - BonusInfo.chargeBulletBonus.maxDameChargeTimeMinusMul);
                if (tempMCT <= 0) tempMCT = 1f;
                temp = (baseChargeMaxDameMultiplier + BonusInfo.chargeBulletBonus.maxDameMultiplier) / tempMCT;
                if (temp > max)
                {
                    max = temp;
                    bonusMD = BonusInfo.chargeBulletBonus.maxDameMultiplier;
                    resultMCT = (baseChargeMaxTime - BonusInfo.chargeBulletBonus.maxDameChargeTimeMinus) * (1f - BonusInfo.chargeBulletBonus.maxDameChargeTimeMinusMul);
                }
            }

            if (BonusInfo.chargeSizeBonus?.maxDameChargeTimeMinus > 0)
            {
                tempMCT = (baseChargeMaxTime - BonusInfo.chargeSizeBonus.maxDameChargeTimeMinus) * (1f - BonusInfo.chargeSizeBonus.maxDameChargeTimeMinusMul);
                if (tempMCT <= 0) tempMCT = 1f;
                temp = (baseChargeMaxDameMultiplier + BonusInfo.chargeSizeBonus.maxDameMultiplier) / tempMCT;
                if (temp > max)
                {
                    max = temp;
                    bonusMD = BonusInfo.chargeSizeBonus.maxDameMultiplier;
                    resultMCT = tempMCT;
                }
            }

            if (BonusInfo.chargeRangeBonus?.maxDameChargeTimeMinus > 0)
            {
                tempMCT = (baseChargeMaxTime - BonusInfo.chargeRangeBonus.maxDameChargeTimeMinus) * (1f - BonusInfo.chargeRangeBonus.maxDameChargeTimeMinusMul);
                if (tempMCT <= 0) tempMCT = 1f;
                temp = (baseChargeMaxDameMultiplier + BonusInfo.chargeRangeBonus.maxDameMultiplier) / tempMCT;
                if (temp > max)
                {
                    max = temp;
                    bonusMD = BonusInfo.chargeRangeBonus.maxDameMultiplier;
                    resultMCT = tempMCT;
                }
            }

            if (BonusInfo.chargeDameBonus?.maxDameChargeTimeMinus > 0)
            {
                tempMCT = (baseChargeMaxTime - BonusInfo.chargeDameBonus.maxDameChargeTimeMinus) * (1f - BonusInfo.chargeDameBonus.maxDameChargeTimeMinusMul);
                if (tempMCT <= 0) tempMCT = 1f;
                temp = (baseChargeMaxDameMultiplier + BonusInfo.chargeDameBonus.maxDameMultiplier) / tempMCT;
                if (temp > max)
                {
                    max = temp;
                    bonusMD = BonusInfo.chargeDameBonus.maxDameMultiplier;
                    resultMCT = tempMCT;
                }
            }

            return (baseChargeMaxDameMultiplier + bonusMD, resultMCT);
        }

        public static (float, float) GetChargeSizeMax(float baseChargeSize, float baseChargeSizeTime)
        {
            var max = 0f;
            var bonusMD = 0f;
            var resultMCT = baseChargeSizeTime;
            var tempMCT = 0f;
            var temp = 0f;
            if (BonusInfo.chargeBulletBonus?.maxSizeChargeTimeMinus > 0)
            {
                tempMCT = (baseChargeSizeTime - BonusInfo.chargeBulletBonus.maxSizeChargeTimeMinus) * (1f - BonusInfo.chargeBulletBonus.maxSizeChargeTimeMinusMul);
                if (tempMCT <= 0) tempMCT = 1f;
                temp = (baseChargeSize + BonusInfo.chargeBulletBonus.maxSizeMultiplier) / tempMCT;
                if (temp > max)
                {
                    max = temp;
                    bonusMD = BonusInfo.chargeBulletBonus.maxSizeMultiplier;
                    resultMCT = tempMCT;
                }
            }

            if (BonusInfo.chargeRangeBonus?.maxSizeChargeTimeMinus > 0)
            {
                tempMCT = (baseChargeSizeTime - BonusInfo.chargeRangeBonus.maxSizeChargeTimeMinus) * (1f - BonusInfo.chargeRangeBonus.maxSizeChargeTimeMinusMul);
                if (tempMCT <= 0) tempMCT = 1f;
                temp = (baseChargeSize + BonusInfo.chargeRangeBonus.maxSizeMultiplier) / tempMCT;
                if (temp > max)
                {
                    max = temp;
                    bonusMD = BonusInfo.chargeRangeBonus.maxSizeMultiplier;
                    resultMCT = tempMCT;
                }
            }

            if (BonusInfo.chargeDameBonus?.maxSizeChargeTimeMinus > 0)
            {
                tempMCT = (baseChargeSizeTime - BonusInfo.chargeDameBonus.maxSizeChargeTimeMinus) * (1f - BonusInfo.chargeDameBonus.maxSizeChargeTimeMinusMul);
                if (tempMCT <= 0) tempMCT = 1f;
                temp = (baseChargeSize + BonusInfo.chargeDameBonus.maxSizeMultiplier) / tempMCT;
                if (temp > max)
                {
                    max = temp;
                    bonusMD = BonusInfo.chargeDameBonus.maxSizeMultiplier;
                    resultMCT = tempMCT;
                }
            }
            
            if (BonusInfo.chargeSizeBonus?.maxDameChargeTimeMinus > 0)
            {
                tempMCT = (baseChargeSizeTime - BonusInfo.chargeSizeBonus.maxSizeChargeTimeMinus) * (1f - BonusInfo.chargeSizeBonus.maxSizeChargeTimeMinusMul);
                if (tempMCT <= 0) tempMCT = 1f;
                temp = (baseChargeSize + BonusInfo.chargeSizeBonus.maxSizeMultiplier) / tempMCT;
                if (temp > max)
                {
                    max = temp;
                    bonusMD = BonusInfo.chargeSizeBonus.maxSizeMultiplier;
                    resultMCT = tempMCT;
                }
            }

            return (baseChargeSize + bonusMD, resultMCT);
        }
        
        public static (int, float) GetChargeBulletMax(int baseChargeBullet, float baseChargeBulletInterval)
        {
            var max = 0;
            var bulletAdd = 0;
            var resultInterval = baseChargeBulletInterval;
            if (BonusInfo.chargeBulletBonus?.maxBulletAdd > max)
            {
                max = BonusInfo.chargeBulletBonus.maxBulletAdd;
                bulletAdd = BonusInfo.chargeBulletBonus.maxBulletAdd;
                resultInterval = (baseChargeBulletInterval - BonusInfo.chargeBulletBonus.bulletAddIntervalMinus) * (1f - BonusInfo.chargeBulletBonus.bulletAddIntervalMinusMul);
            }

            if (BonusInfo.chargeDameBonus?.maxBulletAdd > max)
            {
                max = BonusInfo.chargeDameBonus.maxBulletAdd;
                bulletAdd = BonusInfo.chargeDameBonus.maxBulletAdd;
                resultInterval = (baseChargeBulletInterval - BonusInfo.chargeDameBonus.bulletAddIntervalMinus) * (1f - BonusInfo.chargeDameBonus.bulletAddIntervalMinusMul);
            }

            if (BonusInfo.chargeRangeBonus?.maxBulletAdd > max)
            {
                max = BonusInfo.chargeRangeBonus.maxBulletAdd;
                bulletAdd = BonusInfo.chargeRangeBonus.maxBulletAdd;
                resultInterval = (baseChargeBulletInterval - BonusInfo.chargeRangeBonus.bulletAddIntervalMinus) * (1f - BonusInfo.chargeRangeBonus.bulletAddIntervalMinusMul);
            }
            
            if (BonusInfo.chargeSizeBonus?.maxBulletAdd > max)
            {
                max = BonusInfo.chargeSizeBonus.maxBulletAdd;
                bulletAdd = BonusInfo.chargeSizeBonus.maxBulletAdd;
                resultInterval = (baseChargeBulletInterval - BonusInfo.chargeSizeBonus.bulletAddIntervalMinus) * (1f - BonusInfo.chargeSizeBonus.bulletAddIntervalMinusMul);
            }

            return (baseChargeBullet + bulletAdd, resultInterval);
        }
        
        public static (float, float) GetChargeRangeMax(float baseChargeRange, float baseChargeRangeTime)
        {
            var max = 0f;
            var bonusMR = 0f;
            var resultMRT = baseChargeRangeTime;
            var tempMCT = 0f;
            var temp = 0f;
            if (BonusInfo.chargeBulletBonus?.maxRangeChargeTimeMinus > 0)
            {
                tempMCT = (baseChargeRangeTime - BonusInfo.chargeBulletBonus.maxRangeChargeTimeMinus) * (1f - BonusInfo.chargeBulletBonus.maxRangeChargeTimeMinusMul);
                if (tempMCT <= 0) tempMCT = 1f;
                temp = (baseChargeRange + BonusInfo.chargeBulletBonus.maxRangeMultiplier) / tempMCT;
                if (temp > max)
                {
                    max = temp;
                    bonusMR = BonusInfo.chargeBulletBonus.maxRangeMultiplier;
                    resultMRT = tempMCT;
                }
            }

            if (BonusInfo.chargeRangeBonus?.maxRangeChargeTimeMinus > 0)
            {
                tempMCT = (baseChargeRangeTime - BonusInfo.chargeRangeBonus.maxRangeChargeTimeMinus) * (1f - BonusInfo.chargeRangeBonus.maxRangeChargeTimeMinusMul);
                if (tempMCT <= 0) tempMCT = 1f;
                temp = (baseChargeRange + BonusInfo.chargeRangeBonus.maxRangeMultiplier) / tempMCT;
                if (temp > max)
                {
                    max = temp;
                    bonusMR = BonusInfo.chargeRangeBonus.maxRangeMultiplier;
                    resultMRT = tempMCT;
                }
            }

            if (BonusInfo.chargeDameBonus?.maxRangeChargeTimeMinus > 0)
            {
                tempMCT = (baseChargeRangeTime - BonusInfo.chargeDameBonus.maxRangeChargeTimeMinus) * (1f - BonusInfo.chargeDameBonus.maxRangeChargeTimeMinusMul);
                if (tempMCT <= 0) tempMCT = 1f;
                temp = (baseChargeRange + BonusInfo.chargeDameBonus.maxRangeMultiplier) / tempMCT;
                if (temp > max)
                {
                    max = temp;
                    bonusMR = BonusInfo.chargeDameBonus.maxRangeMultiplier;
                    resultMRT = tempMCT;
                }
            }
            
            if (BonusInfo.chargeSizeBonus?.maxRangeChargeTimeMinus > 0)
            {
                tempMCT = (baseChargeRangeTime - BonusInfo.chargeSizeBonus.maxRangeChargeTimeMinus) * (1f - BonusInfo.chargeSizeBonus.maxRangeChargeTimeMinusMul);
                if (tempMCT <= 0) tempMCT = 1f;
                temp = (baseChargeRange + BonusInfo.chargeSizeBonus.maxRangeMultiplier) / tempMCT;
                if (temp > max)
                {
                    max = temp;
                    bonusMR = BonusInfo.chargeSizeBonus.maxRangeMultiplier;
                    resultMRT = tempMCT;
                }
            }

            return (baseChargeRange + bonusMR, resultMRT);
        }
        
        #endregion
        
        #region Passive

        public static float GetPassiveCooldown(PassiveType passiveType, float baseCooldown)
        {
            if (BonusInfo.passiveBonusCooldownMapByType == null) return Mathf.Max(baseCooldown, 0f);
            if (BonusInfo.passiveBonusCooldownMapByType.TryGetValue(passiveType, out var bonus))
                return Mathf.Max((baseCooldown - bonus) * (1f - BasePLayerCooldownWithBonus), 0f);
            return Mathf.Max(baseCooldown, 0f);
        }

        public static float GetPassiveChance(PassiveType passiveType, float baseChance)
        {
            if (BonusInfo.passiveBonusChanceMapByType == null) return baseChance;
            if (BonusInfo.passiveBonusChanceMapByType.TryGetValue(passiveType, out var bonus))
                return Mathf.Min(baseChance + bonus, 1f);
            return baseChance;
        }

        public static float GetPassiveSize(PassiveType passiveType, float baseSize)
        {
            if (BonusInfo.passiveBonusSizeMapByType == null) return baseSize;
            if (BonusInfo.passiveBonusSizeMapByType.TryGetValue(passiveType, out var bonus))
                return baseSize + bonus;
            return baseSize;
        }

        public static float GetPassiveValue(PassiveType passiveType, float baseValue)
        {
            if (BonusInfo.passiveBonusValueMapByType == null) return baseValue;
            if (BonusInfo.passiveBonusValueMapByType.TryGetValue(passiveType, out var bonus))
                return baseValue + BasePlayerDamageWithBonus + bonus;
            return baseValue;
        }

        public static float GetPassiveStagger(PassiveType passiveType, float baseStagger)
        {
            if (BonusInfo.passiveBonusStaggerMapByType == null) return baseStagger;
            if (BonusInfo.passiveBonusStaggerMapByType.TryGetValue(passiveType, out var bonus))
                return baseStagger + bonus;
            return baseStagger;
        }

        #endregion

        #region Move Towers

        public static float GetTeleCooldown(float baseCooldown)
        {
            return Mathf.Max((baseCooldown - BonusInfo.moveCooldownPlus) * (1f - BasePLayerCooldownWithBonus), 0f);
        }
        
        public static float GetDashCooldown(float baseCooldown)
        {
            return Mathf.Max((baseCooldown - BonusInfo.dashCooldownPlus) * (1f - BasePLayerCooldownWithBonus), 0f);
        }

        public static float GetDashSize(float baseSize)
        {
            return baseSize + BonusInfo.dashSizePlus;
        }

        public static int GetDashDamage(int baseDamage)
        {
            return (int)((1f + BonusInfo.dashDamageMultiplier) * (baseDamage + BonusInfo.dashDamagePlus + BasePlayerDamageWithBonus));
        }

        public static float GetFlashCooldown(float baseCooldown)
        {
            return Mathf.Max((baseCooldown - BonusInfo.flashCooldownPlus) * (1f - BasePLayerCooldownWithBonus), 0f);
        }

        public static float GetFlashSize(float baseSize)
        {
            return baseSize + BonusInfo.flashSizePlus;
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