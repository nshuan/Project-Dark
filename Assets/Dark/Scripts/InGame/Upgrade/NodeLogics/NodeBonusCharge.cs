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

	    public string GetDisplayValue(int level)
	    {
		    if (level < 0) return "??";
		    if (level >= value.Length) level = value.Length - 1;

		    if (isMul)
			    return (value[level] * 100).ToString(CultureInfo.InvariantCulture);
		    
		    return value[level].ToString(CultureInfo.InvariantCulture);
	    }

	    public (string, string) GetBeforeAfterValue(int level)
	    {
		    var before = "";
		    var after = "";
		    if (level < 0) return ("", "");
		    if (level >= value.Length) level = value.Length - 1;
		    if (level == 0)
		    {
			    before = "0";
			    after = GetDisplayValue(level);
		    }
		    else
		    {
			    var sum = 0f;
			    for (var i = 0; i < level; i++)
				    sum += value[i];
			    if (isMul)
			    {
				    before = (sum * 100).ToString(CultureInfo.InvariantCulture);
				    after = ((sum + value[level]) * 100).ToString(CultureInfo.InvariantCulture);
			    }
			    else
			    {
				    before = sum.ToString(CultureInfo.InvariantCulture);
				    after = (sum + value[level]).ToString(CultureInfo.InvariantCulture);
			    }
		    }
		    
		    return (before, after);
	    }

	    public int MaxLevel => value.Length;

	    public enum BonusType
		{
			StepTime
		}
    }
}