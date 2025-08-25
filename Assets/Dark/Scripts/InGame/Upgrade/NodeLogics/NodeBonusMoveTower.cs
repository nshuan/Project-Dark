using System;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeBonusMoveTower : INodeActivateLogic
    {
        public BonusMoveTowerType bonusType;
        public float[] value;
        public bool isMultiply;
        public string bonusDescription;
        
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

        public string GetDescription(int level)
        {
            return bonusDescription;
        }

        public enum BonusMoveTowerType
        {
            Cooldown
        }
    }
}