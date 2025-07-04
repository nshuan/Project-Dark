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
            BonusSkill(skillId, level, ref bonusInfo);
        }

        private void BonusSkill(int id, int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (!bonusInfo.skillBonusMapById.ContainsKey(id))
                bonusInfo.skillBonusMapById.Add(id, new UpgradeBonusSkillInfo());
            
            switch (bonusType)
            {
                case BonusSkillType.Damage:
                    if (isMultiply) bonusInfo.skillBonusMapById[id].skillDameMultiply += value[level - 1];
                    else bonusInfo.skillBonusMapById[id].skillDamePlus += (int)value[level - 1];
                    break;
                case BonusSkillType.Cooldown:
                    if (isMultiply) bonusInfo.skillBonusMapById[id].skillCooldownMultiply += value[level - 1];
                    else bonusInfo.skillBonusMapById[id].skillCooldownPlus += value[level - 1];
                    break;
                case BonusSkillType.Size:
                    bonusInfo.skillBonusMapById[id].skillSizeMultiply += value[level - 1];
                    break;
                case BonusSkillType.Range:
                    bonusInfo.skillBonusMapById[id].skillRangeMultiply += value[level - 1];
                    break;
                case BonusSkillType.BulletNum:
                    bonusInfo.skillBonusMapById[id].bulletPlus += (int)value[level - 1];
                    break;
                case BonusSkillType.Stagger:
                    bonusInfo.skillBonusMapById[id].staggerMultiply += value[level - 1];
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