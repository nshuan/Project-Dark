using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeTree : MonoBehaviour
    {
        [SerializeField] private Transform lineParent;
        [SerializeField] private Transform linePrefab;
        
        
        private Dictionary<int, UIUpgradeNode> nodesMap;

        public UIUpgradeNode GetNodeById(int id)
        {
            return nodesMap.GetValueOrDefault(id);
        }
        
        public void ShowPreRequiredLine(Vector2 from, Vector2 to)
        {
            
        }
        
        [Button]
        public void ValidateNodes()
        {
            nodesMap = new Dictionary<int, UIUpgradeNode>();
            var nodes = GetComponentsInChildren<UIUpgradeNode>();
            foreach (var node in nodes)
            {
                node.treeRef = this;
                nodesMap.TryAdd(node.config.nodeId, node);
            }
        }
    }
}