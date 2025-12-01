using System;
using System.Globalization;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeBonusPlayer : INodeActivateLogic
    {
        public BonusPlayerType bonusType;
        public float[] value;
        public bool isMultiply;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (level <= 0 || level > value.Length) return;

            switch (bonusType)
            {
                case BonusPlayerType.Health:
                    if (isMultiply) bonusInfo.hpMultiply += value[level - 1];
                    else bonusInfo.hpPlus += (int)value[level - 1];
                    break;
                case BonusPlayerType.Damage:
                    if (isMultiply) bonusInfo.dameMultiply += value[level - 1];
                    else bonusInfo.damePlus += (int)value[level - 1];
                    break;
                case BonusPlayerType.Cooldown:
                    if (isMultiply) bonusInfo.cooldownMultiplier += value[level - 1];
                    else bonusInfo.cooldownPlus += value[level - 1];
                    break;
                case BonusPlayerType.CriticalRate:
                    bonusInfo.criticalRatePlus += value[level - 1];
                    break;
                case BonusPlayerType.CriticalDame:
                    bonusInfo.criticalDame += (int)value[level - 1];
                    break;
            }
        }

        public (string, string) GetBeforeAfterValueTotalStat(int level, ref UpgradeBonusInfo bonusInfo)
        {
            var before = "";
            switch (bonusType)
            {
                case BonusPlayerType.Health:
                    before = LevelUtility.GetTowerHp().ToString(CultureInfo.InvariantCulture);
                    break;
                case BonusPlayerType.Damage:
                    before = LevelUtility.BasePlayerDamageWithBonus.ToString(CultureInfo.InvariantCulture);
                    break;
                case BonusPlayerType.Cooldown:
                    before = $"{(LevelUtility.BasePLayerCooldownWithBonus * 100).ToString(CultureInfo.InvariantCulture)}%";
                    break;
                case BonusPlayerType.CriticalRate:
                    before = $"{(LevelUtility.GetCriticalRate() * 100).ToString(CultureInfo.InvariantCulture)}%";
                    break;
            }
            if (level >= value.Length)
                return (before, before);
 
            var hpMultiply = bonusInfo.hpMultiply;
            var hpPlus = bonusInfo.hpPlus;
            var dameMultiply = bonusInfo.dameMultiply;
            var damePlus = bonusInfo.damePlus;
            var cooldownMultiplier = bonusInfo.cooldownMultiplier;
            var cooldownPlus = bonusInfo.cooldownPlus;
            var criticalRatePlus = bonusInfo.criticalRatePlus;
            var criticalDame = bonusInfo.criticalDame;
            ActivateNode(level, ref bonusInfo);
            var after = "";
            switch (bonusType)
            {
                case BonusPlayerType.Health:
                    after = LevelUtility.GetTowerHp().ToString(CultureInfo.InvariantCulture);
                    break;
                case BonusPlayerType.Damage:
                    after = LevelUtility.BasePlayerDamageWithBonus.ToString(CultureInfo.InvariantCulture);
                    break;
                case BonusPlayerType.Cooldown:
                    after = $"{(LevelUtility.BasePLayerCooldownWithBonus * 100).ToString(CultureInfo.InvariantCulture)}%";
                    break;
                case BonusPlayerType.CriticalRate:
                    after = $"{(LevelUtility.GetCriticalRate() * 100).ToString(CultureInfo.InvariantCulture)}%";
                    break;
            }
            bonusInfo.hpMultiply = hpMultiply;
            bonusInfo.hpPlus = hpPlus;
            bonusInfo.dameMultiply = dameMultiply;
            bonusInfo.damePlus = damePlus;
            bonusInfo.cooldownMultiplier = cooldownMultiplier;
            bonusInfo.cooldownPlus = cooldownPlus;
            bonusInfo.criticalRatePlus = criticalRatePlus;
            bonusInfo.criticalDame = criticalDame;
            return (before, after);
        }

        public string GetDisplayValue(int level)
        {
            if (level < 0) return "??";
            if (level >= value.Length) level = value.Length - 1;
            
            switch (bonusType)
            {
                case BonusPlayerType.Health:
                case BonusPlayerType.Damage:
                    if (isMultiply) return (value[level] * 100).ToString(CultureInfo.InvariantCulture);
                    else return value[level].ToString(CultureInfo.InvariantCulture);
                case BonusPlayerType.Cooldown:
                case BonusPlayerType.CriticalRate:
                case BonusPlayerType.CriticalDame:
                    return (value[level] * 100).ToString(CultureInfo.InvariantCulture);
            }
            return value[level].ToString(CultureInfo.InvariantCulture);
        }

        public int MaxLevel => value.Length;

        public enum BonusPlayerType
        {
            Health,
            Damage,
            Cooldown,
            CriticalRate,
            CriticalDame
        }
    }
}