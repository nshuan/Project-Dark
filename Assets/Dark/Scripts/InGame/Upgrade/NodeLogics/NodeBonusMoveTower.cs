using System;
using System.Globalization;

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
                    if (isMultiply)
                    {
                        bonusInfo.moveCooldownMultiplier += value[level - 1];
                        // bonusInfo.dashCooldownMultiplier += value[level - 1];
                        // bonusInfo.flashCooldownMultiplier += value[level - 1];
                    }
                    else
                    {
                        bonusInfo.moveCooldownPlus += value[level - 1];
                        // bonusInfo.dashCooldownPlus += value[level - 1];
                        // bonusInfo.flashCooldownPlus += value[level - 1];
                    }
                    break;
            }
        }

        public (string, string) GetBeforeAfterValueTotalStat(int level, ref UpgradeBonusInfo bonusInfo)
        {
            var before = "";
            switch (bonusType)
            {
                case BonusMoveTowerType.Cooldown:
                    before = $"{LevelUtility.GetTeleCooldown().ToString(CultureInfo.InvariantCulture)}s";
                    break;
            }
            if (level > value.Length)
                return (before, before);
            var moveCooldownMultiplier = bonusInfo.moveCooldownMultiplier;
            var moveCooldownPlus = bonusInfo.moveCooldownPlus;
            ActivateNode(level, ref bonusInfo);
            var after = "";
            switch (bonusType)
            {
                case BonusMoveTowerType.Cooldown:
                    after = $"{LevelUtility.GetTeleCooldown().ToString(CultureInfo.InvariantCulture)}s";
                    break;
            }
            bonusInfo.moveCooldownMultiplier = moveCooldownMultiplier;
            bonusInfo.moveCooldownPlus = moveCooldownPlus;
            return (before, after);
        }

        public string GetDisplayValue(int level)
        {
            if (level < 0) return "??";
            if (level >= value.Length) level = value.Length - 1;
            
            switch (bonusType)
            {
                case BonusMoveTowerType.Cooldown:
                    if (isMultiply) return (value[level] * 100).ToString(CultureInfo.InvariantCulture);
                    return value[level].ToString(CultureInfo.InvariantCulture);
                case BonusMoveTowerType.CastTime:
                    return (value[level] * 100).ToString(CultureInfo.InvariantCulture);
            }
            
            return value[level].ToString(CultureInfo.InvariantCulture);
        }

        public int MaxLevel => value.Length;

        public enum BonusMoveTowerType
        {
            Cooldown,
            CastTime
        }
    }
}