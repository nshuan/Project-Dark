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
				    bonusInfo.chargeBulletBonus.maxDameChargeTimeMinus += value[level - 1];
				    break;
			    case BonusType.MaxSize:
				    bonusInfo.chargeBulletBonus.maxSizeMultiplier += value[level - 1];
				    break;
			    case BonusType.MaxSizeTime:
				    bonusInfo.chargeBulletBonus.maxSizeChargeTimeMinus += value[level - 1];
				    break;
			    case BonusType.MaxRange:
				    bonusInfo.chargeBulletBonus.maxRangeMultiplier += value[level - 1];
				    break;
			    case BonusType.MaxRangeTime:
				    bonusInfo.chargeBulletBonus.maxRangeChargeTimeMinus += value[level - 1];
				    break;
			    case BonusType.MaxBulletAdd:
				    bonusInfo.chargeBulletBonus.maxBulletAdd += (int)value[level - 1];
				    break;
			    case BonusType.BulletAddInterval:
				    bonusInfo.chargeBulletBonus.bulletAddIntervalMinus += value[level - 1];
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
			    case BonusType.MaxDame:
			    case BonusType.MaxSize:
			    case BonusType.MaxRange:
				    return (total * 100).ToString();
			    case BonusType.MaxDameTime:
			    case BonusType.MaxSizeTime:
			    case BonusType.MaxRangeTime:
			    case BonusType.MaxBulletAdd:
			    case BonusType.BulletAddInterval:
				    break;
		    }
		    
		    return total.ToString();
	    }

	    public int MaxLevel => value.Length;

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