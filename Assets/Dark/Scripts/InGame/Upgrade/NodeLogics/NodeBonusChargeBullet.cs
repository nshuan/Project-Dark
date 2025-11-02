using System;
using System.Globalization;

namespace InGame.Upgrade
{
	[Serializable]
    public class NodeBonusChargeBullet : INodeActivateLogic
    {
	    public float[] value;
	    
	    public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
	    {
		    if (level <= 0 || level > value.Length) return;
		    
		    bonusInfo.chargeBonus.bulletMaxStep += (int)value[level - 1];
	    }

	    public string GetDisplayValue(int level)
	    {
		    if (level < 0) return "??";
		    
		    if (level >= value.Length) level = value.Length - 1;
		    
		    return value[level].ToString(CultureInfo.InvariantCulture);
	    }

	    public int MaxLevel => value.Length;

	    public enum BonusType
		{
			DamePerStep,
			MaxDameTime,
			SizePerStep,
			MaxSizeTime,
			RangePerStep,
			MaxRangeTime,
			BulletPerStep,
			BulletMaxTime
		}
    }

}