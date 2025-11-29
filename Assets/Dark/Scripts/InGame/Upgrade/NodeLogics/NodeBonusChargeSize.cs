using System;
using System.Globalization;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeBonusChargeSize : INodeActivateLogic
    {
        public float[] value;
	    
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (level <= 0 || level > value.Length) return;

            bonusInfo.chargeBonus.maxBulletExplodeChargeSize += (int)value[level - 1];
        }

        public string GetDisplayValue(int level)
        {
            if (level < 0) return "??";
            if (level >= value.Length) level = value.Length - 1;
            
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
                before = sum.ToString(CultureInfo.InvariantCulture);
                after = (sum + value[level]).ToString(CultureInfo.InvariantCulture);
            }
		    
            return (before, after);
        }

        public int MaxLevel => value.Length;
    }
}