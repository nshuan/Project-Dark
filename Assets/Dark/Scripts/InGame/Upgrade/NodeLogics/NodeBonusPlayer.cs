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
                    if (isMultiply) bonusInfo.cooldownMultiplier += value[level - 1];
                    else bonusInfo.cooldownPlus += value[level - 1];
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
            
            var total = 0f;
            for (int i = 0; i <= level; i++)
            {
                if (i >= value.Length) break;
                total += value[i];
            }
            
            switch (bonusType)
            {
                case BonusPlayerType.Health:
                case BonusPlayerType.Damage:
                case BonusPlayerType.Cooldown:
                    if (isMultiply) return (total * 100).ToString();
                    else return total.ToString();
                case BonusPlayerType.CriticalRate:
                case BonusPlayerType.CriticalDame:
                    return (total * 100).ToString();
            }
            return total.ToString();
        }

        public int MaxLevel => value.Length;

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