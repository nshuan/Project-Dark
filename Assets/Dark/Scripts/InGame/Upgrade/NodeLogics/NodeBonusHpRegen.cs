using System;
using System.Globalization;

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

        public (string, string) GetBeforeAfterValueTotalStat(int level, ref UpgradeBonusInfo bonusInfo)
        {
            var before = "";
            switch (bonusType)
            {
                case BonusType.AutoRegenerate:
                    before = LevelUtility.GetTowerAutoRegen(LevelUtility.GetTowerHp()).ToString(CultureInfo.InvariantCulture);
                    break;
                case BonusType.OnEnemyDied:
                    before = (bonusInfo.toleranceRegenPercentWhenKill * 100).ToString(CultureInfo.InvariantCulture);
                    break;
            }
            if (level >= value.Length)
                return (before, before);
            var toleranceRegenPercentPerSecond = bonusInfo.toleranceRegenPercentPerSecond;
            var toleranceRegenPercentWhenKill = bonusInfo.toleranceRegenPercentWhenKill;
            ActivateNode(level, ref bonusInfo);
            var after = "";
            switch (bonusType)
            {
                case BonusType.AutoRegenerate:
                    after = LevelUtility.GetTowerAutoRegen(LevelUtility.GetTowerHp()).ToString(CultureInfo.InvariantCulture);
                    break;
                case BonusType.OnEnemyDied:
                    after = (bonusInfo.toleranceRegenPercentWhenKill * 100).ToString(CultureInfo.InvariantCulture);
                    break;
            } 
            bonusInfo.toleranceRegenPercentPerSecond = toleranceRegenPercentPerSecond;
            bonusInfo.toleranceRegenPercentWhenKill = toleranceRegenPercentWhenKill;
            return (before, after);
        }

        public string GetDisplayValue(int level)
        {
            if (level < 0) return "??";
            if (level >= value.Length) level = value.Length - 1;

            switch (bonusType)
            {
                case BonusType.AutoRegenerate:
                    return value[level].ToString(CultureInfo.InvariantCulture);
                case BonusType.OnEnemyDied:
                    return (value[level] * 100).ToString(CultureInfo.InvariantCulture);
            }
            return value[level].ToString(CultureInfo.InvariantCulture);
        }

        public int MaxLevel => value.Length;

        public enum BonusType
        {
            AutoRegenerate,
            OnEnemyDied
        }
    }
}