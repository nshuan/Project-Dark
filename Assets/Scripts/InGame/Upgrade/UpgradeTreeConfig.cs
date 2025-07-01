using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame.Upgrade
{
    [CreateAssetMenu(menuName = "InGame/Upgrade/Upgrade Tree", fileName = "UpgradeTreeConfig")]
    public class UpgradeTreeConfig : SerializedScriptableObject
    {
        public List<UpgradeNodeConfig> upgradeNodes = new List<UpgradeNodeConfig>();
        [ReadOnly, NonSerialized, OdinSerialize] private Dictionary<int, UpgradeNodeConfig[]> upgradeNodesMapByLevel = new Dictionary<int, UpgradeNodeConfig[]>();
        
        public void Activate()
        {
            foreach (var pair in upgradeNodesMapByLevel)
            {
                foreach (var node in pair.Value)
                {
                    if (node.Activated) continue;
                    node.ActivateNode();
                }
            }
        }

        private void OnValidate()
        {
            if (upgradeNodes == null || upgradeNodes.Count == 0)
            {
                upgradeNodesMapByLevel.Clear();
                upgradeNodesMapByLevel = null;
                return;
            }

            upgradeNodesMapByLevel = upgradeNodes
                .GroupBy((node) => node.levelInTree)
                .ToDictionary(node => node.Key, node => node.ToArray());
        }
    }
}