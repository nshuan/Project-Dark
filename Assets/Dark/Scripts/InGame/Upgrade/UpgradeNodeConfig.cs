using System;
using System.Linq;
using Economic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame.Upgrade
{
    [CreateAssetMenu(menuName = "Dark/Upgrade/Upgrade Node Config", fileName = "UpgradeNodeConfig")]
    public class UpgradeNodeConfig : SerializedScriptableObject
    {
        public int nodeId;
        public string nodeName; // Name to display
        public bool hideLevelInNode;
        public string description; // Description to display
        public UpgradeNodeCostInfo[] costInfo; 
        [NonSerialized, OdinSerialize] public INodeActivateLogic[] nodeLogic;
        
        public int MaxLevel
        {
            get
            {
                if (nodeLogic == null || nodeLogic.Length == 0) return 1;
                return nodeLogic.Max((logic) => logic.MaxLevel);
            }
        }

        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (nodeLogic == null) return;
            if (level <= 0 || level > MaxLevel) return;
            for (var i = 1; i <= level; i++)
            {
                foreach (var logic in nodeLogic)
                {
                    logic.ActivateNode(i, ref bonusInfo);
                }   
            }
        }
        
        public void ActivateLevel(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (nodeLogic == null) return;
            if (level <= 0 || level > MaxLevel) return;
            foreach (var logic in nodeLogic)
            {
                logic.ActivateNode(level, ref bonusInfo);
            }   
        }
    }

    [Serializable]
    public class UpgradeNodeCostInfo
    {
        public WealthType costType; // Type of resource needed to unlock this node
    }
}