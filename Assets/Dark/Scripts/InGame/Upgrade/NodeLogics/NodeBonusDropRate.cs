using System;

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
            return (value[level] * 100).ToString();
        }

        public int MaxLevel => value.Length;
    }
}