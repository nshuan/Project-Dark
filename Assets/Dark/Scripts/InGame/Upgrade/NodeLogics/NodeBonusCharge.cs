using System;
using System.Globalization;

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
			    case BonusType.StepTime:
				    if (isMul)
				    {
					    bonusInfo.chargeBonus.stepTimeMul += value[level - 1];
				    }
				    else
				    {
					    bonusInfo.chargeBonus.stepTime += value[level - 1];
				    }
				    break;
		    }
	    }

	    public (string, string) GetBeforeAfterValueTotalStat(int level, ref UpgradeBonusInfo bonusInfo)
	    {
		    var before = LevelUtility.GetChargeStepTime().ToString(CultureInfo.InvariantCulture);
		    if (level > value.Length)
		    {
			    return (before, before);
		    }
		    var stepTimeMul = bonusInfo.chargeBonus.stepTimeMul;
		    var stepTime = bonusInfo.chargeBonus.stepTime;
		    ActivateNode(level, ref bonusInfo);
		    var after = LevelUtility.GetChargeStepTime().ToString(CultureInfo.InvariantCulture);
		    bonusInfo.chargeBonus.stepTimeMul = stepTimeMul;
		    bonusInfo.chargeBonus.stepTime = stepTime;
		    return ($"{before}s", $"{after}s");
	    }

	    public string GetDisplayValue(int level)
	    {
		    if (level < 0) return "??";
		    if (level >= value.Length) level = value.Length - 1;

		    if (isMul)
			    return (value[level] * 100).ToString(CultureInfo.InvariantCulture);
		    
		    return value[level].ToString(CultureInfo.InvariantCulture);
	    }

	    public int MaxLevel => value.Length;

	    public enum BonusType
		{
			StepTime
		}
    }
}