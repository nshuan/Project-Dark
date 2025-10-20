using System;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeBonusFlash : INodeActivateLogic
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
                    bonusInfo.flashCooldownPlus += value[level - 1];
                    break;
                case BonusType.Size:
                    bonusInfo.flashSizePlus += value[level - 1];
                    break;
                case BonusType.Damage:
                    if (isMultiply) bonusInfo.flashDamageMultiplier += (int)value[level - 1];
                    else bonusInfo.flashDamagePlus += (int)value[level - 1];
                    break;
            }
        }

        public string GetDisplayValue(int level)
        {
            if (level < 0) return "??";
            if (level >= value.Length) level = value.Length - 1;
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