using System;
using System.Globalization;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeBonusDropRate : INodeActivateLogic
    {
        public float[] value;
        public bool isMultiply;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (level <= 0 || level > value.Length) return;
            
            if (isMultiply) bonusInfo.dropRateMultiply += value[level - 1];
            else bonusInfo.dropRatePlus += value[level - 1];
        }

        public (string, string) GetBeforeAfterValueTotalStat(int level, ref UpgradeBonusInfo bonusInfo)
        {
            return GetBonusBeforeAfterValue(level);
        }

        public string GetDisplayValue(int level)
        {
            if (level < 0) return "??";
            if (level >= value.Length) level = value.Length - 1;
            
            return (value[level] * 100).ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture);
        }

        public (string, string) GetBonusBeforeAfterValue(int level)
        {
            var before = "";
            var after = "";
            if (level <= 0) return ("", "");
            if (level > value.Length) level = value.Length;
            if (level == 1)
            {
                before = "+0%";
                after = $"+{GetDisplayValue(level)}%";
            }
            else
            {
                var sum = 0f;
                for (var i = 1; i < level; i++)
                    sum += value[i - 1];
                before = $"+{(sum * 100).ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture)}%";
                after = $"+{((sum + value[level - 1]) * 100).ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture)}%";
            }
		    
            return (before, after);
        }

        public int MaxLevel => value.Length;
    }
}