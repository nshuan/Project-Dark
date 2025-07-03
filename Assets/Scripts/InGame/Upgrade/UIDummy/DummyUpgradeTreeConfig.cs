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

        public void ActivateTree(List<int> activatedNodes, ref UpgradeBonusInfo bonusInfo)
        {
            foreach (var nodeId in activatedNodes)
            {
                if (nodeMapById.TryGetValue(nodeId, out var nodeConfig))
                {
                    nodeConfig.ActivateNode(ref bonusInfo);
                }
            }
        }
    }
}