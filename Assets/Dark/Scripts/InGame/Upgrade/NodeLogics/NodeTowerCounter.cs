using System;
using UnityEngine;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeTowerCounter : INodeActivateLogic
    {
        [SerializeField] private TowerCounterConfig counterConfig;
        [SerializeField] private string bonusDescription;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            bonusInfo.unlockedTowerCounter = counterConfig;
        }

        public string GetDescription(int level)
        {
            return bonusDescription;
        }
    }
}