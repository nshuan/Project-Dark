using System;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeTowerCounter : INodeActivateLogic
    {
        public CounterType counterType;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            bonusInfo.unlockedTowerCounter ??= new Dictionary<CounterType, bool>();
            bonusInfo.unlockedTowerCounter[counterType] = true;
        }

        public string GetDisplayValue(int level)
        {
            return "";
        }

        public int MaxLevel => 1;
        
        public enum CounterType
        {
            Pierce,
            Area
        }
    }
}