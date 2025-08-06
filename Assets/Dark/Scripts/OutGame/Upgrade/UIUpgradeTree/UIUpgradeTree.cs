using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeTree : SerializedMonoBehaviour
    {
        [SerializeField] private Transform lineParent;
        [SerializeField] private Transform linePrefab;

        [ReadOnly, OdinSerialize, NonSerialized] private Dictionary<int, UIUpgradeNode> nodesMap;
        [ReadOnly, OdinSerialize, NonSerialized] private Dictionary<int, List<UIUpgradeNode>> nodeChildrenMap;

        [Space] [Header("UI")] 
        [SerializeField] private Button btnDeselectAll;
        
        public UIUpgradeNode GetNodeById(int id)
        {
            return nodesMap.GetValueOrDefault(id);
        }

        public void UpdateChildren(int id)
        {
            if (nodeChildrenMap.TryGetValue(id, out var children))
            {
                foreach (var childNode in children)
                {
                    childNode.UpdateUI();
                }
            }
        }

        private void Awake()
        {
            btnDeselectAll.onClick.RemoveAllListeners();
            btnDeselectAll.onClick.AddListener(() =>
            {
                UIUpgradeNodeInfoPreview.Instance.Hide(true);
            });
        }

#if UNITY_EDITOR
        [Button]
        public void ValidateNodes()
        {
            // Destroy all lines
            var children = new GameObject[lineParent.childCount];
            for (int i = 0; i < lineParent.childCount; i++)
            {
                children[i] = lineParent.GetChild(i).gameObject;
            }
            foreach (var child in children)
            {
                DestroyImmediate(child);
            }
            
            nodesMap = new Dictionary<int, UIUpgradeNode>();
            nodeChildrenMap = new Dictionary<int, List<UIUpgradeNode>>();
            var nodes = GetComponentsInChildren<UIUpgradeNode>();
            foreach (var node in nodes)
            {
                nodesMap.TryAdd(node.config.nodeId, node);
                node.treeRef = this;
                node.preRequireLines = new List<UIUpgradeLineInfo>();
                
                if (node.config.preRequire == null || node.config.preRequire.Length == 0) continue;
                foreach (var preRequireConfig in node.config.preRequire)
                {
                    var preRequireNode = GetNodeById(preRequireConfig.nodeId);
                    if (preRequireNode == null) continue;
                    node.preRequireLines.Add(new UIUpgradeLineInfo()
                    {
                        preRequireId = preRequireConfig.nodeId,
                        line = ShowPreRequiredLine(node.transform.position, node.lineAnchorOffsetRadius, preRequireNode.transform.position, preRequireNode.lineAnchorOffsetRadius)
                    });
                    
                    // Add child map
                    nodeChildrenMap.TryAdd(preRequireConfig.nodeId, new List<UIUpgradeNode>());
                    if (!nodeChildrenMap[preRequireConfig.nodeId].Contains(node))
                        nodeChildrenMap[preRequireConfig.nodeId].Add(node);
                }
                
                EditorUtility.SetDirty(node);
            }
            
            EditorUtility.SetDirty(this);
        }
        
        private UIUpgradeLine ShowPreRequiredLine(Vector2 from, float fromOffsetRadius, Vector2 to, float toOffsetRadius)
        {
            var line = ((GameObject)PrefabUtility.InstantiatePrefab(linePrefab.gameObject)).transform;
            line.SetParent(lineParent);
            var direction = (to - from).normalized;
            from = from + direction * fromOffsetRadius;
            to = to - direction * toOffsetRadius;
            line.position = (from + to) / 2;
            line.GetComponent<RectTransform>().sizeDelta = new Vector2(10f, Vector2.Distance(from, to));
            line.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90);

            return line.GetComponent<UIUpgradeLine>();
        }
#endif
    }
}
