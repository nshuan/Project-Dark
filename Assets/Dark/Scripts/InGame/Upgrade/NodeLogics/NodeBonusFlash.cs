using System;
using System.Globalization;

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
                    if (isMultiply) bonusInfo.flashCooldownMultiplier += value[level - 1];
                    else bonusInfo.flashCooldownPlus += value[level - 1];
                    break;
                case BonusType.Size:
                    if (isMultiply) bonusInfo.flashSizeMultiplier += value[level - 1];
                    else bonusInfo.flashSizePlus += value[level - 1];
                    break;
                case BonusType.Damage:
                    if (isMultiply) bonusInfo.flashDamageMultiplier += value[level - 1];
                    else bonusInfo.flashDamagePlus += (int)value[level - 1];
                    break;
            }
        }

        public (string, string) GetBeforeAfterValueTotalStat(int level, ref UpgradeBonusInfo bonusInfo)
        {
            var before = "";
            switch (bonusType)
            {
                case BonusType.Cooldown:
                    before = $"{LevelUtility.GetFlashCooldown().ToString(CultureInfo.InvariantCulture)}s";
                    break;
                case BonusType.Size:
                    before = LevelUtility.GetFlashSize().ToString(CultureInfo.InvariantCulture);
                    break;
                case BonusType.Damage:
                    before = LevelUtility.GetFlashDamage().ToString(CultureInfo.InvariantCulture);
                    break;
            }
            if (level > value.Length)
            {
                return (before, before);
            }
            var flashCooldownMultiplier = bonusInfo.flashCooldownMultiplier;
            var flashCooldownPlus = bonusInfo.flashCooldownPlus;
            var flashSizeMultiplier = bonusInfo.flashSizeMultiplier;
            var flashSizePlus = bonusInfo.flashSizePlus;
            var flashDamageMultiplier = bonusInfo.flashDamageMultiplier;
            var flashDamagePlus = bonusInfo.flashDamagePlus;
            ActivateNode(level, ref bonusInfo);
            var after = "";
            switch (bonusType)
            {
                case BonusType.Cooldown:
                    after = $"{LevelUtility.GetFlashCooldown().ToString(CultureInfo.InvariantCulture)}s";
                    break;
                case BonusType.Size:
                    after = LevelUtility.GetFlashSize().ToString(CultureInfo.InvariantCulture);
                    break;
                case BonusType.Damage:
                    after = LevelUtility.GetFlashDamage().ToString(CultureInfo.InvariantCulture);
                    break;
            }
            bonusInfo.flashCooldownMultiplier = flashCooldownMultiplier;
            bonusInfo.flashCooldownPlus = flashCooldownPlus;
            bonusInfo.flashSizeMultiplier = flashSizeMultiplier;
            bonusInfo.flashSizePlus = flashSizePlus;
            bonusInfo.flashDamageMultiplier = flashDamageMultiplier;
            bonusInfo.flashDamagePlus = flashDamagePlus;
            return (before, after);
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