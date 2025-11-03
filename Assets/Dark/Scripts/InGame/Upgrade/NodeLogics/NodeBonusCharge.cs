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
			StepTime
		}
    }
}