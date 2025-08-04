using System;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeBonusSkill : INodeActivateLogic
    {
        public BonusSkillType bonusType;
        public float[] value;
        public bool isMultiply;
        public string bonusDescription;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (level <= 0 || level > value.Length) return;
            BonusSkill(level, ref bonusInfo);
        }

        public string GetDescription(int level)
        {
            return bonusDescription;
        }

        private void BonusSkill(int level, ref UpgradeBonusInfo bonusInfo)
        {
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