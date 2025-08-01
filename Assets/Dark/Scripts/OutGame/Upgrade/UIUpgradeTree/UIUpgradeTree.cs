using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeTree : SerializedMonoBehaviour
    {
        [SerializeField] private Transform lineParent;
        [SerializeField] private Transform linePrefab;

        [ReadOnly, OdinSerialize, NonSerialized] private Dictionary<int, UIUpgradeNode> nodesMap;

        public UIUpgradeNode GetNodeById(int id)
        {
            return nodesMap.GetValueOrDefault(id);
        }
        
        public void ShowPreRequiredLine(Vector2 from, Vector2 to)
        {
            var line = Instantiate(linePrefab, lineParent);
            line.position = (from + to) / 2;
            line.GetComponent<RectTransform>().sizeDelta = new Vector2(Vector2.Distance(from, to), 8f);
            var direction = (to - from).normalized;
            line.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
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