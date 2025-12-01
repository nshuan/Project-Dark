using System;
using System.Globalization;
using UnityEngine;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeBonusSkill : INodeActivateLogic
    {
        public BonusSkillType bonusType;
        public float[] value;
        public bool isMultiply;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (level <= 0 || level > value.Length) return;

            switch (bonusType)
            {
                case BonusSkillType.Damage:
                    if (isMultiply) bonusInfo.skillBonus.skillDameMultiply += value[level - 1];
                    else bonusInfo.skillBonus.skillDamePlus += (int)value[level - 1];
                    break;
                case BonusSkillType.Cooldown:
                    if (isMultiply) bonusInfo.skillBonus.skillCooldownMultiply += value[level - 1];
                    else bonusInfo.skillBonus.skillCooldownPlus += value[level - 1];
                    break;
                case BonusSkillType.Size:
                    bonusInfo.skillBonus.skillSizeMultiply += value[level - 1];
                    break;
                case BonusSkillType.Range:
                    bonusInfo.skillBonus.skillRangeMultiply += value[level - 1];
                    break;
                case BonusSkillType.BulletNum:
                    bonusInfo.skillBonus.bulletPlus += (int)value[level - 1];
                    break;
                case BonusSkillType.BulletMaxHit:
                    bonusInfo.skillBonus.bulletMaxHitPlus += (int)value[level - 1];
                    break;
                case BonusSkillType.Stagger:
                    bonusInfo.skillBonus.staggerMultiply += value[level - 1];
                    break;
            }
        }

        public (string, string) GetBeforeAfterValueTotalStat(int level, ref UpgradeBonusInfo bonusInfo)
        {
            var before = "";
            switch (bonusType)
            {
                case BonusSkillType.Damage:
                    before = LevelUtility.GetPlayerBulletDamage(1f).Item1.ToString(CultureInfo.InvariantCulture);
                    break;
                case BonusSkillType.Cooldown:
                    before = $"{LevelUtility.GetSkillCooldown(false).ToString(CultureInfo.InvariantCulture)}s";   
                    break;
                case BonusSkillType.Size:
                    before = LevelUtility.GetSkillSize(1).ToString(CultureInfo.InvariantCulture);
                    break;
                case BonusSkillType.Range:
                    before = LevelUtility.GetSkillRange(1, Vector2.right).ToString(CultureInfo.InvariantCulture);
                    break;
                case BonusSkillType.BulletNum:
                    before = LevelUtility.GetNumberOfBullets(1).ToString(CultureInfo.InvariantCulture);
                    break;
                case BonusSkillType.BulletMaxHit:
                    before = (1 + bonusInfo.skillBonus.bulletMaxHitPlus).ToString(CultureInfo.InvariantCulture);
                    break;
                case BonusSkillType.Stagger:
                    before = LevelUtility.GetBulletStagger().ToString(CultureInfo.InvariantCulture);
                    break;
            }
            if (level > value.Length)
                return (before, before); 
            var skillDameMultiply = bonusInfo.skillBonus.skillDameMultiply;
            var skillDamePlus = bonusInfo.skillBonus.skillDamePlus;
            var skillCooldownMultiply = bonusInfo.skillBonus.skillCooldownMultiply;
            var skillCooldownPlus = bonusInfo.skillBonus.skillCooldownPlus;
            var skillSizeMultiply = bonusInfo.skillBonus.skillSizeMultiply;
            var skillRangeMultiply = bonusInfo.skillBonus.skillRangeMultiply;
            var bulletPlus = bonusInfo.skillBonus.bulletPlus;
            var bulletMaxHitPlus = bonusInfo.skillBonus.bulletMaxHitPlus;
            var staggerMultiply = bonusInfo.skillBonus.staggerMultiply;
            ActivateNode(level, ref bonusInfo);
            var after = "";
            switch (bonusType)
            {
                case BonusSkillType.Damage:
                    after = LevelUtility.GetPlayerBulletDamage(1f).Item1.ToString(CultureInfo.InvariantCulture);
                    break;
                case BonusSkillType.Cooldown:
                    after = $"{LevelUtility.GetSkillCooldown(false).ToString(CultureInfo.InvariantCulture)}s";
                    break;
                case BonusSkillType.Size:
                    after = LevelUtility.GetSkillSize(1).ToString(CultureInfo.InvariantCulture);
                    break;
                case BonusSkillType.Range:
                    after = LevelUtility.GetSkillRange(1, Vector2.right).ToString(CultureInfo.InvariantCulture);
                    break;
                case BonusSkillType.BulletNum:
                    after = LevelUtility.GetNumberOfBullets(1).ToString(CultureInfo.InvariantCulture);
                    break;
                case BonusSkillType.BulletMaxHit:
                    after = (1 + bulletMaxHitPlus).ToString(CultureInfo.InvariantCulture);
                    break;
                case BonusSkillType.Stagger:
                    after = LevelUtility.GetBulletStagger().ToString(CultureInfo.InvariantCulture);
                    break;
            }
            bonusInfo.skillBonus.skillDameMultiply = skillDameMultiply;
            bonusInfo.skillBonus.skillDamePlus = skillDamePlus;
            bonusInfo.skillBonus.skillCooldownMultiply = skillCooldownMultiply;
            bonusInfo.skillBonus.skillCooldownPlus = skillCooldownPlus;
            bonusInfo.skillBonus.skillSizeMultiply = skillSizeMultiply;
            bonusInfo.skillBonus.skillRangeMultiply = skillRangeMultiply;
            bonusInfo.skillBonus.bulletPlus = bulletPlus;
            bonusInfo.skillBonus.bulletMaxHitPlus = bulletMaxHitPlus;
            bonusInfo.skillBonus.staggerMultiply = staggerMultiply;
            return (before, after); 
        }

        public string GetDisplayValue(int level)
        {
            if (level < 0) return "??";
            if (level >= value.Length) level = value.Length - 1;
            
            switch (bonusType)
            {
                case BonusSkillType.Damage:
                case BonusSkillType.Cooldown:
                case BonusSkillType.Size:
                case BonusSkillType.Range:
                    if (isMultiply) return (value[level] * 100).ToString(CultureInfo.InvariantCulture);
                    else return value[level].ToString(CultureInfo.InvariantCulture);
                case BonusSkillType.Stagger:
                    return (value[level] * 100).ToString(CultureInfo.InvariantCulture);
                case BonusSkillType.BulletNum:
                case BonusSkillType.BulletMaxHit:
                    break;
            }
            return value[level].ToString(CultureInfo.InvariantCulture);
        }

        public int MaxLevel => value.Length;
        
        public enum BonusSkillType
        {
            Damage,
            Cooldown,
            Size,
            Range,
            BulletNum,
            BulletMaxHit,
            Stagger
        }
    }
}