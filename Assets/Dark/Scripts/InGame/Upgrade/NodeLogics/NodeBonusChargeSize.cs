using System;
using System.Globalization;
using InGame.ChargeConfig;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeBonusChargeSize : INodeActivateLogic
    {
        public float[] value;
	    
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (level <= 0 || level > value.Length) return;

            bonusInfo.chargeBonus.maxBulletExplodeChargeSize += (int)value[level - 1];
        }

        public (string, string) GetBeforeAfterValueTotalStat(int level, ref UpgradeBonusInfo bonusInfo)
        {
            var before = LevelUtility.GetChargeSizeExplodeBullet((int)PlayerChargeManifest.Get(ChargeType.Size).value);
            if (level > value.Length)
            {
                return (before.ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture), before.ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture));
            }
            var maxBulletExplodeChargeSize = bonusInfo.chargeBonus.maxBulletExplodeChargeSize;
            ActivateNode(level, ref bonusInfo);
            var after = LevelUtility.GetChargeSizeExplodeBullet((int)PlayerChargeManifest.Get(ChargeType.Size).value);
            bonusInfo.chargeBonus.maxBulletExplodeChargeSize = maxBulletExplodeChargeSize;
            return (before.ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture), after.ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture));
        }

        public string GetDisplayValue(int level)
        {
            if (level < 0) return "??";
            if (level >= value.Length) level = value.Length - 1;
            
            return value[level].ToString(GameConst.FloatFormat, CultureInfo.InvariantCulture);
        }

        public int MaxLevel => value.Length;
    }
}