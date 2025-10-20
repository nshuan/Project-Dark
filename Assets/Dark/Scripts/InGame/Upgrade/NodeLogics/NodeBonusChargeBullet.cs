using System;

namespace InGame.Upgrade
{
	[Serializable]
    public class NodeBonusChargeBullet : INodeActivateLogic
    {
	    public BonusType bonusType;
	    public float[] value;
	    
	    public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
	    {
		    if (level <= 0 || level > value.Length) return;

		    switch (bonusType)
		    {
			    case BonusType.MaxDame:
				    bonusInfo.chargeBulletBonus.maxDameMultiplier += value[level - 1];
				    break;
			    case BonusType.MaxDameTime:
				    bonusInfo.chargeBulletBonus.maxDameChargeTime += value[level - 1];
				    break;
			    case BonusType.MaxSize:
				    bonusInfo.chargeBulletBonus.maxSizeMultiplier += value[level - 1];
				    break;
			    case BonusType.MaxSizeTime:
				    bonusInfo.chargeBulletBonus.maxSizeChargeTime += value[level - 1];
				    break;
			    case BonusType.MaxRange:
				    bonusInfo.chargeBulletBonus.maxRangeMultiplier += value[level - 1];
				    break;
			    case BonusType.MaxRangeTime:
				    bonusInfo.chargeBulletBonus.maxRangeChargeTime += value[level - 1];
				    break;
			    case BonusType.MaxBulletAdd:
				    bonusInfo.chargeBulletBonus.maxBulletAdd += (int)value[level - 1];
				    break;
			    case BonusType.BulletAddInterval:
				    bonusInfo.chargeBulletBonus.bulletAddInterval += value[level - 1];
				    break;
		    }
	    }

	    public string GetDisplayValue(int level)
	    {
		    if (level < 0) return "??";
		    if (level >= value.Length) level = value.Length - 1;
		    switch (bonusType)
		    {
			    case BonusType.MaxDame:
			    case BonusType.MaxSize:
			    case BonusType.MaxRange:
				    return (value[level] * 100).ToString();
			    case BonusType.MaxDameTime:
			    case BonusType.MaxSizeTime:
			    case BonusType.MaxRangeTime:
			    case BonusType.MaxBulletAdd:
			    case BonusType.BulletAddInterval:
				    break;
		    }
		    
		    return value[level].ToString();
	    }
	    
		public enum BonusType
		{
			MaxDame,
			MaxDameTime,
			MaxSize,
			MaxSizeTime,
			MaxRange,
			MaxRangeTime,
			MaxBulletAdd,
			BulletAddInterval
		}
    }

}