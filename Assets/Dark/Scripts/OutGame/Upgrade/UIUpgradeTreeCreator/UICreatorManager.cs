using System;
using System.Collections.Generic;
using System.Linq;
using InGame.Upgrade;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEditor;
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
        public int idType;
        public int idPrefab;
        public Vector2 position;
    }

    [Serializable]
    public class NodeButtonInfo
    {
        public Button button;
        public UICreatorUpgradeNode nodePrefab;
    }

    public enum NodeType
    {
        ClassNode,
        SkillNode,
        EffectNode,
        StatNode,
        StatNode2
    }
    
    public class UICreatorManager : SerializedMonoBehaviour
    {
        public UICreatorConfigLoader configLoader;
        [SerializeField] private Transform treeParent;
        [SerializeField] private GameObject groupNodeModeButtons;
        [SerializeField] private TMP_InputField input;
        [SerializeField] private TMP_InputField inputTreeName;
        [SerializeField] private Button btnNewTree;
        [SerializeField] private Button btnDeleteNode;
        [SerializeField] private Button btnChangeMode;
        [SerializeField] private TextMeshProUGUI txtMode;
        [SerializeField] private Button[] btnToggleNodes;
        [SerializeField] private Transform[] nodeButtonGroups;
        [OdinSerialize, NonSerialized] private Dictionary<NodeType, List<NodeButtonInfo>> btnInfo;

        [Space] [Header("Create and Load tree")] 
        [SerializeField] private GameObject groupBtnLoadTree;
        [SerializeField] private Button btnToggleLoadTree;
        [SerializeField] private Button btnCloseLoadTree;
        [SerializeField] private Button btnLoadTree;
        [SerializeField] private TMP_InputField inputTreeNameToLoad;
        
        [Space]
        [Header("Tree Visual")]
        [SerializeField] private Transform lineParent;
        [SerializeField] private RectTransform linePrefab;
        
        private TreeDataStruct newTree;
        [ReadOnly, OdinSerialize, NonSerialized] private Dictionary<int, UICreatorUpgradeNode> nodesMap;
        private Dictionary<int, Dictionary<int, RectTransform>> linesMap;
        private Dictionary<int, List<UICreatorUpgradeNode>> nodeChildMap;
        private Dictionary<int, List<UICreatorUpgradeNode>> nodeParentMap;

        private bool isLinkMode;
        
        private void Awake()
        {
            btnNewTree.onClick.RemoveAllListeners();
            btnNewTree.onClick.AddListener(CreateNewTree);

            for (var i = 0; i < btnToggleNodes.Length; i++)
            {
                var index = i;
                btnToggleNodes[i].onClick.RemoveAllListeners();
                btnToggleNodes[i].onClick.AddListener(() =>
                {
                    HideAllNodeGroup();
                    ToggleNodeGroup(index);
                });
            }
            
            for (var index = 0; index < btnInfo.Count; index++)
            {
                var nodeType = (NodeType)index;
                var infoList = btnInfo[nodeType];
                for (var i = 0; i < infoList.Count; i++)
                {
                    var info = infoList[i];
                    var pIndex = i;
                    info.button.onClick.RemoveAllListeners();
                    info.button.onClick.AddListener(() =>
                    {
                        if (!int.TryParse(input.text, out var id))
                        {
                            DebugUtility.LogError("Id is invalid!");
                            return;
                        }
                        
                        CreateNewNode(nodeType, pIndex, id);
                    });
                }
            }
            
            btnDeselectAll.onClick.RemoveAllListeners();
            btnDeselectAll.onClick.AddListener(DeselectAll);
            btnDeleteNode.onClick.RemoveAllListeners();
            btnDeleteNode.onClick.AddListener(DeleteNode);
            btnChangeMode.onClick.RemoveAllListeners();
            btnChangeMode.onClick.AddListener(ChangeMode);
            
            btnToggleLoadTree.onClick.RemoveAllListeners();
            btnToggleLoadTree.onClick.AddListener(() => groupBtnLoadTree.SetActive(true));
            btnCloseLoadTree.onClick.RemoveAllListeners();
            btnCloseLoadTree.onClick.AddListener(() => groupBtnLoadTree.SetActive(false));
            btnLoadTree.onClick.RemoveAllListeners();
            btnLoadTree.onClick.AddListener(LoadTree);
        }

        public void ToggleNodeGroup(int index)
        {
            nodeButtonGroups[index].gameObject.SetActive(!nodeButtonGroups[index].gameObject.activeInHierarchy);
        }

        public void HideAllNodeGroup()
        {
            foreach (var group in nodeButtonGroups)
            {
                group.gameObject.SetActive(false);
            }
        }

        public void CreateNewTree()
        {
            groupBtnLoadTree.SetActive(false);
            HideAllNodeGroup();
            UICreatorNodeInfoPreview.Instance.Hide();
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
            selectingNode = null;
        }

        public void CreateNewNode(NodeType nodeType, int prefabIndex, int id)
        {
            HideAllNodeGroup();
            UICreatorNodeInfoPreview.Instance.Hide();
            if (newTree == null)
            {
                DebugUtility.LogError("Create a tree first!");
                return;
            }

            var nodeConfig = configLoader.GetNodeConfig(id);
            if (nodeConfig == null)
            {
#if UNITY_EDITOR
                configLoader.GetConfigsFromPath();
                nodeConfig = configLoader.GetNodeConfig(id);
#endif
                if (nodeConfig == null)
                {
                    DebugUtility.LogError("Config not found!");
                    return;
                }
            }

            if (nodesMap.ContainsKey(id))
            {
                DebugUtility.LogError("Node with id: " + id + " already exists!");
                return;
            }

            var prefab = btnInfo[nodeType][prefabIndex].nodePrefab;
            var node = Instantiate(prefab, treeParent);
            node.manager = this;
            node.config = nodeConfig;
            node.CreatorNodeType = nodeType;
            node.PrefabIndex = prefabIndex;
            
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
                    var direction = (childNode.transform.position - node.transform.position).normalized;
                    ShowPreRequiredLine(nodeConfig.nodeId, 
                        node.transform.position + direction * node.lineAnchorOffsetRadius, 
                        childNode.config.nodeId, 
                        childNode.transform.position - direction * childNode.lineAnchorOffsetRadius);
                }
            }

            foreach (var childNode in nodeChildMap[nodeConfig.nodeId])
            {
                var direction = (childNode.transform.position - node.transform.position).normalized;
                ShowPreRequiredLine(nodeConfig.nodeId, 
                    node.transform.position + direction * node.lineAnchorOffsetRadius, 
                    childNode.config.nodeId, 
                    childNode.transform.position - direction * childNode.lineAnchorOffsetRadius);
            }

            foreach (var parentNode in nodeParentMap[nodeConfig.nodeId])
            {
                var direction = (node.transform.position - parentNode.transform.position).normalized; 
                ShowPreRequiredLine(parentNode.config.nodeId, 
                    parentNode.transform.position + direction * parentNode.lineAnchorOffsetRadius,
                    nodeConfig.nodeId, 
                    node.transform.position - direction * node.lineAnchorOffsetRadius);
            }
            
            UpdateLine(nodeConfig.nodeId);
            
            node.InitNode();
        }

        public void DeleteNode()
        {
            HideAllNodeGroup();
            if (selectingNode == null)
            {
                DebugUtility.LogWarning("You are not selecting any node!");
                return;
            }

            // Delete lines
            if (linesMap.ContainsKey(selectingNode.config.nodeId))
            {
                foreach (var pair in linesMap[selectingNode.config.nodeId])
                {
                    Destroy(pair.Value.gameObject);
                }
                linesMap.Remove(selectingNode.config.nodeId);
            }
            foreach (var pair in linesMap)
            {
                if (pair.Value != null && pair.Value.ContainsKey(selectingNode.config.nodeId))
                {
                    Destroy(pair.Value[selectingNode.config.nodeId].gameObject);
                    pair.Value.Remove(selectingNode.config.nodeId);
                }
            }
            
            // Delete node references
            if (nodeChildMap.ContainsKey(selectingNode.config.nodeId))
            {
                foreach (var child in nodeChildMap[selectingNode.config.nodeId])
                {
                    if (nodeParentMap.ContainsKey(child.config.nodeId) &&
                        nodeParentMap[child.config.nodeId].Contains(selectingNode))
                        nodeParentMap[child.config.nodeId].Remove(selectingNode);
                }
                nodeChildMap.Remove(selectingNode.config.nodeId);
            }
            if (nodeParentMap.ContainsKey(selectingNode.config.nodeId))
            {
                foreach (var parent in nodeParentMap[selectingNode.config.nodeId])
                {
                    if (nodeChildMap.ContainsKey(parent.config.nodeId) &&
                        nodeChildMap[parent.config.nodeId].Contains(selectingNode))
                        nodeChildMap[parent.config.nodeId].Remove(selectingNode);
                }
                nodeParentMap.Remove(selectingNode.config.nodeId);
            }
            if (nodesMap.ContainsKey(selectingNode.config.nodeId))
                nodesMap.Remove(selectingNode.config.nodeId);
            
            Destroy(selectingNode.gameObject);
            selectingNode = null;
            btnDeleteNode.gameObject.SetActive(false);
            UICreatorNodeInfoPreview.Instance.Hide();
        }

        public void ChangeMode()
        {
            HideAllNodeGroup();
            DeselectAll();
            isLinkMode = !isLinkMode;
            txtMode.SetText(isLinkMode ? "Link Mode" : "Node Mode");
            groupNodeModeButtons.SetActive(!isLinkMode);
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
                from.x = nodesMap[nodeId].transform.localPosition.x;
                from.y = nodesMap[nodeId].transform.localPosition.y;
                to.x = childNode.transform.localPosition.x;
                to.y = childNode.transform.localPosition.y;
                direction.x = to.x - from.x;
                direction.y = to.y - from.y;
                if (direction.magnitude > 0.05f)
                {
                    direction = direction / direction.magnitude;
                    from = from + direction * nodesMap[nodeId].lineAnchorOffsetRadius;
                    to = to - direction * childNode.lineAnchorOffsetRadius;
                    linesMap[nodeId][childNode.config.nodeId].localPosition = (from + to) / 2;
                    linesMap[nodeId][childNode.config.nodeId].sizeDelta = new Vector2(Vector2.Distance(from, to), 8f);
                    linesMap[nodeId][childNode.config.nodeId].rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
                }
            }

            foreach (var parentNode in nodeParentMap[nodeId])
            {
                from.x = parentNode.transform.localPosition.x;
                from.y = parentNode.transform.localPosition.y;
                to.x = nodesMap[nodeId].transform.localPosition.x;
                to.y = nodesMap[nodeId].transform.localPosition.y;
                direction.x = to.x - from.x;
                direction.y = to.y - from.y;
                if (direction.magnitude > 0.05f)
                {
                    direction = direction / direction.magnitude;
                    from = from + direction * parentNode.lineAnchorOffsetRadius;
                    to = to - direction * nodesMap[nodeId].lineAnchorOffsetRadius;
                    linesMap[parentNode.config.nodeId][nodeId].localPosition = (from + to) / 2;
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
            HideAllNodeGroup();
            if (isLinkMode)
            {
                SelectNodeLink(node);
                return;
            }
            
            if (selectingNode != null) selectingNode.DeselectThis();
            selectingNode = node;
            btnDeleteNode.gameObject.SetActive(true);
            node.SelectThis();
            UICreatorNodeInfoPreview.Instance.UpdateUI(null, node.config);
            UICreatorNodeInfoPreview.Instance.Show(node.transform.position, new Vector2(node.lineAnchorOffsetRadius, 0f));
        }

        public void DeselectAll()
        {
            HideAllNodeGroup();
            if  (selectingNode != null) selectingNode.DeselectThis();
            selectingNode = null;
            btnDeleteNode.gameObject.SetActive(false);
            UICreatorNodeInfoPreview.Instance.Hide();
        }

        #endregion

        #region Edit node links

        public void SelectNodeLink(UICreatorUpgradeNode node)
        {
            if (selectingNode == null)
            {
                selectingNode = node;
                node.SelectThis();
            }
            else
            {
                // Add or remove link 
                if (selectingNode.config.preRequire.Contains(node.config))
                {
                    // Update data
                    var newPreRequire = selectingNode.config.preRequire.Where((nodeConfig) => nodeConfig != node.config)
                        .ToArray();
                    selectingNode.config.preRequire = newPreRequire;
#if UNITY_EDITOR
                    EditorUtility.SetDirty(selectingNode.config);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    Debug.Log("ScriptableObject changes saved to asset.");
#endif
                    
                    if (linesMap.ContainsKey(node.config.nodeId))
                    {
                        if (linesMap[node.config.nodeId].ContainsKey(selectingNode.config.nodeId))
                        {
                            Destroy(linesMap[node.config.nodeId][selectingNode.config.nodeId].gameObject);
                            linesMap[node.config.nodeId].Remove(selectingNode.config.nodeId);
                        }
                    }
                    if (nodeParentMap.ContainsKey(selectingNode.config.nodeId))
                    {
                        if (nodeParentMap[selectingNode.config.nodeId].Contains(node))
                            nodeParentMap[selectingNode.config.nodeId].Remove(node);
                    }
                    if (nodeChildMap.ContainsKey(node.config.nodeId))
                    {
                        if (nodeChildMap[node.config.nodeId].Contains(selectingNode))
                            nodeChildMap[node.config.nodeId].Remove(selectingNode);
                    }
                }
                else
                {
                    // Check loop
                    // TODO check loop
                    
                    // Update data
                    var newPreRequire = selectingNode.config.preRequire.ToList();
                    newPreRequire.Add(node.config);
                    selectingNode.config.preRequire = newPreRequire.ToArray();
#if UNITY_EDITOR
                    EditorUtility.SetDirty(selectingNode.config);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    Debug.Log("ScriptableObject changes saved to asset.");
#endif

                    linesMap.TryAdd(node.config.nodeId, new Dictionary<int, RectTransform>());
                    if (!linesMap[node.config.nodeId].ContainsKey(selectingNode.config.nodeId))
                    {
                        var line = Instantiate(linePrefab, lineParent);
                        linesMap[node.config.nodeId].Add(selectingNode.config.nodeId, line);
                    }
                    nodeParentMap.TryAdd(selectingNode.config.nodeId, new List<UICreatorUpgradeNode>());
                    if (!nodeParentMap[selectingNode.config.nodeId].Contains(node))
                        nodeParentMap[selectingNode.config.nodeId].Add(node);
                    nodeChildMap.TryAdd(node.config.nodeId, new List<UICreatorUpgradeNode>());
                    if (!nodeChildMap[node.config.nodeId].Contains(selectingNode))
                        nodeChildMap[node.config.nodeId].Add(selectingNode);
                    
                    UpdateLine(node.config.nodeId);
                }
                
                selectingNode.DeselectThis();
                selectingNode = null;
            }
        }

        #endregion

        #region Load from saved JSON

        public void LoadTree()
        {
            var name = inputTreeNameToLoad.text;
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Tree name can not be empty!");
                return;
            }

            if (!jsonConverter.Exist(name))
            {
                Debug.LogError($"No tree with name {name}!");
                return;
            }
            
            CreateNewTree();
            newTree = jsonConverter.LoadJson(name);

            foreach (var nodeData in newTree.nodes)
            {
                CreateNewNode((NodeType)nodeData.idType, nodeData.idPrefab, nodeData.id);
                nodesMap[nodeData.id].transform.localPosition = nodeData.position;
            }

            foreach (var nodeData in newTree.nodes)
            {
                UpdateLine(nodeData.id);
            }
        }
        
        #endregion
        
        #region Data

        [Space] 
        [Header("Save Load")] 
        [SerializeField] private GenerateJsonFromTree jsonConverter;
        
        public void SaveTreeData()
        {
            HideAllNodeGroup();
            if (newTree == null)
            {
                DebugUtility.LogError("Create a tree first!");
                return;
            }

            if (string.IsNullOrEmpty(inputTreeName.text))
            {
                DebugUtility.LogError("Input a valid tree name first!");
                return;
            }
            
            newTree.nodes = new List<NodeDataStruct>();
            foreach (var pair in nodesMap)
            {
                newTree.nodes.Add(new NodeDataStruct()
                {
                    id = pair.Key,
                    idType = (int)pair.Value.CreatorNodeType,
                    idPrefab = pair.Value.PrefabIndex,
                    position = pair.Value.transform.localPosition,
                });
            }
            
            if (newTree.nodes == null || newTree.nodes.Count == 0)
            {
                DebugUtility.LogError("Tree doesn't have any nodes!");
                return;
            }
            
            jsonConverter.SaveJson(inputTreeName.text, newTree);
        }

        #endregion
    }
}