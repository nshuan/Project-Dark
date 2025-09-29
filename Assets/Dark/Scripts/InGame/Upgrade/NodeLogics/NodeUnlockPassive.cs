using System;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Upgrade
{
	[Serializable]
    public class NodeUnlockPassive : INodeActivateLogic
    {
	    public PassiveTriggerType triggerType;
	    public PassiveType passiveType;
	    
	    public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
	    {
		    if (!bonusInfo.passiveMapByTriggerType.ContainsKey(triggerType))
			    bonusInfo.passiveMapByTriggerType.Add(triggerType, new List<PassiveType>());
		    
		    if (!bonusInfo.passiveMapByTriggerType[triggerType].Contains(passiveType))
			    bonusInfo.passiveMapByTriggerType[triggerType].Add(passiveType);
	    }

	    public string GetDisplayValue(int level)
	    {
		    return "";
	    }
    }
}