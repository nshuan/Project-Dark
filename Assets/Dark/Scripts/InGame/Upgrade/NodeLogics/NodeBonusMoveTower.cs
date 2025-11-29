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
                    case BonusMoveTowerType.Cooldown:
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
                    case BonusMoveTowerType.CastTime:
                        before = (sum * 100).ToString(CultureInfo.InvariantCulture);
                        after = ((sum + value[level]) * 100).ToString(CultureInfo.InvariantCulture);
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

        public enum BonusMoveTowerType
        {
            Cooldown,
            CastTime
        }
    }
}