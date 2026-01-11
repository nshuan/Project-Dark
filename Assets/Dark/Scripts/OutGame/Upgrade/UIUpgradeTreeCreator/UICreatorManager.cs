using System;
using System.Collections.Generic;
using System.Linq;
using Dark.Scripts.Common.UIWarning;
using Dark.Scripts.OutGame.Upgrade.UIUpgradeTreeCreator.Grid;
using InGame.CharacterClass;
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
        public Guid guid;
        public int id;
        public int idType;
        public int idPrefab;
        public SerializableVector2 position;
        public List<Guid> preRequired;
        public List<UpgradeGroupIdInfo> groups;
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
        public JsonToTreePrefabConverter prefabConverter;
        [SerializeField] private Transform treeParent;
        [SerializeField] private GameObject groupNodeModeButtons;
        [SerializeField] private TMP_InputField input;
        [SerializeField] private TMP_InputField inputTreeName;
        [SerializeField] private Button btnNewTree;
        [SerializeField] private Button btnDeleteNode;
        [SerializeField] private Button btnChangeMode;
        [SerializeField] private Button btnDeleteAll;
        [SerializeField] private TextMeshProUGUI txtMode;
        [SerializeField] private Button[] btnToggleNodes;
        [SerializeField] private Transform[] nodeButtonGroups;
        [OdinSerialize, NonSerialized] private Dictionary<NodeType, List<NodeButtonInfo>> btnInfo;
        public Button btnSetGroupId;
        public TMP_InputField inpSetGroupId;

        [Space] [Header("Create and Load tree")] 
        [SerializeField] private GameObject groupBtnLoadTree;
        [SerializeField] private Button btnToggleLoadTree;
        [SerializeField] private Button btnCloseLoadTree;
        [SerializeField] private Button btnLoadTree;
        [SerializeField] private TMP_InputField inputTreeNameToLoad;
        [SerializeField] private UIPopupWarning popupConfirm;
        
        [Space]
        [Header("Tree Visual")]
        [SerializeField] private Transform lineParent;
        [SerializeField] private RectTransform linePrefab;
        
        private TreeDataStruct newTree;
        [ReadOnly, OdinSerialize, NonSerialized] private Dictionary<Guid, UICreatorUpgradeNode> nodesMap;
        private Dictionary<Guid, Dictionary<Guid, RectTransform>> linesMap;
        private Dictionary<Guid, List<UICreatorUpgradeNode>> nodeChildMap;
        private Dictionary<Guid, List<UICreatorUpgradeNode>> nodeParentMap;
        private Dictionary<int, NodeSpriteInfo> cacheSpriteMap;
        private Dictionary<int, List<UICreatorUpgradeNode>> groupMap;

        public bool isLinkMode;
        
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
                        
                        CreateNewNode(nodeType, pIndex, id, Guid.Empty);
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
            
            btnDeleteAll.onClick.RemoveAllListeners();
            btnDeleteAll.onClick.AddListener(ClearAll);
            
            btnSetGroupId.onClick.RemoveAllListeners();
            btnSetGroupId.onClick.AddListener(SetGroupIdForSelectingNodes);
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

        public void ClearAll()
        {
            Action actionClearAll = () =>
            {
                HideAllNodeGroup();
                UICreatorNodeInfoPreview.Instance.Hide();
                btnSetGroupId.gameObject.SetActive(false);
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

//                 foreach (var config in configLoader.GetAllConfigs())
//                 {
//                     config.preRequire = null;
// #if UNITY_EDITOR
//                     EditorUtility.SetDirty(config);
// #endif
//                 }
//
// #if UNITY_EDITOR
//                 AssetDatabase.SaveAssets();
//                 AssetDatabase.Refresh();
// #endif
//                 Debug.Log("ScriptableObject changes saved to asset.");

                newTree = new TreeDataStruct() { nodes = new List<NodeDataStruct>() };
                nodesMap = new Dictionary<Guid, UICreatorUpgradeNode>();
                linesMap = new Dictionary<Guid, Dictionary<Guid, RectTransform>>();
                nodeChildMap = new Dictionary<Guid, List<UICreatorUpgradeNode>>();
                nodeParentMap = new Dictionary<Guid, List<UICreatorUpgradeNode>>();
                selectingNodes = null;
            };
            
            popupConfirm.Setup(
                "Clear all nodes?",
                "This action will also clear all pre-required references in node configs",
                () =>
                {
                    popupConfirm.gameObject.SetActive(false);
                    actionClearAll?.Invoke();
                },
                () => popupConfirm.gameObject.SetActive(false));
            popupConfirm.gameObject.SetActive(true);
        }

        public void CreateNewTree()
        {
            groupBtnLoadTree.SetActive(false);
            HideAllNodeGroup();
            UICreatorNodeInfoPreview.Instance.Hide();
            btnSetGroupId.gameObject.SetActive(false);
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
            nodesMap = new Dictionary<Guid, UICreatorUpgradeNode>();
            linesMap = new Dictionary<Guid, Dictionary<Guid, RectTransform>>();
            nodeChildMap =  new Dictionary<Guid, List<UICreatorUpgradeNode>>();
            nodeParentMap = new Dictionary<Guid, List<UICreatorUpgradeNode>>();
            selectingNodes = null;
        }

        public Guid CreateNewNode(NodeType nodeType, int prefabIndex, int id, Guid guid, List<Guid> preRequire = null)
        {
            HideAllNodeGroup();
            UICreatorNodeInfoPreview.Instance.Hide();
            btnSetGroupId.gameObject.SetActive(false);
            if (newTree == null)
            {
                DebugUtility.LogError("Create a tree first!");
                return Guid.Empty;
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
                    return Guid.Empty;
                }
            }

            var prefab = btnInfo[nodeType][prefabIndex].nodePrefab;
            var node = Instantiate(prefab, treeParent);
            node.manager = this;
            node.config = nodeConfig;
            node.CreatorNodeType = nodeType;
            node.PrefabIndex = prefabIndex;
            node.guid = guid == Guid.Empty ? Guid.NewGuid() : guid;
            
            nodesMap.Add(node.guid, node);
            nodeChildMap.TryAdd(node.guid, new List<UICreatorUpgradeNode>());
            nodeParentMap.TryAdd(node.guid, new List<UICreatorUpgradeNode>());
            if (preRequire != null)
            {
                foreach (var preGuid in preRequire)
                {
                    nodeChildMap.TryAdd(preGuid, new List<UICreatorUpgradeNode>());
                    if (!nodeChildMap[preGuid].Contains(node))
                        nodeChildMap[preGuid].Add(node);
                    if (nodesMap.ContainsKey(preGuid))
                        nodeParentMap[node.guid].Add(nodesMap[preGuid]);
                }
            }

            foreach (var childNode in nodeChildMap[node.guid])
            {
                nodeParentMap.TryAdd(childNode.guid, new List<UICreatorUpgradeNode>());
                if (!nodeParentMap[childNode.guid].Contains(node))
                {
                    nodeParentMap[childNode.guid].Add(node);
                    var direction = (childNode.transform.position - node.transform.position).normalized;
                    ShowPreRequiredLine(node.guid, 
                        node.transform.position + direction * node.lineAnchorOffsetRadius, 
                        childNode.guid, 
                        childNode.transform.position - direction * childNode.lineAnchorOffsetRadius);
                }
            }

            foreach (var childNode in nodeChildMap[node.guid])
            {
                var direction = (childNode.transform.position - node.transform.position).normalized;
                ShowPreRequiredLine(node.guid, 
                    node.transform.position + direction * node.lineAnchorOffsetRadius, 
                    childNode.guid, 
                    childNode.transform.position - direction * childNode.lineAnchorOffsetRadius);
            }

            foreach (var parentNode in nodeParentMap[node.guid])
            {
                var direction = (node.transform.position - parentNode.transform.position).normalized; 
                ShowPreRequiredLine(parentNode.guid, 
                    parentNode.transform.position + direction * parentNode.lineAnchorOffsetRadius,
                    node.guid, 
                    node.transform.position - direction * node.lineAnchorOffsetRadius);
            }
            
            UpdateLine(node.guid);
            
            node.InitNode();
            
            UpdateNodeSprites(node);

            return node.guid;
        }

        public void DeleteNode()
        {
            HideAllNodeGroup();
            if (selectingNodes == null || selectingNodes.Count == 0)
            {
                DebugUtility.LogWarning("You are not selecting any node!");
                return;
            }

            foreach (var selectingNode in selectingNodes)
            {
                // Delete lines
                if (linesMap.ContainsKey(selectingNode.guid))
                {
                    foreach (var pair in linesMap[selectingNode.guid])
                    {
                        Destroy(pair.Value.gameObject);
                    }
                    linesMap.Remove(selectingNode.guid);
                }
                foreach (var pair in linesMap)
                {
                    if (pair.Value != null && pair.Value.ContainsKey(selectingNode.guid))
                    {
                        Destroy(pair.Value[selectingNode.guid].gameObject);
                        pair.Value.Remove(selectingNode.guid);
                    }
                }
                
                // Delete node references
                if (nodeChildMap.ContainsKey(selectingNode.guid))
                {
                    foreach (var child in nodeChildMap[selectingNode.guid])
                    {
                        if (nodeParentMap.ContainsKey(child.guid) &&
                            nodeParentMap[child.guid].Contains(selectingNode))
                            nodeParentMap[child.guid].Remove(selectingNode);
                    }
                    nodeChildMap.Remove(selectingNode.guid);
                }
                if (nodeParentMap.ContainsKey(selectingNode.guid))
                {
                    foreach (var parent in nodeParentMap[selectingNode.guid])
                    {
                        if (nodeChildMap.ContainsKey(parent.guid) &&
                            nodeChildMap[parent.guid].Contains(selectingNode))
                            nodeChildMap[parent.guid].Remove(selectingNode);
                    }
                    nodeParentMap.Remove(selectingNode.guid);
                }
                if (nodesMap.ContainsKey(selectingNode.guid))
                    nodesMap.Remove(selectingNode.guid);
                
                Destroy(selectingNode.gameObject);
            }
            
            selectingNodes = null;
            btnDeleteNode.gameObject.SetActive(false);
            UICreatorNodeInfoPreview.Instance.Hide();
            btnSetGroupId.gameObject.SetActive(false);
        }

        public void UpdateNodeSprites(UICreatorUpgradeNode node)
        {
#if UNITY_EDITOR
            cacheSpriteMap ??= UIUpgradeTree.GetSpritesMapById();
            
            var id = node.config.nodeId;
            if (cacheSpriteMap.TryGetValue(id, out var spriteInfo))
            {
                node.SetIcon(spriteInfo.normalSprite);
            }
            
#endif
        }
        
        public void ChangeMode()
        {
            HideAllNodeGroup();
            DeselectAll();
            isLinkMode = !isLinkMode;
            txtMode.SetText(isLinkMode ? "Link Mode" : "Node Mode");
            groupNodeModeButtons.SetActive(!isLinkMode);
        }
        public UICreatorUpgradeNode GetNodeById(Guid id)
        {
            return nodesMap.GetValueOrDefault(id);
        }
        
        public void ShowPreRequiredLine(Guid fromId, Vector2 from, Guid toId, Vector2 to)
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
                linesMap.Add(fromId, new Dictionary<Guid, RectTransform>());
                linesMap[fromId].Add(toId, line);
            }
        }

        public void UpdateLine(Guid guid)
        {
            var from = new Vector2();
            var to = new Vector2();
            var direction = new Vector2();
            foreach (var childNode in nodeChildMap[guid])
            {
                from.x = nodesMap[guid].transform.localPosition.x;
                from.y = nodesMap[guid].transform.localPosition.y;
                to.x = childNode.transform.localPosition.x;
                to.y = childNode.transform.localPosition.y;
                direction.x = to.x - from.x;
                direction.y = to.y - from.y;
                if (direction.magnitude > 0.05f)
                {
                    direction = direction / direction.magnitude;
                    from = from + direction * nodesMap[guid].lineAnchorOffsetRadius;
                    to = to - direction * childNode.lineAnchorOffsetRadius;
                    linesMap[guid][childNode.guid].localPosition = (from + to) / 2;
                    linesMap[guid][childNode.guid].sizeDelta = new Vector2(Vector2.Distance(from, to), 8f);
                    linesMap[guid][childNode.guid].rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
                }
            }

            foreach (var parentNode in nodeParentMap[guid])
            {
                from.x = parentNode.transform.localPosition.x;
                from.y = parentNode.transform.localPosition.y;
                to.x = nodesMap[guid].transform.localPosition.x;
                to.y = nodesMap[guid].transform.localPosition.y;
                direction.x = to.x - from.x;
                direction.y = to.y - from.y;
                if (direction.magnitude > 0.05f)
                {
                    direction = direction / direction.magnitude;
                    from = from + direction * parentNode.lineAnchorOffsetRadius;
                    to = to - direction * nodesMap[guid].lineAnchorOffsetRadius;
                    linesMap[parentNode.guid][guid].localPosition = (from + to) / 2;
                    linesMap[parentNode.guid][guid].sizeDelta = new Vector2(Vector2.Distance(from, to), 8f);
                    linesMap[parentNode.guid][guid].rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
                }
            }
        }

        #region Select Node

        [Space]
        [Header("Select Node")]
        [SerializeField] private Button btnDeselectAll;
        [SerializeField] private KeyCode keyMultiselect = KeyCode.LeftShift;

        private bool forceMultiselect = false;
        private bool keyMultiselectDown;
        private List<UICreatorUpgradeNode> selectingNodes;

        public void SelectNode(UICreatorUpgradeNode node)
        {
            HideAllNodeGroup();
            if (isLinkMode)
            {
                SelectNodeLink(node);
                return;
            }

            if (keyMultiselectDown == false && forceMultiselect == false)
            {
                if (selectingNodes != null)
                {
                    foreach (var selecting in selectingNodes)
                    {
                        selecting.DeselectThis();
                    }
                }
                
                selectingNodes = new List<UICreatorUpgradeNode>() { node };
                node.SelectThis();
                UICreatorNodeInfoPreview.Instance.UpdateUI(this, node, node.config);
                UICreatorNodeInfoPreview.Instance.Show();
                btnSetGroupId.gameObject.SetActive(false);
            }
            else
            {
                selectingNodes ??= new List<UICreatorUpgradeNode>();
                if (selectingNodes.Contains(node))
                {
                    selectingNodes.Remove(node);
                    node.DeselectThis();
                }
                else
                {
                    selectingNodes.Add(node);
                    node.SelectThis();
                }
                UICreatorNodeInfoPreview.Instance.Hide();
                btnSetGroupId.gameObject.SetActive(true);
                inpSetGroupId.text = "";
            }
            
            if (selectingNodes == null || selectingNodes.Count == 0)
                btnDeleteNode.gameObject.SetActive(false);
            else 
                btnDeleteNode.gameObject.SetActive(true);
        }

        public void DeselectAll()
        {
            HideAllNodeGroup();
            if (selectingNodes != null)
            {
                foreach (var node in selectingNodes)
                {
                    node.DeselectThis();
                }
            }
            selectingNodes= null;
            btnDeleteNode.gameObject.SetActive(false);
            UICreatorNodeInfoPreview.Instance.Hide();
            btnSetGroupId.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(keyMultiselect))
                keyMultiselectDown = true;
            if (Input.GetKeyUp(keyMultiselect))
                keyMultiselectDown = false;
        }

        #endregion

        #region Group

        public void SelectGroupNodes(List<int> groupId)
        {
            DeselectAll();
            forceMultiselect = true;
            foreach (var pair in nodesMap)
            {
                if (pair.Value.group.Any(groupId.Contains)) SelectNode(pair.Value);
            }
            forceMultiselect = false;
        }

        private void SetGroupIdForSelectingNodes()
        {
            if (selectingNodes == null) return;
            var listGroupStr = inpSetGroupId.text.Trim(' ').Split(',');
            var groups = new List<int>();
            foreach (var str in listGroupStr)
            {
                if (int.TryParse(str, out var intValue))
                {
                    groups.Add(intValue);
                }
            }
            foreach (var selectingNode in selectingNodes)
            {
                selectingNode.group = new List<int>(groups);
            }
            
            RefreshGroupNodes();
        }

        public void RefreshGroupNodes()
        {
            groupMap ??= new Dictionary<int, List<UICreatorUpgradeNode>>();

            foreach (var pair in nodesMap)
            {
                foreach (var group in pair.Value.group)
                {
                    if (groupMap.TryGetValue(group, out var listNode))
                    {
                        if (!listNode.Contains(pair.Value)) listNode.Add(pair.Value);
                        groupMap[group] = listNode;
                    }
                    else
                    {
                        listNode = new List<UICreatorUpgradeNode>() { pair.Value };
                        groupMap[group] = listNode;
                    }
                }
            }
        }

        public void SetGroupLockNode(UICreatorUpgradeNode lockNode)
        {
            groupMap ??= new Dictionary<int, List<UICreatorUpgradeNode>>();

            foreach (var group in lockNode.group)
            {
                if (groupMap.TryGetValue(group, out var listNode))
                {
                    foreach (var node in listNode)
                    {
                        node.isGroupLockNode ??= new Dictionary<int, bool>();
                        node.isGroupLockNode[group] = false;
                        node.SetAreaLock();
                    }
                }
                else
                {
                    groupMap[group] = new List<UICreatorUpgradeNode>() { lockNode };
                }
                
                lockNode.isGroupLockNode ??= new Dictionary<int, bool>();
                lockNode.isGroupLockNode[group] = true;
                lockNode.SetAreaLock();
            }
        }

        #endregion
        
        #region Edit node links

        public void SelectNodeLink(UICreatorUpgradeNode node)
        {
            if (keyMultiselectDown)
            {
                selectingNodes ??= new List<UICreatorUpgradeNode>();
                if (selectingNodes.Contains(node))
                {
                    selectingNodes.Remove(node);
                    node.DeselectThis();
                }
                else
                {
                    selectingNodes.Add(node);
                    node.SelectThis();
                }
            }
            else
            {
                if (selectingNodes == null || selectingNodes.Count == 0)
                {
                    selectingNodes = new List<UICreatorUpgradeNode>() { node };
                    node.SelectThis();
                }
                else
                {
                    foreach (var selectingNode in selectingNodes)
                    {
                        // Add or remove link node
                        if (nodeParentMap.ContainsKey(selectingNode.guid) && nodeParentMap[selectingNode.guid].Contains(node))
                        {
                            // Update data
                            var newPreRequire = nodeParentMap[selectingNode.guid].Where((preNode) => !ReferenceEquals(preNode, node))
                                .ToList();
                            nodeParentMap[selectingNode.guid] = newPreRequire;
        // #if UNITY_EDITOR
        //                     EditorUtility.SetDirty(selectingNode.config);
        //                     AssetDatabase.SaveAssets();
        //                     AssetDatabase.Refresh();
        //                     Debug.Log("ScriptableObject changes saved to asset.");
        // #endif
                            
                            if (linesMap.ContainsKey(node.guid))
                            {
                                if (linesMap[node.guid].ContainsKey(selectingNode.guid))
                                {
                                    Destroy(linesMap[node.guid][selectingNode.guid].gameObject);
                                    linesMap[node.guid].Remove(selectingNode.guid);
                                }
                            }
                            if (nodeParentMap.ContainsKey(selectingNode.guid))
                            {
                                if (nodeParentMap[selectingNode.guid].Contains(node))
                                    nodeParentMap[selectingNode.guid].Remove(node);
                            }
                            if (nodeChildMap.ContainsKey(node.guid))
                            {
                                if (nodeChildMap[node.guid].Contains(selectingNode))
                                    nodeChildMap[node.guid].Remove(selectingNode);
                            }
                        }
                        else
                        {
                            // Check loop
                            // TODO check loop
                            
        //                     // Update data
        //                     var newPreRequire = selectingNode.config.preRequire.ToList();
        //                     newPreRequire.Add(node.config);
        //                     selectingNode.config.preRequire = newPreRequire.ToArray();
        // #if UNITY_EDITOR
        //                     EditorUtility.SetDirty(selectingNode.config);
        //                     AssetDatabase.SaveAssets();
        //                     AssetDatabase.Refresh();
        //                     Debug.Log("ScriptableObject changes saved to asset.");
        // #endif

                            linesMap.TryAdd(node.guid, new Dictionary<Guid, RectTransform>());
                            if (!linesMap[node.guid].ContainsKey(selectingNode.guid))
                            {
                                var line = Instantiate(linePrefab, lineParent);
                                linesMap[node.guid].Add(selectingNode.guid, line);
                            }
                            nodeParentMap.TryAdd(selectingNode.guid, new List<UICreatorUpgradeNode>());
                            if (!nodeParentMap[selectingNode.guid].Contains(node))
                                nodeParentMap[selectingNode.guid].Add(node);
                            nodeChildMap.TryAdd(node.guid, new List<UICreatorUpgradeNode>());
                            if (!nodeChildMap[node.guid].Contains(selectingNode))
                                nodeChildMap[node.guid].Add(selectingNode);
                            
                            UpdateLine(node.guid);
                        }
                        
                        selectingNode.DeselectThis();
                    }
                    
                    selectingNodes = null;
                }
            }
        }

        public void OnDragNode(RectTransform anchor, Vector3 delta)
        {
            if (selectingNodes == null || selectingNodes.Count == 0) return;
            if (selectingNodes.All((node) => (RectTransform)node.transform != anchor)) return;

            foreach (var selectingNode in selectingNodes)
            {
                if ((RectTransform)selectingNode.transform == anchor) continue;
                selectingNode.transform.position += delta;
                UpdateLine(selectingNode.guid);
            }
        }

        public void OnEndDrag()
        {
            if (selectingNodes == null || selectingNodes.Count == 0) return;
            
            foreach (var selectingNode in selectingNodes)
            {
                UIAlignManager.Instance.Align((RectTransform)selectingNode.transform);
                UpdateLine(selectingNode.guid);
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
                var guid = CreateNewNode((NodeType)nodeData.idType, nodeData.idPrefab, nodeData.id, nodeData.guid, nodeData.preRequired);
                nodeData.guid = guid;
                nodesMap[guid].transform.localPosition = nodeData.position;
                nodeData.groups ??= new List<UpgradeGroupIdInfo>() { new UpgradeGroupIdInfo() { groupId = 0 } };
                nodesMap[guid].group = nodeData.groups.Select((info) => info.groupId).ToList();
                nodesMap[guid].isGroupLockNode = new Dictionary<int, bool>();
                foreach (var group in nodeData.groups)
                {
                    if (group.isLockNode) nodesMap[guid].isGroupLockNode.Add(group.groupId, true);
                }
                nodesMap[guid].SetAreaLock();
            }

            RefreshGroupNodes();
            
            foreach (var nodeData in newTree.nodes)
            {
                UpdateLine(nodeData.guid);
            }

            inputTreeName.text = name;
        }
        
        #endregion
        
        #region Data

        [Space] 
        [Header("Save Load")] 
        [SerializeField] private GenerateJsonFromTree jsonConverter;
        
        public void SaveTreeData()
        {
            RefreshGroupNodes();
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
                List<Guid> preRequire = null; 
                if (nodeParentMap.ContainsKey(pair.Key))
                    preRequire = nodeParentMap[pair.Key].Select((node) => node.guid).ToList();
                else
                    preRequire = new List<Guid>();

                var groupInfo = new List<UpgradeGroupIdInfo>();
                foreach (var group in pair.Value.group)
                {
                    groupInfo.Add(new UpgradeGroupIdInfo()
                    {
                        groupId = group,
                        isLockNode = pair.Value.isGroupLockNode != null && pair.Value.isGroupLockNode.ContainsKey(group) && pair.Value.isGroupLockNode[group]
                    });
                }
                if (groupInfo.Count == 0) groupInfo.Add(new UpgradeGroupIdInfo() { groupId = 0 });
                newTree.nodes.Add(new NodeDataStruct()
                {
                    guid = pair.Key,
                    id = pair.Value.config.nodeId,
                    idType = (int)pair.Value.CreatorNodeType,
                    idPrefab = pair.Value.PrefabIndex,
                    position = pair.Value.transform.localPosition,
                    preRequired = preRequire,
                    groups = groupInfo,
                });
            }
            
            if (newTree.nodes == null || newTree.nodes.Count == 0)
            {
                DebugUtility.LogError("Tree doesn't have any nodes!");
                return;
            }
            
            jsonConverter.SaveJson(inputTreeName.text, newTree);

#if UNITY_EDITOR
            prefabConverter.ConvertJsonToPrefab(inputTreeName.text, inputTreeName.text);
            UpgradeTreeManifest.GetTreeConfig(CharacterClass.Archer).Validate();
#endif
        }

        #endregion
    }
}