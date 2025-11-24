using System;
using System.Globalization;

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
                    if (isMultiply) bonusInfo.dashCooldownMultiplier += value[level - 1];
                    else bonusInfo.dashCooldownPlus += value[level - 1];
                    break;
                case BonusType.Size:
                    if (isMultiply) bonusInfo.dashSizeMultiplier += value[level - 1];
                    else bonusInfo.dashSizePlus += value[level - 1];
                    break;
                case BonusType.Damage:
                    if (isMultiply) bonusInfo.dashDamageMultiplier += value[level - 1];
                    else bonusInfo.dashDamagePlus += (int)value[level - 1];
                    break;
            }
        }

        public string GetDisplayValue(int level)
        {
            if (level < 0) return "??";
            if (level >= value.Length) level = value.Length - 1;
            
            if (isMultiply)
                return (value[level] * 100).ToString(CultureInfo.InvariantCulture);
            return value[level].ToString(CultureInfo.InvariantCulture);
        }

        public int MaxLevel => value.Length;

        public enum BonusType
        {
            Cooldown,
            Size,
            Damage
        }
    }
}