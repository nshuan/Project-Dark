using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace InGame.Upgrade.UI
{
    public class UIUpgradeTree : SerializedMonoBehaviour
    {
        [field: ReadOnly, NonSerialized, OdinSerialize] public Dictionary<int, UIUpgradeNode> NodesMap { get; set; }
        [field: ReadOnly, NonSerialized, OdinSerialize] public Dictionary<int, UpgradeNodeConfig> NodeConfigsMap { get; set; }
        
        private void OnEnable()
        {
            foreach (var pair in NodesMap)
            {
                pair.Value.UpdateUI(
                    UpgradeManager.Instance.GetData(pair.Key),
                    NodeConfigsMap[pair.Key],
                    () => RefreshNode(pair.Key)
                    );
            }
        }

        private void RefreshNode(int nodeId)
        {
            if (NodesMap.TryGetValue(nodeId, out var uiNode))
            {
                uiNode.UpdateUI(
                    UpgradeManager.Instance.GetData(nodeId),
                    NodeConfigsMap[nodeId],
                    () => RefreshNode(nodeId)
                );
            }
        }
    }
}