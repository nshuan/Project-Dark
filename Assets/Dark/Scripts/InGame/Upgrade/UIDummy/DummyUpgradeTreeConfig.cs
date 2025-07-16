using System;
using System.Collections.Generic;
using InGame.Upgrade.UI;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame.Upgrade.UIDummy
{
    [CreateAssetMenu(menuName = "InGame/Upgrade/Upgrade Tree", fileName = "UpgradeTreeConfig")]
    public class DummyUpgradeTreeConfig : SerializedScriptableObject
    {
        [NonSerialized, OdinSerialize]
        public Dictionary<int, UpgradeNodeConfig> nodeMapById;
        
        [Space]
        public UIUpgradeTree treePrefab;

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

#if UNITY_EDITOR
        [Button]
        public void UpdateNodeMap()
        {
            if (treePrefab == null) return;

            nodeMapById = new Dictionary<int, UpgradeNodeConfig>();
            foreach (var pair in treePrefab.NodeConfigsMap)
            {
                nodeMapById.Add(pair.Key, pair.Value);
            }
        }

        private void OnValidate()
        {
            UpdateNodeMap();
        }
#endif
    }
}