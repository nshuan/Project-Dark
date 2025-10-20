using System;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeBonusPlayer : INodeActivateLogic
    {
        public BonusPlayerType bonusType;
        public float[] value;
        public bool isMultiply;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (level <= 0 || level > value.Length) return;

            switch (bonusType)
            {
                case BonusPlayerType.Health:
                    if (isMultiply) bonusInfo.hpMultiply += value[level - 1];
                    else bonusInfo.hpPlus += (int)value[level - 1];
                    break;
                case BonusPlayerType.Damage:
                    if (isMultiply) bonusInfo.dameMultiply += value[level - 1];
                    else bonusInfo.damePlus += (int)value[level - 1];
                    break;
                case BonusPlayerType.Cooldown:
                    bonusInfo.cooldownPlus += value[level - 1];
                    break;
                case BonusPlayerType.CriticalRate:
                    bonusInfo.criticalRatePlus += value[level - 1];
                    break;
                case BonusPlayerType.CriticalDame:
                    bonusInfo.criticalDame += (int)value[level - 1];
                    break;
            }
        }

        public string GetDisplayValue(int level)
        {
            if (level < 0) return "??";
            if (level >= value.Length) level = value.Length - 1;
            switch (bonusType)
            {
                case BonusPlayerType.Health:
                case BonusPlayerType.Damage:
                    if (isMultiply) return (value[level] * 100).ToString();
                    else return value[level].ToString();
                case BonusPlayerType.CriticalRate:
                    return (value[level] * 100).ToString();
                case BonusPlayerType.Cooldown:
                case BonusPlayerType.CriticalDame:
                    break;
            }
            return value[level].ToString();
        }

        public enum BonusPlayerType
        {
            Health,
            Damage,
            Cooldown,
            CriticalRate,
            CriticalDame
        }
    }
}