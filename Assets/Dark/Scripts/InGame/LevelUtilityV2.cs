using System;
using System.Collections.Generic;
using InGame.AttackNormalConfig;
using InGame.ChargeConfig;
using InGame.CounterConfig;
using UnityEngine;

namespace InGame
{
    public class LevelUtilityV2
    {
        public static UpgradeBonusInfoV2 BonusInfo { get; set; } = new UpgradeBonusInfoV2();
        public static PlayerStats StatsBase { get; set; }
        public static PlayerSkillConfig StatsNormalAttack { get; set; }
        public static MoveTowersConfig StatsTele { get; set; }
        
        public static PlayerSkillNormalConfig StatsNormalPiercing { get; set; }
        public static PlayerSkillNormalConfig StatsNormalBullet { get; set; }
        public static PlayerChargeConfig StatsChargeBullet { get; set; }
        public static PlayerChargeConfig StatsChargeSize { get; set; }
        public static MoveTowersConfig StatsFlash { get; set; }
        public static MoveTowersConfig StatsDash { get; set; }
        public static TowerCounterConfig StatsCounterPiercing { get; set; }
        public static TowerCounterConfig StatsCounterSlash { get; set; }
        public static Dictionary<PassiveTriggerType, Dictionary<PassiveType, PassiveConfig>> PassiveConfigsMap { get; set; }

        public static int ToInt(float value) => Mathf.CeilToInt(value);
            
        #region Base

        /// <summary>
        /// HP = [ Player_HP + Total (HP_Plus) ] * [ 1 + Total (HP_Multiple) ]
        /// </summary>
        /// <returns></returns>
        public static int GetBaseTowerHp()
        {
            return ToInt((1f + BonusInfo.bonusBase.bonusHp.mul) * (StatsBase.hp + BonusInfo.bonusBase.bonusHp.addInt) );
        }

        // Similar to Hp
        public static int GetBaseTowerShield()
        {
            return ToInt((1f + BonusInfo.bonusBase.bonusShield.mul) * (StatsBase.armor + BonusInfo.bonusBase.bonusShield.addInt));
        }

        public static int GetBaseDmg()
        {
            return StatsBase.damageBase + BonusInfo.bonusBase.bonusDmg.addInt;
        }

        public static float GetBaseDmgRate()
        {
            return StatsBase.damageRate + BonusInfo.bonusBase.bonusDmg.mul;
        }
        
        public static float GetBaseCooldown()
        {
            return StatsBase.cooldown + BonusInfo.bonusBase.bonusCooldown.mul;
        }

        // Similar to hp
        public static int GetBaseRegen()
        {
            return ToInt((1f + BonusInfo.bonusBase.bonusRegen.mul) * (StatsBase.regen + BonusInfo.bonusBase.bonusRegen.addInt));
        }
        
        // Similar to hp
        public static float GetBaseLifeLeech()
        {
            return (StatsBase.lifeLeech + BonusInfo.bonusBase.bonusLifeLeech.add) * (1f + BonusInfo.bonusBase.bonusLifeLeech.mul);
        }
        
        // Similar to hp
        public static float GetBaseCriticalDmgScale()
        {
            return (StatsBase.criticalDamage + BonusInfo.bonusBase.bonusCritDmg.add) * (1f + BonusInfo.bonusBase.bonusCritDmg.mul);
        }
        
        // Similar to hp
        public static float GetBaseCriticalRate()
        {
            return (StatsBase.criticalRate + BonusInfo.bonusBase.bonusCritRate.add) * (1f + BonusInfo.bonusBase.bonusCritRate.mul);
        }
        
        // Similar to hp
        public static float GetBaseStagger()
        {
            return (StatsBase.stagger + BonusInfo.bonusBase.bonusStagger.add) * (1f + BonusInfo.bonusBase.bonusStagger.mul);
        }
        
        // Similar to hp
        public static float GetBossScaleDamage()
        {
            return (StatsBase.bossDamageScale + BonusInfo.bonusBase.bonusDmgBoss.add) * (1f + BonusInfo.bonusBase.bonusDmgBoss.mul);
        }
        
        // Similar to hp
        public static float GetVestigeDropScale()
        {
            return (1f + BonusInfo.bonusBase.bonusVestigeDrop.mul);
        }
        
        // Similar to hp
        public static float GetVestigeDoubleChance()
        {
            return (0f + BonusInfo.bonusBase.bonusVestigeDoubleChance.add) * (1f + BonusInfo.bonusBase.bonusVestigeDoubleChance.mul);
        }
        
        // Similar to hp
        public static float GetVestigeTripleChance()
        {
            return (0f + BonusInfo.bonusBase.bonusVestigeTripleChance.add) * (1f + BonusInfo.bonusBase.bonusVestigeTripleChance.mul);
        }
        
        // Similar to hp
        public static float GetVestigeCollectSize()
        {
            return (StatsBase.vestigeCollectSize + BonusInfo.bonusBase.bonusVestigeCollectSize.add) * (1f + BonusInfo.bonusBase.bonusVestigeCollectSize.mul);
        }
        
        // Similar to hp
        public static float GetExpDropScale()
        {
            return (1f + BonusInfo.bonusBase.bonusExpDrop.mul);
        }
        
        #endregion

        #region Normal Attack

        /// <summary>
        /// Player_Damage = Base_Damage + Total (Dame_Plus)
        /// Player_Damage_Mul = 1 + Total (Dame_Multiple)
        /// Bullet_Dame = [ Player_Damage + Dame_Per_Bullet + Total (Skill_Dame_Plus) ] * [ 1 + Player_Damage_Mul ] * [ 1 + Total (Skill_Dame_Multiple) ]
        /// 
        /// Crit_Dame_Multiplier = Crit_Dame_Base + Total (Crit_Dame)
        /// 
        /// Crit: Bullet_Damage_Dealt = Bullet_Dame * [ Crit_Dame_Base + Total (Crit_Dame) ] * [1 + ( Charge_Dame_Max / Charge_Dame_Time ) * Charge_Time ]
        /// Non-Crit: Bullet_Damage_Dealt = Bullet_Dame * [1 + ( Charge_Dame_Max / Charge_Dame_Time ) * Charge_Time ]
        /// </summary>
        /// <returns></returns>
        public static (int, int) GetNormalAttackDamage()
        {
            var bulletDamage = ToInt(
                (1f + BonusInfo.bonusNormalAttack.bonusNormalAttackDmg.mul + GetBaseDmgRate())
                * (StatsNormalAttack.damePerBullet + BonusInfo.bonusNormalAttack.bonusNormalAttackDmg.addInt + GetBaseDmg()));
            return LevelTemporaryUtility.FilterPlayerBulletDamage(
                ToInt(bulletDamage),
                ToInt(GetBaseCriticalDmgScale() * bulletDamage), 
                BonusInfo);
        }

        /// <summary>
        /// Skill_Cooldown = [Skill_Cooldown_Base - Total (Skill_Cooldown_Plus) ] * [ 1 - Total (Skill_Cooldown_Multiple) ]
        /// Final_Cooldown = Skill_Cooldown * (1 - Player_Base_Cooldown)
        /// </summary>
        /// <returns></returns>
        public static float GetNormalAttackCooldown()
        {
            var baseCooldown = StatsNormalAttack.cooldown;
            return LevelTemporaryUtility.FilterSkillCooldown(Mathf.Max(0.001f,
                (baseCooldown - BonusInfo.bonusNormalAttack.bonusNormalAttackCooldown.add) 
                    * Mathf.Clamp01(1 - BonusInfo.bonusNormalAttack.bonusNormalAttackCooldown.mul - GetBaseCooldown())), 
                BonusInfo);
        }

        /// <summary>
        /// Skill_Range = Range * [1 + Total (Skill_Range_Multiple) ] * [ 1 + ( Charge_Range_Max / Charge_Range_Time ) * Charge_Time ]
        /// </summary>
        /// <param name="baseRange"></param>
        /// <param name="chargeRange"></param>
        /// <returns></returns>
        public static float GetNormalAttackRange(Vector2 direction)
        {
            // Calculate the ratio: true_range / skill_range
            var magnitude = direction.magnitude;
            direction.x = Mathf.Abs(direction.x) / magnitude;
            direction.y = Mathf.Abs(direction.y) / magnitude;
            var angle = Mathf.Atan2(direction.y, direction.x);
            var ratio = GameConst.IsoRatio
                        / Mathf.Sqrt(Mathf.Pow(GameConst.IsoRatio * Mathf.Cos(angle), 2) +
                                     Mathf.Pow(Mathf.Sin(angle), 2));
            
            return (StatsNormalAttack.range + BonusInfo.bonusNormalAttack.bonusNormalAttackRange.add) * (1 + BonusInfo.bonusNormalAttack.bonusNormalAttackRange.mul) * ratio;
        }
        
        /// <summary>
        /// Skill_Size = Size * [1 + Total (Skill_Size_Multiple) ] * [ 1 + ( Charge_Size_Max / Charge_Size_Time ) * Charge_Time ]
        /// </summary>
        /// <param name="baseSize"></param>
        /// <param name="chargeSize"></param>
        /// <returns></returns>
        public static float GetNormalAttackSize()
        {
            return (StatsNormalAttack.size + BonusInfo.bonusNormalAttack.bonusNormalAttackSize.add) * (1 + BonusInfo.bonusNormalAttack.bonusNormalAttackSize.mul);
        }

        public static float GetNormalPiercingDamageScale()
        {
            return (StatsNormalPiercing.dmgScale + BonusInfo.bonusNormalAttack.bonusPiercingDmg.add) *
                        (1f + BonusInfo.bonusNormalAttack.bonusPiercingDmg.mul);
        }

        public static int GetNormalPiercingAmount()
        {
            return Mathf.RoundToInt((StatsNormalPiercing.amount + BonusInfo.bonusNormalAttack.bonusPiercingAmount.addInt) * (1f + BonusInfo.bonusNormalAttack.bonusPiercingAmount.mul));
        }

        public static float GetNormalBulletDamageScale()
        {
            return (StatsNormalBullet.dmgScale + BonusInfo.bonusNormalAttack.bonusBulletDmg.add) *
                        (1f + BonusInfo.bonusNormalAttack.bonusBulletDmg.mul);
        }

        public static int GetNormalBulletAmount()
        {
            return Mathf.RoundToInt((StatsNormalBullet.amount + BonusInfo.bonusNormalAttack.bonusBulletAmount.addInt) * (1f + BonusInfo.bonusNormalAttack.bonusBulletAmount.mul));
        }
        
        #endregion

        #region Charge Attack

        /// <summary>
        /// Skill_Cooldown = [Skill_Cooldown_Base - Total (Skill_Cooldown_Plus) ] * [ 1 - Total (Skill_Cooldown_Multiple) ]
        /// Final_Cooldown = Skill_Cooldown * (1 - Player_Base_Cooldown)
        /// </summary>
        /// <returns></returns>
        public static float GetChargeAttackCooldown()
        {
            var baseCooldown = StatsNormalAttack.chargeCooldown;
            return LevelTemporaryUtility.FilterSkillCooldown(Mathf.Max(0.001f,
                    (baseCooldown - BonusInfo.bonusChargeAttack.bonusChargeCooldown.add) 
                    * Mathf.Clamp01(1 - BonusInfo.bonusChargeAttack.bonusChargeCooldown.mul - GetBaseCooldown())), 
                BonusInfo);
        }
        
        // Should not be less than 0.1f, too small
        public static float GetChargeStepTime()
        {
            return Mathf.Max((StatsNormalAttack.chargeStepTime - BonusInfo.bonusChargeAttack.bonusChargeTime.add) * (1f - Mathf.Clamp(BonusInfo.bonusChargeAttack.bonusChargeTime.mul, 0f, 1f)), 0.1f);
        }

        /// <summary>
        /// Player_Damage = Base_Damage + Total (Dame_Plus)
        /// Player_Damage_Mul = 1 + Total (Dame_Multiple)
        /// Bullet_Dame = [ Player_Damage + Dame_Per_Bullet + Total (Skill_Dame_Plus) ] * [ 1 + Player_Damage_Mul ] * [ 1 + Total (Skill_Dame_Multiple) ]
        /// 
        /// Crit_Dame_Multiplier = Crit_Dame_Base + Total (Crit_Dame)
        /// 
        /// Crit: Bullet_Damage_Dealt = Bullet_Dame * [ Crit_Dame_Base + Total (Crit_Dame) ] * [1 + ( Charge_Dame_Max / Charge_Dame_Time ) * Charge_Time ]
        /// Non-Crit: Bullet_Damage_Dealt = Bullet_Dame * [1 + ( Charge_Dame_Max / Charge_Dame_Time ) * Charge_Time ]
        /// </summary>
        /// <returns></returns>
        public static (int, int) GetChargeAttackDamage(float chargeDameMultiplier)
        {
            var bulletDamage = ToInt(
                (1f + BonusInfo.bonusChargeAttack.bonusChargeDmg.mul + GetBaseDmgRate())
                * (StatsNormalAttack.damePerBullet + BonusInfo.bonusChargeAttack.bonusChargeDmg.addInt + GetBaseDmg()));
            return LevelTemporaryUtility.FilterPlayerBulletDamage(
                ToInt(bulletDamage * chargeDameMultiplier),
                ToInt(bulletDamage * chargeDameMultiplier * GetBaseCriticalDmgScale()), 
                BonusInfo);
        }

        // Charge bullet max step
        public static int GetChargeBulletAmount()
        {
            return ToInt((StatsChargeBullet.value + BonusInfo.bonusChargeAttack.bonusBulletAmount.addInt) * (1f + BonusInfo.bonusChargeAttack.bonusBulletAmount.mul));
        }
        
        // Charge size max bullet blossom
        public static int GetChargeSizeAmount()
        {
            return ToInt((1f + BonusInfo.bonusChargeAttack.bonusSizeAmount.mul) * (StatsChargeSize.value + BonusInfo.bonusChargeAttack.bonusSizeAmount.addInt) );
        }
        
        // Charge range step
        public static float GetChargeRangeStep()
        {
            return (StatsNormalAttack.chargeRangeStep + BonusInfo.bonusChargeAttack.bonusRangeStep.add) *
                   (1f + BonusInfo.bonusChargeAttack.bonusRangeStep.mul);
        }

        #endregion

        #region Move Attack

        public static float GetTeleCooldown()
        {
            return Mathf.Max((StatsTele.cooldown - BonusInfo.bonusMove.bonusMoveCooldown.add) * Mathf.Clamp01(1f - BonusInfo.bonusMove.bonusMoveCooldown.mul - GetBaseCooldown()), 0f);
        }
        
        public static int GetFlashDamage()
        {
            return ToInt((1f + BonusInfo.bonusMove.bonusMoveDmg.mul + GetBaseDmgRate()) * (StatsFlash.damage + BonusInfo.bonusMove.bonusMoveDmg.addInt + GetBaseDmg()));
        }

        public static int GetDashDamage()
        {
            return ToInt((1f + BonusInfo.bonusMove.bonusMoveDmg.mul + GetBaseDmgRate()) * (StatsDash.damage + BonusInfo.bonusMove.bonusMoveDmg.addInt + GetBaseDmg()));
        }
        
        public static float GetFlashCooldown()
        {
            return Mathf.Max((StatsFlash.cooldown - BonusInfo.bonusMove.bonusMoveCooldown.add) * Mathf.Clamp01(1f - BonusInfo.bonusMove.bonusMoveCooldown.mul - GetBaseCooldown()), 0f);
        }

        public static float GetDashCooldown()
        {
            return Mathf.Max((StatsDash.cooldown - BonusInfo.bonusMove.bonusMoveCooldown.add) * Mathf.Clamp01(1f - BonusInfo.bonusMove.bonusMoveCooldown.mul - GetBaseCooldown()), 0f);
        }

        public static float GetFlashSize()
        {
            return (StatsFlash.size + BonusInfo.bonusMove.bonusFlashSize.add) * (1f + BonusInfo.bonusMove.bonusFlashSize.mul);
        }
        
        public static float GetDashSize()
        {
            return (StatsDash.size + BonusInfo.bonusMove.bonusDashSize.add) * (1f + BonusInfo.bonusMove.bonusDashSize.mul);
        }

        #endregion

        #region Counter Attack

        public static int GetCounterPiercingDamage()
        {
            return ToInt(
                (1f + BonusInfo.bonusCounter.bonusCounterDmg.mul + GetBaseDmgRate()) *
                (StatsCounterPiercing.damage + BonusInfo.bonusCounter.bonusCounterDmg.addInt + GetBaseDmg()));
        }
        
        public static int GetCounterSlashDamage()
        {
            return ToInt(
                (1f + BonusInfo.bonusCounter.bonusCounterDmg.mul + GetBaseDmgRate()) * 
                (StatsCounterSlash.damage + BonusInfo.bonusCounter.bonusCounterDmg.addInt + GetBaseDmg()));
        }

        public static float GetCounterPiercingCooldown()
        {
            return Mathf.Max(
                (StatsCounterPiercing.cooldown - BonusInfo.bonusCounter.bonusCounterCooldown.add) *
                Mathf.Clamp01(1f - BonusInfo.bonusCounter.bonusCounterCooldown.mul - GetBaseCooldown()), 0.001f);
        }
        
        public static float GetCounterSlashCooldown()
        {
            return Mathf.Max(
                (StatsCounterSlash.cooldown - BonusInfo.bonusCounter.bonusCounterCooldown.add) *
                Mathf.Clamp01(1f - BonusInfo.bonusCounter.bonusCounterCooldown.mul - GetBaseCooldown()), 0.001f);
        }

        public static int GetCounterPiercingSize()
        {
            return Mathf.RoundToInt((StatsCounterPiercing.size + BonusInfo.bonusCounter.bonusPiercingAmount.addInt) * (1f + BonusInfo.bonusCounter.bonusPiercingAmount.mul));
        }

        public static float GetCounterSlashRange()
        {
            return (StatsCounterSlash.range + BonusInfo.bonusCounter.bonusSlashSize.add) * (1f + BonusInfo.bonusCounter.bonusSlashSize.mul);
        }

        public static float GetCounterPiercingDetectRange()
        {
            return StatsCounterPiercing.detectRange;
        }

        public static float GetCounterSlashDetectRange()
        {
            return StatsCounterSlash.detectRange;
        }

        public static float GetCounterPiercingDelayAfterDetected()
        {
            return StatsCounterPiercing.delayOnDetectedEnemy;
        }

        public static float GetCounterSlashDelayAfterDetected()
        {
            return StatsCounterSlash.delayOnDetectedEnemy;
        }

        #endregion

        #region Math

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
        
        public static float GetRelativeRangeMove(float maxRange, Vector2 direction)
        {
            var magnitude = direction.magnitude;
            direction.x = Mathf.Abs(direction.x) / magnitude;
            direction.y = Mathf.Abs(direction.y) / magnitude;
            var angle = Mathf.Atan2(direction.y, direction.x);
            var ratio = GameConst.IsoRatioMove
                        / Mathf.Sqrt(Mathf.Pow(GameConst.IsoRatioMove * Mathf.Cos(angle), 2) +
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

        #endregion

        #region Passive

        public static float GetPassiveCooldown(PassiveTriggerType triggerType, PassiveType passiveType)
        {
            var baseCooldown = 1f;
            if (PassiveConfigsMap.TryGetValue(triggerType, out var triggerDict) && triggerDict.TryGetValue(passiveType, out var config))
                baseCooldown = config.cooldown;
            return Mathf.Max(baseCooldown * (1f - GetBaseCooldown()), 0f);
        }

        public static float GetPassiveChance(PassiveTriggerType triggerType, PassiveType passiveType)
        {
            var baseChance = 0f;
            if (PassiveConfigsMap.TryGetValue(triggerType, out var triggerDict) && triggerDict.TryGetValue(passiveType, out var config))
                baseChance = config.chance;
            var bonus = GetPassiveRateBonus(triggerType, passiveType);
            return Mathf.Min((baseChance + bonus.add) * (1f + bonus.mul), 1f);
        }
        
        public static float GetPassiveSize(PassiveTriggerType triggerType, PassiveType passiveType)
        {
            var baseSize = 0f;
            if (PassiveConfigsMap.TryGetValue(triggerType, out var triggerDict) && triggerDict.TryGetValue(passiveType, out var config))
                baseSize = config.size;
            var bonus = (passiveType) switch
            {
                PassiveType.Explosion => GetPassiveExplosiveSizeBonus(triggerType),
                PassiveType.Lightning => GetPassiveLightningAmountBonus(triggerType),
                PassiveType.Burning => GetPassiveBurningDurationBonus(triggerType),
                PassiveType.Thunder => GetPassiveThunderExecutionChanceBonus(triggerType),
                _ => new UpgradeBonusStatV2()
            };
            
            return (baseSize + bonus.add) * (1f + bonus.mul);
        }

        public static float GetPassiveValue(PassiveTriggerType triggerType, PassiveType passiveType)
        {
            var baseValue = 0f;
            if (PassiveConfigsMap.TryGetValue(triggerType, out var triggerDict) && triggerDict.TryGetValue(passiveType, out var config))
                baseValue = config.value;
            baseValue += GetBaseDmg();
            var bonus = GetPassiveDamageBonus(triggerType, passiveType);
            baseValue += bonus.add;
            return baseValue * (1f + bonus.mul + GetBaseDmgRate());
        }

        public static float GetPassiveStagger(PassiveTriggerType triggerType, PassiveType passiveType)
        {
            var baseStagger = 0f;
            if (PassiveConfigsMap.TryGetValue(triggerType, out var triggerDict) && triggerDict.TryGetValue(passiveType, out var config))
                baseStagger = config.stagger;
            return baseStagger;
        }

        public static bool IsUnlockedPassive(PassiveTriggerType triggerType, PassiveType passiveType)
        {
            switch (triggerType)
            {
                case PassiveTriggerType.DameByNormalAttack:
                    switch (passiveType)
                    {
                        case PassiveType.Lightning:
                            return BonusInfo.bonusUnlockSkill.unlockPassiveNormalLightning;
                        case PassiveType.Explosion:
                            return BonusInfo.bonusUnlockSkill.unlockPassiveNormalExplosive;
                        case PassiveType.Burning:
                            return BonusInfo.bonusUnlockSkill.unlockPassiveNormalBurning;
                        case PassiveType.Thunder:
                            return BonusInfo.bonusUnlockSkill.unlockPassiveNormalThunder;
                    }
                    break;
                case PassiveTriggerType.DameByChargeAttack:
                    switch (passiveType)
                    {
                        case PassiveType.Lightning:
                            return BonusInfo.bonusUnlockSkill.unlockPassiveChargeLightning;
                        case PassiveType.Explosion:                                       
                            return BonusInfo.bonusUnlockSkill.unlockPassiveChargeExplosive;
                        case PassiveType.Burning:                                         
                            return BonusInfo.bonusUnlockSkill.unlockPassiveChargeBurning;
                        case PassiveType.Thunder:                                         
                            return BonusInfo.bonusUnlockSkill.unlockPassiveChargeThunder;
                    }
                    break;
                case PassiveTriggerType.DameByMoveSKill:
                    switch (passiveType)
                    {
                        case PassiveType.Lightning:
                            return BonusInfo.bonusUnlockSkill.unlockPassiveMoveLightning;
                        case PassiveType.Explosion:                                       
                            return BonusInfo.bonusUnlockSkill.unlockPassiveMoveExplosive;
                        case PassiveType.Burning:                                         
                            return BonusInfo.bonusUnlockSkill.unlockPassiveMoveBurning;
                        case PassiveType.Thunder:                                         
                            return BonusInfo.bonusUnlockSkill.unlockPassiveMoveThunder;
                    }
                    break;
                case PassiveTriggerType.TowerTakeDame:
                    switch (passiveType)
                    {
                        case PassiveType.Lightning:
                            return BonusInfo.bonusUnlockSkill.unlockPassiveCounterLightning;
                        case PassiveType.Explosion:                                       
                            return BonusInfo.bonusUnlockSkill.unlockPassiveCounterExplosive;
                        case PassiveType.Burning:                                         
                            return BonusInfo.bonusUnlockSkill.unlockPassiveCounterBurning;
                        case PassiveType.Thunder:                                         
                            return BonusInfo.bonusUnlockSkill.unlockPassiveCounterThunder;
                    }
                    break;
            }

            return false;
        }
        
        private static UpgradeBonusStatV2 GetPassiveDamageBonus(PassiveTriggerType triggerType, PassiveType passiveType)
        {
            switch (triggerType)
            {
                case PassiveTriggerType.DameByNormalAttack:
                    switch (passiveType)
                    {
                        case PassiveType.Lightning:
                            return BonusInfo.bonusPassiveNormalAttack.bonusLightningDmg;
                        case PassiveType.Explosion:
                            return BonusInfo.bonusPassiveNormalAttack.bonusExplosiveDmg;
                        case PassiveType.Burning:
                            return BonusInfo.bonusPassiveNormalAttack.bonusBurningDmg;
                        case PassiveType.Thunder:
                            return BonusInfo.bonusPassiveNormalAttack.bonusThunderDmg;
                    }
                    break;
                case PassiveTriggerType.DameByChargeAttack:
                    switch (passiveType)
                    {
                        case PassiveType.Lightning:
                            return BonusInfo.bonusPassiveChargeAttack.bonusLightningDmg;
                        case PassiveType.Explosion:
                            return BonusInfo.bonusPassiveChargeAttack.bonusExplosiveDmg;
                        case PassiveType.Burning:
                            return BonusInfo.bonusPassiveChargeAttack.bonusBurningDmg;
                        case PassiveType.Thunder:
                            return BonusInfo.bonusPassiveChargeAttack.bonusThunderDmg;
                    }
                    break;
                case PassiveTriggerType.DameByMoveSKill:
                    switch (passiveType)
                    {
                        case PassiveType.Lightning:
                            return BonusInfo.bonusPassiveMove.bonusLightningDmg;
                        case PassiveType.Explosion:
                            return BonusInfo.bonusPassiveMove.bonusExplosiveDmg;
                        case PassiveType.Burning:
                            return BonusInfo.bonusPassiveMove.bonusBurningDmg;
                        case PassiveType.Thunder:
                            return BonusInfo.bonusPassiveMove.bonusThunderDmg;
                    }
                    break;
                case PassiveTriggerType.TowerTakeDame:
                    switch (passiveType)
                    {
                        case PassiveType.Lightning:
                            return BonusInfo.bonusPassiveCounter.bonusLightningDmg;
                        case PassiveType.Explosion:
                            return BonusInfo.bonusPassiveCounter.bonusExplosiveDmg;
                        case PassiveType.Burning:
                            return BonusInfo.bonusPassiveCounter.bonusBurningDmg;
                        case PassiveType.Thunder:
                            return BonusInfo.bonusPassiveCounter.bonusThunderDmg;
                    }
                    break;
            }

            return new UpgradeBonusStatV2();
        }
         
        private static UpgradeBonusStatV2 GetPassiveRateBonus(PassiveTriggerType triggerType, PassiveType passiveType)
        {
            switch (triggerType)
            {
                case PassiveTriggerType.DameByNormalAttack:
                    switch (passiveType)
                    {
                        case PassiveType.Lightning:
                            return BonusInfo.bonusPassiveNormalAttack.bonusLightningRate;
                        case PassiveType.Explosion:
                            return BonusInfo.bonusPassiveNormalAttack.bonusExplosiveRate;
                        case PassiveType.Burning:
                            return BonusInfo.bonusPassiveNormalAttack.bonusBurningRate;
                        case PassiveType.Thunder:
                            return BonusInfo.bonusPassiveNormalAttack.bonusThunderRate;
                    }
                    break;
                case PassiveTriggerType.DameByChargeAttack:
                    switch (passiveType)
                    {
                        case PassiveType.Lightning:
                            return BonusInfo.bonusPassiveChargeAttack.bonusLightningRate;
                        case PassiveType.Explosion:
                            return BonusInfo.bonusPassiveChargeAttack.bonusExplosiveRate;
                        case PassiveType.Burning:
                            return BonusInfo.bonusPassiveChargeAttack.bonusBurningRate;
                        case PassiveType.Thunder:
                            return BonusInfo.bonusPassiveChargeAttack.bonusThunderRate;
                    }
                    break;
                case PassiveTriggerType.DameByMoveSKill:
                    switch (passiveType)
                    {
                        case PassiveType.Lightning:
                            return BonusInfo.bonusPassiveMove.bonusLightningRate;
                        case PassiveType.Explosion:
                            return BonusInfo.bonusPassiveMove.bonusExplosiveRate;
                        case PassiveType.Burning:
                            return BonusInfo.bonusPassiveMove.bonusBurningRate;
                        case PassiveType.Thunder:
                            return BonusInfo.bonusPassiveMove.bonusThunderRate;
                    }
                    break;
                case PassiveTriggerType.TowerTakeDame:
                    switch (passiveType)
                    {
                        case PassiveType.Lightning:
                            return BonusInfo.bonusPassiveCounter.bonusLightningRate;
                        case PassiveType.Explosion:
                            return BonusInfo.bonusPassiveCounter.bonusExplosiveRate;
                        case PassiveType.Burning:
                            return BonusInfo.bonusPassiveCounter.bonusBurningRate;
                        case PassiveType.Thunder:
                            return BonusInfo.bonusPassiveCounter.bonusThunderRate;
                    }
                    break;
            }

            return new UpgradeBonusStatV2();
        }
        
        private static UpgradeBonusStatV2 GetPassiveLightningAmountBonus(PassiveTriggerType triggerType)
        {
            return triggerType switch
            {
                PassiveTriggerType.DameByNormalAttack => BonusInfo.bonusPassiveNormalAttack.bonusLightningAmount,
                PassiveTriggerType.DameByChargeAttack => BonusInfo.bonusPassiveChargeAttack.bonusLightningAmount,
                PassiveTriggerType.DameByMoveSKill => BonusInfo.bonusPassiveMove.bonusLightningAmount,
                PassiveTriggerType.TowerTakeDame => BonusInfo.bonusPassiveCounter.bonusLightningAmount,
                _ => new UpgradeBonusStatV2()
            };
        }
        
        private static UpgradeBonusStatV2 GetPassiveExplosiveSizeBonus(PassiveTriggerType triggerType)
        {
            return triggerType switch
            {
                PassiveTriggerType.DameByNormalAttack => BonusInfo.bonusPassiveNormalAttack.bonusExplosiveSize,
                PassiveTriggerType.DameByChargeAttack => BonusInfo.bonusPassiveChargeAttack.bonusExplosiveSize,
                PassiveTriggerType.DameByMoveSKill => BonusInfo.bonusPassiveMove.bonusExplosiveSize,
                PassiveTriggerType.TowerTakeDame => BonusInfo.bonusPassiveCounter.bonusExplosiveSize,
                _ => new UpgradeBonusStatV2()
            };
        }
        
        private static UpgradeBonusStatV2 GetPassiveBurningDurationBonus(PassiveTriggerType triggerType)
        {
            return triggerType switch
            {
                PassiveTriggerType.DameByNormalAttack => BonusInfo.bonusPassiveNormalAttack.bonusBurningDuration,
                PassiveTriggerType.DameByChargeAttack => BonusInfo.bonusPassiveChargeAttack.bonusBurningDuration,
                PassiveTriggerType.DameByMoveSKill => BonusInfo.bonusPassiveMove.bonusBurningDuration,
                PassiveTriggerType.TowerTakeDame => BonusInfo.bonusPassiveCounter.bonusBurningDuration,
                _ => new UpgradeBonusStatV2()
            };
        }
        
        private static UpgradeBonusStatV2 GetPassiveThunderExecutionChanceBonus(PassiveTriggerType triggerType)
        {
            return triggerType switch
            {
                PassiveTriggerType.DameByNormalAttack => BonusInfo.bonusPassiveNormalAttack.bonusThunderExecutionChance,
                PassiveTriggerType.DameByChargeAttack => BonusInfo.bonusPassiveChargeAttack.bonusThunderExecutionChance,
                PassiveTriggerType.DameByMoveSKill => BonusInfo.bonusPassiveMove.bonusThunderExecutionChance,
                PassiveTriggerType.TowerTakeDame => BonusInfo.bonusPassiveCounter.bonusThunderExecutionChance,
                _ => new UpgradeBonusStatV2()
            };
        }
        
        #endregion
    }
}