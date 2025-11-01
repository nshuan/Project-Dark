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
                    if (isMultiply) bonusInfo.dashDamageMultiplier += (int)value[level - 1];
                    bonusInfo.dashDamagePlus += (int)value[level - 1];
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
            
            return total.ToString();
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