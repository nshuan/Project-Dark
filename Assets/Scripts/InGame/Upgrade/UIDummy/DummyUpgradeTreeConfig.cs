using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame.Upgrade.UIDummy
{
    [CreateAssetMenu(menuName = "InGame/Upgrade/Upgrade Tree", fileName = "UpgradeTreeConfig")]
    public class DummyUpgradeTreeConfig : SerializedScriptableObject
    {
        [field: ReadOnly, NonSerialized, OdinSerialize]
        public Dictionary<int, UpgradeNodeConfig> nodeMapById;
        
        [Space]
        [ReadOnly]
        public DummyUpgradeTree treePrefab;

        public void ActivateTree(List<UpgradeNodeData> nodeData, ref UpgradeBonusInfo bonusInfo)
        {
            foreach (var node in nodeData)
            {
                if (nodeMapById.TryGetValue(node.id, out var nodeConfig))
                {
                    nodeConfig.ActivateNode(node.level, ref bonusInfo);
                }
            }
        }
    }
}