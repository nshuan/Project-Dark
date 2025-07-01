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
        [ReadOnly, NonSerialized, OdinSerialize] private Dictionary<int, UpgradeNodeConfig> upgradeNodesMapByIndex = new Dictionary<int, UpgradeNodeConfig>();

        private void OnValidate()
        {
            if (upgradeNodes == null || upgradeNodes.Count == 0)
            {
                upgradeNodesMapByIndex.Clear();
                upgradeNodesMapByIndex = null;
                return;
            }

            upgradeNodesMapByIndex = upgradeNodes.ToDictionary((node) => node.nodeIndex, (node) => node);
        }
    }
}