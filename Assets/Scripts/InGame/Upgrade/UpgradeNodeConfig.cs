using System;
using System.Linq;
using InGame.Upgrade.UI;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame.Upgrade
{
    [CreateAssetMenu(menuName = "InGame/Upgrade/Node Config", fileName = "NodeConfig")]
    public class UpgradeNodeConfig : SerializedScriptableObject
    {
        public int nodeId;
        public string nodeName; // Name to display
        public UpgradeNodeConfig[] preRequire;
        public string description; // Description to display
        public int costType; // Type of resource needed to unlock this node
        public int[] costValue;
        public int levelNum = 1;
        public UIUpgradeNode nodePrefab;
        [NonSerialized, OdinSerialize] public INodeActivateLogic[] nodeLogic;
        
        public UpgradeNodeState State { get; set; }
        public bool Activated { get; set; }

        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (level <= 0 || level > levelNum) return;
            for (var i = 1; i <= level; i++)
            {
                foreach (var logic in nodeLogic)
                {
                    logic.ActivateNode(level, ref bonusInfo);
                }   
            }
        }

#if UNITY_EDITOR
        
        private void OnValidate()
        {
            if (preRequire.Contains(this))
            {
                var tempRequire = preRequire.ToList();
                tempRequire.Remove(this);
                preRequire = tempRequire.ToArray();
            }
        }

#endif
    }

    public enum UpgradeNodeState
    {
        Locked,
        CanUnlock,
        Unlocked
    }
}