using System;

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
            
            var total = 0f;
            for (int i = 0; i <= level; i++)
            {
                if (i >= value.Length) break;
                total += value[i];
            }
            
            return total.ToString();
        }

        public int MaxLevel => value.Length;
    }
}