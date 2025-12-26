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
                        bonusInfo.dashCooldownMultiplier += value[level - 1];
                        bonusInfo.flashCooldownMultiplier += value[level - 1];
                    }
                    else
                    {
                        bonusInfo.moveCooldownPlus += value[level - 1];
                        bonusInfo.dashCooldownPlus += value[level - 1];
                        bonusInfo.flashCooldownPlus += value[level - 1];
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
                    before = $"{LevelUtility.GetTeleCooldown().ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture)}s";
                    if (bonusInfo.unlockedMoveToTower is { Count: > 0 })
                    {
                        if (bonusInfo.unlockedMoveToTower[0] == 1)
                            before = $"{LevelUtility.GetFlashCooldown().ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture)}s";
                        else if (bonusInfo.unlockedMoveToTower[0] == 2)
                            before = $"{LevelUtility.GetDashCooldown().ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture)}s";
                    }
                    break;
            }
            if (level > value.Length)
                return (before, before);
            var moveCooldownMultiplier = bonusInfo.moveCooldownMultiplier;
            var moveCooldownPlus = bonusInfo.moveCooldownPlus;
            if (bonusInfo.unlockedMoveToTower is { Count: > 0 })
            {
                if (bonusInfo.unlockedMoveToTower[0] == 1)
                {
                    moveCooldownMultiplier = bonusInfo.flashCooldownMultiplier;
                    moveCooldownPlus = bonusInfo.flashCooldownPlus;
                }
                else if (bonusInfo.unlockedMoveToTower[0] == 2)
                {
                    moveCooldownMultiplier = bonusInfo.dashCooldownMultiplier;
                    moveCooldownPlus = bonusInfo.dashCooldownPlus;
                }
            }
            ActivateNode(level, ref bonusInfo);
            var after = "";
            switch (bonusType)
            {
                case BonusMoveTowerType.Cooldown:
                    after = $"{LevelUtility.GetTeleCooldown().ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture)}s";
                    if (bonusInfo.unlockedMoveToTower is { Count: > 0 })
                    {
                        if (bonusInfo.unlockedMoveToTower[0] == 1)
                            after = $"{LevelUtility.GetFlashCooldown().ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture)}s";
                        else if (bonusInfo.unlockedMoveToTower[0] == 2)
                            after = $"{LevelUtility.GetDashCooldown().ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture)}s";
                    }
                    break;
            }
            if (bonusInfo.unlockedMoveToTower is { Count: > 0 })
            {
                if (bonusInfo.unlockedMoveToTower[0] == 1)
                {
                    bonusInfo.flashCooldownMultiplier = moveCooldownMultiplier;
                    bonusInfo.flashCooldownPlus = moveCooldownPlus;
                }
                else if (bonusInfo.unlockedMoveToTower[0] == 2)
                {
                    bonusInfo.dashCooldownMultiplier = moveCooldownMultiplier;
                    bonusInfo.dashCooldownPlus = moveCooldownPlus;
                }
            }
            else
            {
                bonusInfo.moveCooldownMultiplier = moveCooldownMultiplier;
                bonusInfo.moveCooldownPlus = moveCooldownPlus;
            }
            return (before, after);
        }

        public string GetDisplayValue(int level)
        {
            if (level < 0) return "??";
            if (level >= value.Length) level = value.Length - 1;
            
            switch (bonusType)
            {
                case BonusMoveTowerType.Cooldown:
                    if (isMultiply) return (value[level] * 100).ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture);
                    return value[level].ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture);
                case BonusMoveTowerType.CastTime:
                    return (value[level] * 100).ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture);
            }
            
            return value[level].ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture);
        }

        public int MaxLevel => value.Length;

        public enum BonusMoveTowerType
        {
            Cooldown,
            CastTime
        }
    }
}