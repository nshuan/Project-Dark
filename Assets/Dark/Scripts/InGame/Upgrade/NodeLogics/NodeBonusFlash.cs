using System;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeBonusFlash : INodeActivateLogic
    {
        public BonusType bonusType;
        public float[] value;
        public bool isMultiply;
        public string bonusDescription;

        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (level <= 0 || level > value.Length) return;

            switch (bonusType)
            {
                case BonusType.Cooldown:
                    bonusInfo.flashCooldownPlus += value[level - 1];
                    break;
                case BonusType.Size:
                    bonusInfo.flashSizePlus += value[level - 1];
                    break;
                case BonusType.Damage:
                    bonusInfo.flashDamagePlus += (int)value[level - 1];
                    break;
            }
        }

        public string GetDescription(int level)
        {
            return bonusDescription;
        }
        
        public enum BonusType
        {
            Cooldown,
            Size,
            Damage
        }
    }
}