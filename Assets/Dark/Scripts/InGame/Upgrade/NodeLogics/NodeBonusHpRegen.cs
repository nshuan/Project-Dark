using System;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeBonusHpRegen : INodeActivateLogic
    {
        public BonusType bonusType;
        public float[] value;
        public bool isMultiply;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (level <= 0 || level > value.Length) return;

            switch (bonusType)
            {
                case BonusType.AutoRegenerate:
                    bonusInfo.toleranceRegenPercentPerSecond += value[level - 1];
                    break;
                case BonusType.OnEnemyDied:
                    bonusInfo.toleranceRegenPercentWhenKill += value[level - 1];
                    break;
            }
        }

        public string GetDisplayValue(int level)
        {
            if (level < 0) return "??";
            if (level >= value.Length) level = value.Length - 1;
            return (value[level] * 100).ToString();
        }

        public enum BonusType
        {
            AutoRegenerate,
            OnEnemyDied
        }
    }
}