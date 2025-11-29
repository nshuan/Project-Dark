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

        public string GetDisplayValue(int level)
        {
            if (level < 0) return "??";
            if (level >= value.Length) level = value.Length - 1;
            
            return (value[level] * 100).ToString(CultureInfo.InvariantCulture);
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
                before = (sum * 100).ToString(CultureInfo.InvariantCulture);
                after = ((sum + value[level]) * 100).ToString(CultureInfo.InvariantCulture);
            }
		    
            return (before, after);
        }

        public int MaxLevel => value.Length;
    }
}