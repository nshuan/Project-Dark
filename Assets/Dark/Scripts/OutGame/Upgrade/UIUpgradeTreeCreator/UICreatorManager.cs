using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Upgrade.UIUpgradeTreeCreator
{
    [Serializable]
    public class TreeDataStruct
    {
        public List<NodeDataStruct> nodes;
    }

    [Serializable]
    public class NodeDataStruct
    {
        public int id;
        public int idPrefab;
        public Vector2 position;
    }

    public enum NodeType
    {
        SkillNode,
        EffectNode,
        StatNode,
        StatNode2
    }
    
    public class UICreatorManager : SerializedMonoBehaviour
    {
        public UICreatorConfigLoader configLoader;
        [SerializeField] private Transform treeParent;
        [SerializeField] private Button btnNewTree;
        [SerializeField] private TMP_InputField input;
        [SerializeField] private Button[] btnCreateNode;
        [OdinSerialize, NonSerialized] private Dictionary<NodeType, UICreatorUpgradeNode> nodePrefabs;
        
        [Space]
        [Header("Tree Visual")]
        [SerializeField] private Transform lineParent;
        [SerializeField] private RectTransform linePrefab;
        
        private TreeDataStruct newTree;
        [ReadOnly, OdinSerialize, NonSerialized] private Dictionary<int, UICreatorUpgradeNode> nodesMap;
        private Dictionary<int, Dictionary<int, RectTransform>> linesMap;
        
        private void Awake()
        {
            btnNewTree.onClick.RemoveAllListeners();
            btnNewTree.onClick.AddListener(CreateNewTree);

            for (var index = 0; index < btnCreateNode.Length; index++)
            {
                var btn = btnCreateNode[index];
                var nodeType = (NodeType)index;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => CreateNewNode(nodeType));
            }
        }

        public void CreateNewTree()
        {
            // Destroy all nodes
            var children = new GameObject[treeParent.childCount];
            for (int i = 0; i < treeParent.childCount; i++)
            {
                children[i] = treeParent.GetChild(i).gameObject;
            }
            foreach (var child in children)
            {
                Destroy(child);
            }
            
            // Destroy all lines
            if (linesMap != null)
            {
                foreach (var pair1 in linesMap)
                {
                    if (pair1.Value != null)
                    {
                        foreach (var pair2 in pair1.Value)
                        {
                            if (pair2.Value != null)
                                Destroy(pair2.Value.gameObject);
                        }
                    }
                }
            }
            
            newTree = new TreeDataStruct() { nodes = new List<NodeDataStruct>() };
            nodesMap = new Dictionary<int, UICreatorUpgradeNode>();
            linesMap = new Dictionary<int, Dictionary<int, RectTransform>>();
        }

        public void CreateNewNode(NodeType nodeType)
        {
            if (newTree == null)
            {
                DebugUtility.LogError("Create a tree first!");
                return;
            }

            if (!int.TryParse(input.text, out var id))
            {
                DebugUtility.LogError("Id is invalid!");
                return;
            }

            var nodeConfig = configLoader.GetNodeConfig(id);
            if (nodeConfig == null)
            {
                DebugUtility.LogError("Config not found!");
                return;
            }

            if (nodesMap.ContainsKey(id))
            {
                DebugUtility.LogError("Node with id: " + id + " already exists!");
                return;
            }

            var prefab = nodePrefabs[nodeType];
            var node = Instantiate(prefab, treeParent);
            node.manager = this;
            node.config = nodeConfig;
            node.InitNode();
            nodesMap.Add(nodeConfig.nodeId, node);
        }

        public UICreatorUpgradeNode GetNodeById(int id)
        {
            return nodesMap.GetValueOrDefault(id);
        }
        
        public void ShowPreRequiredLine(int fromId, Vector2 from, int toId, Vector2 to)
        {
            RectTransform line = null;
            if (linesMap.ContainsKey(fromId))
            {
                if (linesMap[fromId].ContainsKey(toId))
                {
                    line = linesMap[fromId][toId];
                }
                else
                {
                    line = Instantiate(linePrefab, lineParent);
                    linesMap[fromId].Add(toId, line);
                }
            }
            else
            {
                line = Instantiate(linePrefab, lineParent);
                linesMap.Add(fromId, new Dictionary<int, RectTransform>());
                linesMap[fromId].Add(toId, line);
            }
            
            line.position = (from + to) / 2;
            line.sizeDelta = new Vector2(Vector2.Distance(from, to), 8f);
            var direction = (to - from).normalized;
            line.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }

        #region Select Node

        private UICreatorUpgradeNode selectingNode;

        public void SelectNode(UICreatorUpgradeNode node)
        {
            if (selectingNode != null) selectingNode.DeselectThis();
            selectingNode = node;
            node.SelectThis();
        }

        #endregion
    }
}