using System;

namespace InGame.Upgrade
{
    public class NodeBonusSkill : INodeActivateLogic
    {
        public int skillId;
        public BonusSkillType bonusType;
        public float[] value;
        public bool isMultiply;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (level <= 0 || level > value.Length) return;
            if (!bonusInfo.skillBonusMapById.ContainsKey(skillId))
                bonusInfo.skillBonusMapById.Add(skillId, new UpgradeBonusSkillInfo());

            switch (bonusType)
            {
                case BonusSkillType.Damage:
                    if (isMultiply) bonusInfo.skillBonusMapById[skillId].skillDameMultiply += value[level - 1];
                    else bonusInfo.skillBonusMapById[skillId].skillDamePlus += (int)value[level - 1];
                    break;
                case BonusSkillType.Cooldown:
                    if (isMultiply) bonusInfo.skillBonusMapById[skillId].skillCooldownMultiply += value[level - 1];
                    else bonusInfo.skillBonusMapById[skillId].skillCooldownPlus += value[level - 1];
                    break;
                case BonusSkillType.Size:
                    bonusInfo.skillBonusMapById[skillId].skillSizeMultiply += value[level - 1];
                    break;
                case BonusSkillType.Range:
                    bonusInfo.skillBonusMapById[skillId].skillRangeMultiply += value[level - 1];
                    break;
                case BonusSkillType.BulletNum:
                    bonusInfo.skillBonusMapById[skillId].bulletPlus += (int)value[level - 1];
                    break;
                case BonusSkillType.Stagger:
                    bonusInfo.skillBonusMapById[skillId].staggerMultiply += value[level - 1];
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
            Stagger
        }
    }
}