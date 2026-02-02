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
        public bool lockOnDemo;
        public bool hideLevelInNode;
        public string description; // Description to display
        public float vestigeCostRatio; // cost will multiply to this value
        public bool dynamicVestige; // is this node using dynamic vestige?
        public bool dynamicEchoes; // is this node using dynamic echoes?
        public UpgradeNodeCostInfo[] costInfo; 
        [NonSerialized, OdinSerialize] public INodeActivateLogicV2[] nodeLogic;

        public UpgradeGroupIdInfo[] groupId;
        
        public int MaxLevel
        {
            get
            {
                if (nodeLogic == null || nodeLogic.Length == 0) return 1;
                return nodeLogic.Max((logic) => logic.MaxLevel);
            }
        }

        public void ActivateNode(int level, ref UpgradeBonusInfoV2 bonusInfo)
        {
            if (nodeLogic == null) return;
            if (level <= 0 || level > MaxLevel) return;
            UpgradeManager.Instance.RefreshGroupUnlockOrder();
            for (var i = 1; i <= level; i++)
            {
                foreach (var logic in nodeLogic)
                {
                    if (logic is INodeDynamicBonusValueV2 { IsDynamic: true } dynamicLogic)
                        dynamicLogic.OverrideBonusValue(groupId.Min((info) =>
                            UpgradeManager.Instance.GetGroupUnlockOrder(info.groupId, false)));
                    logic.ActivateNode(i, ref bonusInfo);
                }   
            }
        }
        
        public void ActivateLevel(int level, ref UpgradeBonusInfoV2 bonusInfo)
        {
            if (nodeLogic == null) return;
            if (level <= 0 || level > MaxLevel) return;
            foreach (var logic in nodeLogic)
            {
                if (logic is INodeDynamicBonusValueV2 { IsDynamic: true } dynamicLogic)
                    dynamicLogic.OverrideBonusValue(groupId.Min((info) =>
                        UpgradeManager.Instance.GetGroupUnlockOrder(info.groupId, false)));
                logic.ActivateNode(level, ref bonusInfo);
            }   
        }
    }

    [Serializable]
    public class UpgradeNodeCostInfo
    {
        public WealthType costType; // Type of resource needed to unlock this node
        public int[] costValue;
    }

    [Serializable]
    public class UpgradeGroupIdInfo
    {
        public int groupId;
        public bool isLockNode;
    }
}