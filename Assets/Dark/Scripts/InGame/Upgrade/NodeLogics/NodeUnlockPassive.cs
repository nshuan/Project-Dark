using System;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Upgrade
{
	[Serializable]
    public class NodeUnlockPassive : INodeActivateLogic
    {
	    [SerializeField] private PassiveTriggerType triggerType;
	    [SerializeField] private PassiveType passiveType;
	    
	    public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
	    {
		    if (!bonusInfo.effectsMapByTriggerType.ContainsKey(triggerType))
			    bonusInfo.effectsMapByTriggerType.Add(triggerType, new List<PassiveType>());
		    
		    if (!bonusInfo.effectsMapByTriggerType[triggerType].Contains(passiveType))
			    bonusInfo.effectsMapByTriggerType[triggerType].Add(passiveType);
	    }

	    public string GetDescription(int level)
	    {
		    var result = "";
		    return result;
	    }
    }
}