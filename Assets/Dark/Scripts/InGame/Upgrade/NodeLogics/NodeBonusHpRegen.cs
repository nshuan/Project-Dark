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
            
            var total = 0f;
            for (int i = 0; i <= level; i++)
            {
                if (i >= value.Length) break;
                total += value[i];
            }
            
            switch (bonusType)
            {
                case BonusType.AutoRegenerate:
                    return total.ToString();
                case BonusType.OnEnemyDied:
                    return (total * 100).ToString();
            }
            return total.ToString();
        }

        public int MaxLevel => value.Length;

        public enum BonusType
        {
            AutoRegenerate,
            OnEnemyDied
        }
    }
}