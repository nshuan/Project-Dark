using System;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeBonusDash : INodeActivateLogic
    {
        public BonusType bonusType;
        public float[] value;
        public bool isMultiply;

        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (level <= 0 || level > value.Length) return;

            switch (bonusType)
            {
                case BonusType.Cooldown:
                    bonusInfo.dashCooldownPlus += value[level - 1];
                    break;
                case BonusType.Size:
                    bonusInfo.dashSizePlus += value[level - 1];
                    break;
                case BonusType.Damage:
                    bonusInfo.dashDamagePlus += (int)value[level - 1];
                    break;
            }
        }

        public string GetDisplayValue(int level)
        {
            if (level < 0 || level > value.Length) return "??";
            return value[level].ToString();
        }
        
        public enum BonusType
        {
            Cooldown,
            Size,
            Damage
        }
    }
}