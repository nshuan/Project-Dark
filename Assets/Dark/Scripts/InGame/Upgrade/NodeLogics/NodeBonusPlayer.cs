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

        public string GetDescription(int level)
        {
            var result = "";
            return result;
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