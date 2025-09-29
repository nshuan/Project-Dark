using System;
using UnityEngine;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeTowerCounter : INodeActivateLogic
    {
        [SerializeField] private string bonusDescription;
        
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            bonusInfo.unlockedTowerCounter = true;
        }

        public string GetDescription(int level)
        {
            return bonusDescription;
        }
    }
}