using System;
using UnityEngine;

namespace InGame
{
    public class LevelUtility
    {
        public static UpgradeBonusInfo BonusInfo { get; set; } = new UpgradeBonusInfo();
        
        /// <summary>
        /// HP = [ Player_HP + Total (HP_Plus) ] * [ 1 + Total (HP_Multiple) ]
        /// </summary>
        /// <param name="baseHealth"></param>
        /// <returns></returns>
        public static int GetPlayerHealth(int baseHealth)
        {
            return Mathf.RoundToInt((baseHealth + BonusInfo.hpPlus) * (1 + BonusInfo.hpMultiply));
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
        public static (int, int) GetPlayerBulletDamage(
            int skillId,
            int playerDamage, 
            int skillDamage, 
            float criticalDameMultiplier, 
            float chargeDameMultiplier)
        {
            playerDamage = Mathf.RoundToInt((playerDamage + BonusInfo.damePlus) * (1 + BonusInfo.dameMultiply));
            var bulletDamage = Mathf.RoundToInt((playerDamage + skillDamage + BonusInfo.skillBonus.skillDamePlus) * (1 + BonusInfo.skillBonus.skillDameMultiply));
            criticalDameMultiplier = criticalDameMultiplier + BonusInfo.criticalDame;
            return (
                Mathf.RoundToInt(bulletDamage * chargeDameMultiplier), 
                Mathf.RoundToInt(bulletDamage * criticalDameMultiplier * chargeDameMultiplier)
                );
        }

        /// <summary>
        /// Tỷ lệ random = Crit_Rate_Base + Total (Crit_Rate_Plus)
        /// </summary>
        /// <param name="baseCriticalRate"></param>
        /// <returns></returns>
        public static float GetCriticalRate(float baseCriticalRate)
        {
            return baseCriticalRate + BonusInfo.criticalRatePlus;
        }

        /// <summary>
        /// Number_Of_Bullet = Bullet + Total (Bullet_Plus) + Max [ RoundDown (Charge_Time / Charge_Bullet_Interval), Max_Bullet_Add ]
        /// </summary>
        /// <param name="skillId"></param>
        /// <param name="baseBulletNum"></param>
        /// <param name="chargeBulletNum"></param>
        /// <returns></returns>
        public static int GetNumberOfBullets(int skillId, int baseBulletNum, int chargeBulletNum)
        {
            return baseBulletNum + BonusInfo.skillBonus.bulletPlus + chargeBulletNum;
        }

        /// <summary>
        /// Player_Cooldown = Base_Cooldown + Total (Cooldown_Plus)
        /// Skill_Cooldown = [Skill_Cooldown_Base - Total (Skill_Cooldown_Plus) ] * [ 1 - Total (Skill_Cooldown_Multiple) ]
        /// Final_Cooldown = Skill_Cooldown * (1 - Player_Cooldown)
        /// </summary>
        /// <param name="playerCooldown"></param>
        /// <param name="baseSkillCooldown"></param>
        /// <returns></returns>
        public static float GetSkillCooldown(int skillId, float playerCooldown, float baseSkillCooldown)
        {
            return Mathf.Max(0f, (baseSkillCooldown - BonusInfo.skillBonus.skillCooldownPlus) * (1 - BonusInfo.skillBonus.skillCooldownMultiply) * (1 - playerCooldown - BonusInfo.cooldownPlus));
        }

        /// <summary>
        /// Skill_Range = Range * [1 + Total (Skill_Range_Multiple) ] * [ 1 + ( Charge_Range_Max / Charge_Range_Time ) * Charge_Time ]
        /// </summary>
        /// <param name="baseRange"></param>
        /// <param name="chargeRange"></param>
        /// <returns></returns>
        public static float GetSkillRange(int skillId, float baseRange, float chargeRange, Vector2 direction)
        {
            // Calculate the ratio: true_range / skill_range
            var magnitude = direction.magnitude;
            direction.x = Mathf.Abs(direction.x) / magnitude;
            direction.y = Mathf.Abs(direction.y) / magnitude;
            var angle = Mathf.Atan2(direction.y, direction.x);
            var ratio = GameConst.IsoRatio
                        / Mathf.Sqrt(Mathf.Pow(GameConst.IsoRatio * Mathf.Cos(angle), 2) +
                                     Mathf.Pow(Mathf.Sin(angle), 2));
            
            return baseRange * (1 + BonusInfo.skillBonus.skillRangeMultiply) * chargeRange * ratio;
        }

        /// <summary>
        /// Skill_Size = Size * [1 + Total (Skill_Size_Multiple) ] * [ 1 + ( Charge_Size_Max / Charge_Size_Time ) * Charge_Time ]
        /// </summary>
        /// <param name="baseSize"></param>
        /// <param name="chargeSize"></param>
        /// <returns></returns>
        public static float GetSkillSize(int skillId, float baseSize, float chargeSize)
        {
            return baseSize * (1 + BonusInfo.skillBonus.skillSizeMultiply) * chargeSize;
        }

        /// <summary>
        /// Total_Stagger = Stagger * [1 + Total (Stagger_Multiple) ]
        /// </summary>
        /// <param name="baseStagger"></param>
        /// <returns></returns>
        public static float GetBulletStagger(int skillId, float baseStagger)
        {
            return baseStagger * (1 + BonusInfo.skillBonus.staggerMultiply);
        }

        public static float GetDropRate(float baseDropRate)
        {
            return (baseDropRate + BonusInfo.dropRatePlus) * (1f + BonusInfo.dropRateMultiply);
        }

        #region Charge

        /// <summary>
        /// Get the MaxDameMultiplier of the max MaxDameMultiplierAdd / MaxDameChargeTime
        /// </summary>
        /// <param name="baseChargeMaxDameMultiplier"></param>
        /// <param name="baseChargeMaxTime"></param>
        /// <returns>(MaxDame, MaxChargeTime)</returns>
        public static (float, float) GetChargeDameMax(float baseChargeMaxDameMultiplier, float baseChargeMaxTime)
        {
            var max = 0f;
            var resultMD = 0f;
            var resultMCT = 0f;
            var temp = 0f;
            if (BonusInfo.chargeBulletBonus?.maxDameChargeTime > 0)
            {
                temp = BonusInfo.chargeBulletBonus.maxDameMultiplier / BonusInfo.chargeBulletBonus.maxDameChargeTime;
                if (temp > max)
                {
                    max = temp;
                    resultMD = BonusInfo.chargeBulletBonus.maxDameMultiplier;
                    resultMCT = BonusInfo.chargeBulletBonus.maxDameChargeTime;
                }
            }

            if (BonusInfo.chargeSizeBonus?.maxDameChargeTime > 0)
            {
                temp = BonusInfo.chargeSizeBonus.maxDameMultiplier / BonusInfo.chargeSizeBonus.maxDameChargeTime;
                if (temp > max)
                {
                    max = temp;
                    resultMD = BonusInfo.chargeSizeBonus.maxDameMultiplier;
                    resultMCT = BonusInfo.chargeSizeBonus.maxDameChargeTime;
                }
            }

            if (BonusInfo.chargeRangeBonus?.maxDameChargeTime > 0)
            {
                temp = BonusInfo.chargeRangeBonus.maxDameMultiplier / BonusInfo.chargeRangeBonus.maxDameChargeTime;
                if (temp > max)
                {
                    max = temp;
                    resultMD = BonusInfo.chargeRangeBonus.maxDameMultiplier;
                    resultMCT = BonusInfo.chargeRangeBonus.maxDameChargeTime;
                }
            }

            if (BonusInfo.chargeDameBonus?.maxDameChargeTime > 0)
            {
                temp = BonusInfo.chargeDameBonus.maxDameMultiplier / BonusInfo.chargeDameBonus.maxDameChargeTime;
                if (temp > max)
                {
                    max = temp;
                    resultMD = BonusInfo.chargeDameBonus.maxDameMultiplier;
                    resultMCT = BonusInfo.chargeDameBonus.maxDameChargeTime;
                }
            }

            return (baseChargeMaxDameMultiplier + resultMD, baseChargeMaxTime + resultMCT);
        }

        public static (float, float) GetChargeSizeMax(float baseChargeSize, float baseChargeSizeTime)
        {
            var max = 0f;
            var resultMD = 0f;
            var resultMCT = 0f;
            var temp = 0f;
            if (BonusInfo.chargeBulletBonus?.maxSizeChargeTime > 0)
            {
                temp = BonusInfo.chargeBulletBonus.maxSizeMultiplier / BonusInfo.chargeBulletBonus.maxSizeChargeTime;
                if (temp > max)
                {
                    max = temp;
                    resultMD = BonusInfo.chargeBulletBonus.maxSizeMultiplier;
                    resultMCT = BonusInfo.chargeBulletBonus.maxSizeChargeTime;
                }
            }

            if (BonusInfo.chargeRangeBonus?.maxSizeChargeTime > 0)
            {
                temp = BonusInfo.chargeRangeBonus.maxSizeMultiplier / BonusInfo.chargeRangeBonus.maxSizeChargeTime;
                if (temp > max)
                {
                    max = temp;
                    resultMD = BonusInfo.chargeRangeBonus.maxSizeMultiplier;
                    resultMCT = BonusInfo.chargeRangeBonus.maxSizeChargeTime;
                }
            }

            if (BonusInfo.chargeDameBonus?.maxSizeChargeTime > 0)
            {
                temp = BonusInfo.chargeDameBonus.maxSizeMultiplier / BonusInfo.chargeDameBonus.maxSizeChargeTime;
                if (temp > max)
                {
                    max = temp;
                    resultMD = BonusInfo.chargeDameBonus.maxSizeMultiplier;
                    resultMCT = BonusInfo.chargeDameBonus.maxSizeChargeTime;
                }
            }
            
            if (BonusInfo.chargeSizeBonus?.maxDameChargeTime > 0)
            {
                temp = BonusInfo.chargeSizeBonus.maxSizeMultiplier / BonusInfo.chargeSizeBonus.maxSizeChargeTime;
                if (temp > max)
                {
                    max = temp;
                    resultMD = BonusInfo.chargeSizeBonus.maxSizeMultiplier;
                    resultMCT = BonusInfo.chargeSizeBonus.maxSizeChargeTime;
                }
            }

            return (baseChargeSize + resultMD, baseChargeSizeTime + resultMCT);
        }
        
        public static (int, float) GetChargeBulletMax(int baseChargeBullet, float baseChargeBulletInterval)
        {
            var max = 0;
            var resultBullet = 0;
            var resultInterval = 0f;
            if (BonusInfo.chargeBulletBonus?.maxBulletAdd > max)
            {
                max = BonusInfo.chargeBulletBonus.maxBulletAdd;
                resultBullet = BonusInfo.chargeBulletBonus.maxBulletAdd;
                resultInterval = BonusInfo.chargeBulletBonus.bulletAddInterval;
            }

            if (BonusInfo.chargeDameBonus?.maxBulletAdd > max)
            {
                max = BonusInfo.chargeDameBonus.maxBulletAdd;
                resultBullet = BonusInfo.chargeDameBonus.maxBulletAdd;
                resultInterval = BonusInfo.chargeDameBonus.bulletAddInterval;
            }

            if (BonusInfo.chargeRangeBonus?.maxBulletAdd > max)
            {
                max = BonusInfo.chargeRangeBonus.maxBulletAdd;
                resultBullet = BonusInfo.chargeRangeBonus.maxBulletAdd;
                resultInterval = BonusInfo.chargeRangeBonus.bulletAddInterval;
            }
            
            if (BonusInfo.chargeSizeBonus?.maxBulletAdd > max)
            {
                max = BonusInfo.chargeSizeBonus.maxBulletAdd;
                resultBullet = BonusInfo.chargeSizeBonus.maxBulletAdd;
                resultInterval = BonusInfo.chargeSizeBonus.bulletAddInterval;
            }

            return (baseChargeBullet + resultBullet, baseChargeBulletInterval + resultInterval);
        }
        
        public static (float, float) GetChargeRangeMax(float baseChargeRange, float baseChargeRangeTime)
        {
            var max = 0f;
            var resultMR = 0f;
            var resultMRT = 0f;
            var temp = 0f;
            if (BonusInfo.chargeBulletBonus?.maxRangeChargeTime > 0)
            {
                temp = BonusInfo.chargeBulletBonus.maxRangeMultiplier / BonusInfo.chargeBulletBonus.maxRangeChargeTime;
                if (temp > max)
                {
                    max = temp;
                    resultMR = BonusInfo.chargeBulletBonus.maxRangeMultiplier;
                    resultMRT = BonusInfo.chargeBulletBonus.maxRangeChargeTime;
                }
            }

            if (BonusInfo.chargeRangeBonus?.maxRangeChargeTime > 0)
            {
                temp = BonusInfo.chargeRangeBonus.maxRangeMultiplier / BonusInfo.chargeRangeBonus.maxRangeChargeTime;
                if (temp > max)
                {
                    max = temp;
                    resultMR = BonusInfo.chargeRangeBonus.maxRangeMultiplier;
                    resultMRT = BonusInfo.chargeRangeBonus.maxRangeChargeTime;
                }
            }

            if (BonusInfo.chargeDameBonus?.maxRangeChargeTime > 0)
            {
                temp = BonusInfo.chargeDameBonus.maxRangeMultiplier / BonusInfo.chargeDameBonus.maxRangeChargeTime;
                if (temp > max)
                {
                    max = temp;
                    resultMR = BonusInfo.chargeDameBonus.maxRangeMultiplier;
                    resultMRT = BonusInfo.chargeDameBonus.maxRangeChargeTime;
                }
            }
            
            if (BonusInfo.chargeSizeBonus?.maxRangeChargeTime > 0)
            {
                temp = BonusInfo.chargeSizeBonus.maxRangeMultiplier / BonusInfo.chargeSizeBonus.maxRangeChargeTime;
                if (temp > max)
                {
                    max = temp;
                    resultMR = BonusInfo.chargeSizeBonus.maxRangeMultiplier;
                    resultMRT = BonusInfo.chargeSizeBonus.maxRangeChargeTime;
                }
            }

            return (baseChargeRange + resultMR, baseChargeRangeTime + resultMRT);
        }
        
        #endregion
        
        #region Passive

        public static float GetPassiveCooldown(PassiveType passiveType, float baseCooldown)
        {
            if (BonusInfo.passiveBonusCooldownMapByType == null) return baseCooldown;
            if (BonusInfo.passiveBonusCooldownMapByType.TryGetValue(passiveType, out var bonus))
                return Mathf.Max(baseCooldown - bonus, 0f);
            return baseCooldown;
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
                return baseValue + bonus;
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
            return baseCooldown - BonusInfo.moveCooldownPlus;
        }
        
        public static float GetDashCooldown(float baseCooldown)
        {
            return baseCooldown - BonusInfo.dashCooldownPlus;
        }

        public static float GetDashSize(float baseSize)
        {
            return baseSize + BonusInfo.dashSizePlus;
        }

        public static int GetDashDamage(int baseDamage)
        {
            return (int)((1f + BonusInfo.dashDamageMultiplier) * (baseDamage + BonusInfo.dashDamagePlus));
        }

        public static float GetFlashCooldown(float baseCooldown)
        {
            return baseCooldown - BonusInfo.flashCooldownPlus;
        }

        public static float GetFlashSize(float baseSize)
        {
            return baseSize + BonusInfo.flashSizePlus;
        }

        public static int GetFlashDamage(int baseDamage)
        {
            return (int)((1f + BonusInfo.flashDamageMultiplier) * (baseDamage + BonusInfo.flashDamagePlus));
        }

        #endregion

        #region Tower

        public static int GetTowerHp(int baseHp)
        {
            return (int)((1f + BonusInfo.hpMultiply) * (baseHp + BonusInfo.hpPlus));
        }

        public static int GetTowerCounterDamage(int baseDamage)
        {
            return baseDamage + BonusInfo.towerCounterDamagePlus;
        }

        public static float GetTowerCounterCooldown(float baseCooldown)
        {
            return Mathf.Max(baseCooldown - BonusInfo.towerCounterCooldownPlus, 0f);
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