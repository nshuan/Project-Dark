using System;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeBonusChargeSize : INodeActivateLogic
    {
        public NodeBonusChargeBullet.BonusType bonusType;
        public float[] value;
	    
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (level <= 0 || level > value.Length) return;

            switch (bonusType)
            {
                case NodeBonusChargeBullet.BonusType.MaxDame:
                    bonusInfo.chargeSizeBonus.maxDameMultiplier += value[level - 1];
                    break;
                case NodeBonusChargeBullet.BonusType.MaxDameTime:
                    bonusInfo.chargeSizeBonus.maxDameChargeTimeMinus += value[level - 1];
                    break;
                case NodeBonusChargeBullet.BonusType.MaxSize:
                    bonusInfo.chargeSizeBonus.maxSizeMultiplier += value[level - 1];
                    break;
                case NodeBonusChargeBullet.BonusType.MaxSizeTime:
                    bonusInfo.chargeSizeBonus.maxSizeChargeTimeMinus += value[level - 1];
                    break;
                case NodeBonusChargeBullet.BonusType.MaxRange:
                    bonusInfo.chargeSizeBonus.maxRangeMultiplier += value[level - 1];
                    break;
                case NodeBonusChargeBullet.BonusType.MaxRangeTime:
                    bonusInfo.chargeSizeBonus.maxRangeChargeTimeMinus += value[level - 1];
                    break;
                case NodeBonusChargeBullet.BonusType.MaxBulletAdd:
                    bonusInfo.chargeSizeBonus.maxBulletAdd += (int)value[level - 1];
                    break;
                case NodeBonusChargeBullet.BonusType.BulletAddInterval:
                    bonusInfo.chargeSizeBonus.bulletAddIntervalMinus += value[level - 1];
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
                case NodeBonusChargeBullet.BonusType.MaxDame:
                case NodeBonusChargeBullet.BonusType.MaxSize:
                case NodeBonusChargeBullet.BonusType.MaxRange:
                    return (total * 100).ToString();
                case NodeBonusChargeBullet.BonusType.MaxDameTime:
                case NodeBonusChargeBullet.BonusType.MaxSizeTime:
                case NodeBonusChargeBullet.BonusType.MaxRangeTime:
                case NodeBonusChargeBullet.BonusType.MaxBulletAdd:
                case NodeBonusChargeBullet.BonusType.BulletAddInterval:
                    break;
            }
            return total.ToString();
        }

        public int MaxLevel => value.Length;
    }
}