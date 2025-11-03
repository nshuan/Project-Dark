using System;
using System.Globalization;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeBonusMoveTower : INodeActivateLogic
    {
        public BonusMoveTowerType bonusType;
        public float[] value;
        public bool isMultiply;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (level <= 0 || level > value.Length) return;

            switch (bonusType)
            {
                case BonusMoveTowerType.Cooldown:
                    if (isMultiply) bonusInfo.moveCooldownMultiplier += value[level - 1];
                    else bonusInfo.moveCooldownPlus += value[level - 1];
                    break;
            }
        }

        public string GetDisplayValue(int level)
        {
            if (level < 0) return "??";
            if (level >= value.Length) level = value.Length - 1;
            
            switch (bonusType)
            {
                case BonusMoveTowerType.Cooldown:
                    if (isMultiply) return (value[level] * 100).ToString(CultureInfo.InvariantCulture);
                    return value[level].ToString(CultureInfo.InvariantCulture);
                case BonusMoveTowerType.CastTime:
                    return (value[level] * 100).ToString(CultureInfo.InvariantCulture);
            }
            
            return value[level].ToString(CultureInfo.InvariantCulture);
        }

        public int MaxLevel => value.Length;

        public enum BonusMoveTowerType
        {
            Cooldown,
            CastTime
        }
    }
}