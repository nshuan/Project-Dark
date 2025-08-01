using System;
using UnityEngine;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeTowerCounter : INodeActivateLogic
    {
        [SerializeField] private TowerCounterConfig counterConfig;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            bonusInfo.unlockedTowerCounter = counterConfig;
        }

        public string GetDescription(int level)
        {
            var result = "";
            return result;
        }
    }
}