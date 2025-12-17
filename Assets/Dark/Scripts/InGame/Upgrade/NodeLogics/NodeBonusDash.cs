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

        public (string, string) GetBeforeAfterValueTotalStat(int level, ref UpgradeBonusInfo bonusInfo)
        {
            var before = "";
            switch (bonusType)
            {
                case BonusType.Cooldown:
                    before = $"{LevelUtility.GetDashCooldown().ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture)}s";
                    break;
                case BonusType.Size:
                    before = LevelUtility.GetDashSize().ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture);
                    break;
                case BonusType.Damage:
                    before = LevelUtility.GetDashDamage().ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture);
                    break;
            }
            if (level > value.Length)
            {
                return (before, before);
            }
            var dashCooldownMultiplier = bonusInfo.dashCooldownMultiplier;
            var dashCooldownPlus = bonusInfo.dashCooldownPlus;
            var dashSizeMultiplier = bonusInfo.dashSizeMultiplier;
            var dashSizePlus = bonusInfo.dashSizePlus;
            var dashDamageMultiplier = bonusInfo.dashDamageMultiplier;
            var dashDamagePlus = bonusInfo.dashDamagePlus;
            ActivateNode(level, ref bonusInfo);
            var after = "";
            switch (bonusType)
            {
                case BonusType.Cooldown:
                    after = $"{LevelUtility.GetDashCooldown().ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture)}s";
                    break;
                case BonusType.Size:
                    after = LevelUtility.GetDashSize().ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture);
                    break;
                case BonusType.Damage:
                    after = LevelUtility.GetDashDamage().ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture);
                    break;
            }
            bonusInfo.dashCooldownMultiplier = dashCooldownMultiplier;
            bonusInfo.dashCooldownPlus = dashCooldownPlus; 
            bonusInfo.dashSizeMultiplier = dashSizeMultiplier;
            bonusInfo.dashSizePlus = dashSizePlus;
            bonusInfo.dashDamageMultiplier = dashDamageMultiplier;
            bonusInfo.dashDamagePlus = dashDamagePlus;
            return (before, after);
        }

        public string GetDisplayValue(int level)
        {
            if (level < 0) return "??";
            if (level >= value.Length) level = value.Length - 1;
            
            if (isMultiply)
                return (value[level] * 100).ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture);
            return value[level].ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture);
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