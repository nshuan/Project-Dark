using System;

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
                    bonusInfo.moveCooldownPlus += (int)value[level - 1];
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
                case BonusMoveTowerType.Cooldown:
                    break;
                case BonusMoveTowerType.CastTime:
                    return (total * 100).ToString();
            }
            
            return total.ToString();
        }

        public int MaxLevel => value.Length;

        public enum BonusMoveTowerType
        {
            Cooldown,
            CastTime
        }
    }
}