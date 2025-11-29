using System;
using System.Globalization;

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
            BonusSkill(level, ref bonusInfo);
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

        public (string, string) GetBeforeAfterValue(int level)
        {
            var before = "";
            var after = "";
            if (level < 0) return ("", "");
            if (level >= value.Length) level = value.Length - 1;
            if (level == 0)
            {
                before = "0";
                after = GetDisplayValue(level);
            }
            else
            {
                var sum = 0f;
                for (var i = 0; i < level; i++)
                    sum += value[i];
                
                switch (bonusType)
                {
                    case BonusSkillType.Damage:
                    case BonusSkillType.Cooldown:
                    case BonusSkillType.Size:
                    case BonusSkillType.Range:
                        if (isMultiply)
                        {
                            before = (sum * 100).ToString(CultureInfo.InvariantCulture);
                            after = ((sum + value[level]) * 100).ToString(CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            before = sum.ToString(CultureInfo.InvariantCulture);
                            after = (sum + value[level]).ToString(CultureInfo.InvariantCulture);
                        }
                        break;
                    case BonusSkillType.Stagger:
                        before = (sum * 100).ToString(CultureInfo.InvariantCulture);
                        after = ((sum + value[level]) * 100).ToString(CultureInfo.InvariantCulture);
                        break;
                    case BonusSkillType.BulletNum:
                    case BonusSkillType.BulletMaxHit:
                        break;
                    default:
                        before = sum.ToString(CultureInfo.InvariantCulture);
                        after = (sum + value[level]).ToString(CultureInfo.InvariantCulture);
                        break;
                }
            }
		    
            return (before, after);
        }

        public int MaxLevel => value.Length;

        private void BonusSkill(int level, ref UpgradeBonusInfo bonusInfo)
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