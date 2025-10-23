using System;

namespace InGame.Upgrade
{
	[Serializable]
    public class NodeBonusCharge : INodeActivateLogic
    {
	    public BonusType bonusType;
	    public float[] value;
	    public bool isMul;
	    
	    public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
	    {
		    if (level <= 0 || level > value.Length) return;

		    switch (bonusType)
		    {
			    case BonusType.MaxTime:
				    if (isMul)
				    {
					    bonusInfo.chargeDameBonus.maxDameChargeTimeMinusMul += value[level - 1];
					    bonusInfo.chargeBulletBonus.maxDameChargeTimeMinusMul += value[level - 1];
					    bonusInfo.chargeSizeBonus.maxDameChargeTimeMinusMul += value[level - 1];
					    bonusInfo.chargeRangeBonus.maxDameChargeTimeMinusMul += value[level - 1];
				    }
				    else
				    {
					    bonusInfo.chargeDameBonus.maxDameChargeTimeMinus += value[level - 1];
					    bonusInfo.chargeBulletBonus.maxDameChargeTimeMinus += value[level - 1];
					    bonusInfo.chargeSizeBonus.maxDameChargeTimeMinus += value[level - 1];
					    bonusInfo.chargeRangeBonus.maxDameChargeTimeMinus += value[level - 1];
				    }
				    break;
		    }
	    }

	    public string GetDisplayValue(int level)
	    {
		    if (level < 0) return "??";
		    if (level >= value.Length) level = value.Length - 1;

		    if (isMul)
			    return (value[level] * 100).ToString();
		    
		    return value[level].ToString();
	    }

	    public int MaxLevel => value.Length;

	    public enum BonusType
		{
			MaxTime
		}
    }
}