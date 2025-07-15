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
        /// Bullet_Dame = Player_Damage + SKill_Damage
        /// Player_Damage = [ Base_Damage + Total (Dame_Plus) ] * [1 + Total (Dame_Multiple) ]
        /// SKill_Damage = [ Dame_Per_Bullet + Total (Skill_Dame_Plus) ] * [ 1 + Total (Skill_Dame_Multiple) ]
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
            skillDamage = Mathf.RoundToInt((skillDamage + BonusInfo.skillBonus.skillDamePlus) * (1 + BonusInfo.skillBonus.skillDameMultiply));
            criticalDameMultiplier = chargeDameMultiplier + BonusInfo.criticalDame;
            var bulletDamage = playerDamage + skillDamage;
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
        /// Final_Cooldown = Skill_Cooldown * Player_Cooldown
        /// </summary>
        /// <param name="playerCooldown"></param>
        /// <param name="baseSkillCooldown"></param>
        /// <returns></returns>
        public static float GetSkillCooldown(int skillId, float playerCooldown, float baseSkillCooldown)
        {
            return (baseSkillCooldown - BonusInfo.skillBonus.skillCooldownPlus) * (1 - BonusInfo.skillBonus.skillCooldownMultiply) * (playerCooldown + BonusInfo.cooldownPlus);
        }

        /// <summary>
        /// Skill_Range = Range * [1 + Total (Skill_Range_Multiple) ] * [ 1 + ( Charge_Range_Max / Charge_Range_Time ) * Charge_Time ]
        /// </summary>
        /// <param name="baseRange"></param>
        /// <param name="chargeRange"></param>
        /// <returns></returns>
        public static float GetSkillRange(int skillId, float baseRange, float chargeRange)
        {
            return baseRange * (1 + BonusInfo.skillBonus.skillRangeMultiply) * chargeRange;
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
    }
}