using System;
using UnityEngine;

namespace InGame.Upgrade
{
    [Serializable]
    public class NodeTowerCounter : INodeActivateLogic
    {
        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            bonusInfo.unlockedTowerCounter = true;
        }

        public string GetDisplayValue(int level)
        {
            return "";
        }

        public int MaxLevel => 1;
    }
}