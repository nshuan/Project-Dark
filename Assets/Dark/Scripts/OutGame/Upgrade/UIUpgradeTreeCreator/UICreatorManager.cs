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
        private Dictionary<int, List<UICreatorUpgradeNode>> nodeChildMap;
        private Dictionary<int, List<UICreatorUpgradeNode>> nodeParentMap;
        
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
            
            btnDeselectAll.onClick.RemoveAllListeners();
            btnDeselectAll.onClick.AddListener(DeselectAll);
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
            nodeChildMap =  new Dictionary<int, List<UICreatorUpgradeNode>>();
            nodeParentMap = new Dictionary<int, List<UICreatorUpgradeNode>>();
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
            
            nodesMap.Add(nodeConfig.nodeId, node);
            nodeChildMap.TryAdd(nodeConfig.nodeId, new List<UICreatorUpgradeNode>());
            nodeParentMap.TryAdd(nodeConfig.nodeId, new List<UICreatorUpgradeNode>());
            foreach (var preConfig in nodeConfig.preRequire)
            {
                nodeChildMap.TryAdd(preConfig.nodeId, new List<UICreatorUpgradeNode>());
                if (!nodeChildMap[preConfig.nodeId].Contains(node))
                    nodeChildMap[preConfig.nodeId].Add(node);
                if (nodesMap.ContainsKey(preConfig.nodeId))
                    nodeParentMap[nodeConfig.nodeId].Add(nodesMap[preConfig.nodeId]);
            }

            foreach (var childNode in nodeChildMap[nodeConfig.nodeId])
            {
                nodeParentMap.TryAdd(childNode.config.nodeId, new List<UICreatorUpgradeNode>());
                if (!nodeParentMap[childNode.config.nodeId].Contains(node))
                {
                    nodeParentMap[childNode.config.nodeId].Add(node);
                    ShowPreRequiredLine(nodeConfig.nodeId, node.transform.position, childNode.config.nodeId, childNode.transform.position);
                }
            }

            foreach (var childNode in nodeChildMap[nodeConfig.nodeId])
            {
                ShowPreRequiredLine(nodeConfig.nodeId, node.transform.position, childNode.config.nodeId, childNode.transform.position);
            }

            foreach (var parentNode in nodeParentMap[nodeConfig.nodeId])
            {
                ShowPreRequiredLine(parentNode.config.nodeId, parentNode.transform.position, nodeConfig.nodeId, node.transform.position);
            }
            
            UpdateLine(nodeConfig.nodeId);
            
            node.InitNode();
        }

        public UICreatorUpgradeNode GetNodeById(int id)
        {
            return nodesMap.GetValueOrDefault(id);
        }
        
        public void ShowPreRequiredLine(int fromId, Vector2 from, int toId, Vector2 to)
        {
            RectTransform line;
            if (linesMap.ContainsKey(fromId))
            {
                if (!linesMap[fromId].ContainsKey(toId))
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
        }

        public void UpdateLine(int nodeId)
        {
            var from = new Vector2();
            var to = new Vector2();
            var direction = new Vector2();
            foreach (var childNode in nodeChildMap[nodeId])
            {
                from.x = nodesMap[nodeId].transform.position.x;
                from.y = nodesMap[nodeId].transform.position.y;
                to.x = childNode.transform.position.x;
                to.y = childNode.transform.position.y;
                direction.x = to.x - from.x;
                direction.y = to.y - from.y;
                if (direction.magnitude > 0.05f)
                {
                    direction = direction / direction.magnitude;
                    linesMap[nodeId][childNode.config.nodeId].position = (from + to) / 2;
                    linesMap[nodeId][childNode.config.nodeId].sizeDelta = new Vector2(Vector2.Distance(from, to), 8f);
                    linesMap[nodeId][childNode.config.nodeId].rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
                }
            }

            foreach (var parentNode in nodeParentMap[nodeId])
            {
                from.x = parentNode.transform.position.x;
                from.y = parentNode.transform.position.y;
                to.x = nodesMap[nodeId].transform.position.x;
                to.y = nodesMap[nodeId].transform.position.y;
                direction.x = to.x - from.x;
                direction.y = to.y - from.y;
                if (direction.magnitude > 0.05f)
                {
                    direction = direction / direction.magnitude;
                    linesMap[parentNode.config.nodeId][nodeId].position = (from + to) / 2;
                    linesMap[parentNode.config.nodeId][nodeId].sizeDelta = new Vector2(Vector2.Distance(from, to), 8f);
                    linesMap[parentNode.config.nodeId][nodeId].rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
                }
            }
        }

        #region Select Node

        [Space]
        [Header("Select Node")]
        [SerializeField] private Button btnDeselectAll;
        
        private UICreatorUpgradeNode selectingNode;

        public void SelectNode(UICreatorUpgradeNode node)
        {
            if (selectingNode != null) selectingNode.DeselectThis();
            selectingNode = node;
            node.SelectThis();
        }

        public void DeselectAll()
        {
            if  (selectingNode != null) selectingNode.DeselectThis();
            selectingNode = null;
        }

        #endregion
    }
}