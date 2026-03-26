using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using Dark.Scripts.InGame.Upgrade;
using Dark.Scripts.SceneNavigation;
using DG.Tweening;
using Economic;
using InGame;
using InGame.CharacterClass;
using InGame.Upgrade;
using InGame.Upgrade.DynamicCost;
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

        [ReadOnly, OdinSerialize, NonSerialized] private Dictionary<int, List<UIUpgradeNode>> nodesMap; // Luu cac node co cung id
        [ReadOnly, OdinSerialize, NonSerialized] public Dictionary<int, List<UIUpgradeNode>> nodeChildrenMap;
        [ReadOnly, OdinSerialize, NonSerialized] public Dictionary<int, List<UIUpgradeNode>> nodesMapByLayer;

        public Dictionary<int, List<UIUpgradeNode>> NodesMap => nodesMap;
        
        [Space] [Header("Skill Node Ids")]
        public List<int> skillNodeIds = new List<int>();
        
        [Space] [Header("Passive Node Ids")]
        public List<int> passiveNodeIds = new List<int>();
        
        [Space] [Header("UI")] 
        [SerializeField] private Button btnDeselectAll;

        [Space] [Header("Spawn")] 
        [SerializeField] private float nodeSpawnDelayStep = 0.1f;
        [SerializeField] private float firstNodeDelayStep = 0.5f;

        [Space] [Header("Reset Skill")] 
        [SerializeField] private UpgradeResetConfig resetConfig;

        public UpgradeResetConfig ResetConfig => resetConfig;
        
        public int LastUpgradeNodeId { get; set; } = -1;
        public Action<UIUpgradeNode> OnNodeUpgraded { get; set; }
        public Action OnTreeSpawned { get; set; }

        private void OnDestroy()
        {
            OnNodeUpgraded = null;
            OnTreeSpawned = null;
        }

        public void UpdateChildren(int id, bool isUnlock, bool isResetNode = false)
        {
            if (nodeChildrenMap.TryGetValue(id, out var children))
            {
                foreach (var childNode in children)
                {
                    if (isUnlock) childNode.DoUnlockVfx(id).OnComplete(() =>
                    {
                        childNode.UpdateUI();
                        if (childNode.CurrentState == UIUpgradeNodeState.Activated)
                            UpdateChildren(childNode.config.nodeId, true);
                    });
                    else
                    {
                        childNode.UpdateUI();
                        if (isResetNode) UpdateChildren(childNode.config.nodeId, false, true);
                    }
                }
            }
        }

        public void UpgradeAllNodesWithId(int id)
        {
            if (nodesMap.TryGetValue(id, out var nodes))
            {
                foreach (var node in nodes)
                {
                    node.Upgrade();
                }
            }

            foreach (var pair in NodesMap)
            {
                foreach (var node in pair.Value)
                {
                    node.SetGlow();
                }
            }
        }

        public void ResetNode(int nodeId, UpgradeGroupIdInfo[] groups, bool useResetPoint)
        {
            if (!UpgradeManager.Instance.ResetNode(nodeId, groups, useResetPoint)) return;

            if (nodeChildrenMap.TryGetValue(nodeId, out var children))
            {
                foreach (var child in children)
                {
                    var canReset = true;
                    if (child.preRequires is { Count: > 1 })
                    {
                        foreach (var preRequire in child.preRequires)
                        {
                            if (preRequire.preRequireId == nodeId) continue;
                            var data = UpgradeManager.Instance.GetData(preRequire.preRequireId);
                            if (data is { level: > 0 })
                            {
                                canReset = false;
                                break;
                            }
                        }
                    }
                    if (canReset) ResetNode(child.config.nodeId, child.config.groupId, false);
                }
            }
        }

        public (int, int, int) GetResetReturnResources(UpgradeNodeConfig nodeConfig)
        {
            var checkedList = new List<int>();
            var result = GetResetReturnResources(nodeConfig, ref checkedList);
            result.Item1 = Mathf.RoundToInt(result.Item1 * resetConfig.refundVestigeRatio);
            result.Item2 = Mathf.RoundToInt(result.Item2 * resetConfig.refundEchoesRatio);
            result.Item3 = Mathf.RoundToInt(result.Item3 * resetConfig.refundSigilsRatio);
            return result;
        }

        private (int, int, int) GetResetReturnResources(UpgradeNodeConfig nodeConfig, ref List<int> checkedNodes)
        {
            if (checkedNodes.Contains(nodeConfig.nodeId)) return (0, 0, 0);
            checkedNodes.Add(nodeConfig.nodeId);
            
            var data = UpgradeManager.Instance.GetData(nodeConfig.nodeId);
            if (data == null || data.level == 0) return (0, 0, 0);

            var result = (0, 0, 0);
            var nodeGroupUnlockOrder = nodeConfig.groupId.Min((groupId) => UpgradeManager.Instance.GetGroupUnlockOrder(groupId.groupId, false));
            var costInfo = nodeConfig.costInfo;
            
            foreach (var cost in costInfo)
            {
                if (cost.costType == WealthType.Vestige)
                {
                    if (nodeConfig.dynamicVestige)
                    {
                        if (nodeConfig.MaxLevel == 1)
                        {
                            result.Item1 += Mathf.RoundToInt(nodeConfig.vestigeCostRatio *
                                                      DynamicVestigeConfig.Instance.GetCost1Stage(
                                                          nodeGroupUnlockOrder));
                        }
                        else
                        {
                            var listCostValue = DynamicVestigeConfig.Instance.GetCost5Stage(nodeGroupUnlockOrder);
                            for (var i = 0; i < data.level; i++)
                            {
                                if (i >= listCostValue.Length) break;
                                result.Item1 += Mathf.RoundToInt(nodeConfig.vestigeCostRatio * listCostValue[i]);
                            }
                        }
                    }
                    else
                    {
                        for (var i = 0; i < data.level; i++)
                        {
                            if (i >= cost.costValue.Length) break;
                            result.Item1 += Mathf.RoundToInt(nodeConfig.vestigeCostRatio * cost.costValue[i]);
                        }
                    }
                }
                else if (cost.costType == WealthType.Echoes)
                {
                    if (nodeConfig.dynamicEchoes)
                    {
                        if (nodeConfig.MaxLevel == 1)
                        {
                            result.Item2 += Mathf.RoundToInt(1f *
                                                             DynamicVestigeConfig.Instance.GetCost1Echoes(
                                                                 nodeGroupUnlockOrder));
                        }
                        else
                        {
                            var listCostValue = DynamicVestigeConfig.Instance.GetCost5Echoes(nodeGroupUnlockOrder);
                            for (var i = 0; i < data.level; i++)
                            {
                                if (i >= listCostValue.Length) break;
                                result.Item2 += Mathf.RoundToInt(1f * listCostValue[i]);
                            }
                        }
                    }
                    else
                    {
                        for (var i = 0; i < data.level; i++)
                        {
                            if (i >= cost.costValue.Length) break;
                            result.Item2 += Mathf.RoundToInt(1f * cost.costValue[i]);
                        }
                    }
                }
                else
                {
                    for (var i = 0; i < data.level; i++)
                    {
                        if (i >= cost.costValue.Length) break;
                        result.Item3 += Mathf.RoundToInt(1f * cost.costValue[i]);
                    }
                }
            }

            if (nodeChildrenMap.TryGetValue(nodeConfig.nodeId, out var children))
            {
                foreach (var child in children)
                {
                    var canReset = true;
                    if (child.preRequires is { Count: > 1 })
                    {
                        foreach (var preRequire in child.preRequires)
                        {
                            if (preRequire.preRequireId == nodeConfig.nodeId) continue;
                            var preRequiredData = UpgradeManager.Instance.GetData(preRequire.preRequireId);
                            if (preRequiredData == null || preRequiredData.level == 0) continue;
                            if (checkedNodes.Contains(preRequire.preRequireId)) continue;
                            canReset = false;
                            break;
                        }
                    }
                    if (!canReset) continue; 
                    
                    var childReturnValue = GetResetReturnResources(child.config, ref checkedNodes);
                    result.Item1 += childReturnValue.Item1;
                    result.Item2 += childReturnValue.Item2;
                    result.Item3 += childReturnValue.Item3;
                }
            }

            return result;
        }
        
        public void InvokeNodeUpgraded(UIUpgradeNode node)
        {
            OnNodeUpgraded?.Invoke(node);
        }

        public bool IsNodeSkill(int nodeId)
        {
            return skillNodeIds.Contains(nodeId);
        }

        public bool IsNodePassive(int nodeId)
        {
            return passiveNodeIds.Contains(nodeId);
        }

        private void Awake()
        {
            btnDeselectAll.onClick.RemoveAllListeners();
            btnDeselectAll.onClick.AddListener(() =>
            {
                UIUpgradeNodeInfoPreview.Instance.Hide(true);
            });
        }

        private void OnEnable()
        {
            UpgradeManager.Instance.RefreshGroupUnlockOrder();
            // Auto upgrade node layer 0
            foreach (var nodeBase in nodesMapByLayer[0])
            {
                // Do area 0 được đánh id là 1
                UpgradeManager.Instance.UpgradeNode(UpgradeNodeType.NodeClass, nodeBase.config.nodeId,
                    new[] { new UpgradeGroupIdInfo() { groupId = 1, isLockNode = true } });
            }
            
            // Do Spawn Animation
            var spawnNodesMapByLayer = new Dictionary<int, List<UIUpgradeNode>>();
            var currentLayerNodes = new List<UIUpgradeNode>();
            foreach (var layer in nodesMapByLayer)
            {
                foreach (var node in layer.Value)
                {
                    node.UpdateUI();    
                    if (GameConst.HideLockedNode && node.CurrentState == UIUpgradeNodeState.Locked)
                        continue;
                    currentLayerNodes.Add(node);
                }
                
                if (currentLayerNodes is { Count: > 0 })
                    spawnNodesMapByLayer[layer.Key] = new List<UIUpgradeNode>(currentLayerNodes);
                
                currentLayerNodes.Clear(); 
            }
            
            OnTreeSpawned?.Invoke();
            
            DOTween.Kill(this);
            var seq = DOTween.Sequence(this);
            foreach (var pair in spawnNodesMapByLayer)
            {
                foreach (var node in pair.Value)
                {
                    var stepSeq = DOTween.Sequence();
                    if (node.preRequires is { Count: > 0 })
                    {
                        foreach (var preRequireInfo in node.preRequires)
                        {
                            stepSeq.Join(preRequireInfo.line.DoSpawn());
                        }
                    }

                    stepSeq.Join(node.DoSpawn().SetDelay(0.05f));
                    stepSeq.Pause();
                    
                    seq.AppendCallback(() =>
                    {
                        stepSeq.Play();
                    });
                }

                if (pair.Key == 0) seq.AppendInterval(firstNodeDelayStep);
                else seq.AppendInterval(nodeSpawnDelayStep);
                
                if (pair.Key >= 5) break;
            }

            seq.SetDelay(Loading.Instance.CurrentTotalDurationAfterSceneLoaded)
                .AppendInterval(2f)
                .OnComplete(() =>
                {
                    if (UIUpgradeNodeSkillPool.Instance.canvasForVfxAppear)
                        Destroy(UIUpgradeNodeSkillPool.Instance.canvasForVfxAppear.gameObject);
                });
        }

#if UNITY_EDITOR
        private static string spritesPath = "Assets/Dark/Config/Upgrade/Skill_Tree_Sprites";
        
        [Button]
        public void ValidateNodes(CharacterClass classType)
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

            nodesMap = new Dictionary<int, List<UIUpgradeNode>>();
            nodeChildrenMap = new Dictionary<int, List<UIUpgradeNode>>();
            var nodes = GetComponentsInChildren<UIUpgradeNode>();
            foreach (var node in nodes)
            {
                node.treeRef = this;
                nodesMap.TryAdd(node.config.nodeId, new List<UIUpgradeNode>());
                if (!nodesMap[node.config.nodeId].Contains(node))
                    nodesMap[node.config.nodeId].Add(node);
            }
            
            foreach (var node in nodes)
            {
                node.name = $"{node.config.nodeId}_{node.config.nodeName}";
                if (node.preRequires == null || node.preRequires.Count == 0) continue;
                foreach (var preRequire in node.preRequires)
                {
                    if (!preRequire.node) continue;
                    preRequire.line = ShowPreRequiredLine(node.transform.position, node.lineAnchorOffsetRadius,
                        preRequire.node.transform.position, preRequire.node.lineAnchorOffsetRadius);
                    preRequire.line.name = $"Line_{node.config.nodeId}_{preRequire.node.config.nodeId}";
                    // Add child map
                    nodeChildrenMap.TryAdd(preRequire.preRequireId, new List<UIUpgradeNode>());
                    if (!nodeChildrenMap[preRequire.preRequireId].Contains(node))
                        nodeChildrenMap[preRequire.preRequireId].Add(node);
                }
                
                EditorUtility.SetDirty(node);
            }
            
            // Cache node map by layer
            nodesMapByLayer = new Dictionary<int, List<UIUpgradeNode>>();
            var checkedNodes = new List<int>();
            var currentLayerNodes = new List<UIUpgradeNode>();
            foreach (var pair in nodesMap)
            {
                if (pair.Value[0].preRequires == null || pair.Value[0].preRequires.Count == 0)
                {
                    foreach (var node in pair.Value)
                    {
                        if (checkedNodes.Contains(node.config.nodeId)) continue;
                        currentLayerNodes.Add(node);
                        checkedNodes.Add(node.config.nodeId);
                    }
                }
            }
            
            nodesMapByLayer[0] = new List<UIUpgradeNode>(currentLayerNodes);
            while (currentLayerNodes.Count > 0)
            {
                var newLayerNodes = new List<UIUpgradeNode>();
                foreach (var node in currentLayerNodes)
                {
                    if (!nodeChildrenMap.TryGetValue(node.config.nodeId, out var childrenNodes)) continue;
                    foreach (var child in childrenNodes)
                    {
                        if (checkedNodes.Contains(child.config.nodeId)) continue;
                        if (newLayerNodes.Contains(child)) continue;
                        newLayerNodes.Add(child);
                        checkedNodes.Add(child.config.nodeId);
                    }
                }

                if (newLayerNodes.Count > 0)
                {
                    nodesMapByLayer[nodesMapByLayer.Count] = newLayerNodes;
                }

                currentLayerNodes = newLayerNodes;
            }
            
            EditorUtility.SetDirty(this);
            
            UpdateNodeSprites(classType);
        }
        
        [Button]
        public void ValidateNodeRequireItself()
        {
            foreach (var pair in nodesMap)
            {
                foreach (var node in pair.Value)
                {
                    if (node.preRequires != null)
                    {
                        var newPreRequires = new List<UIUpgradePreRequireInfo>();
                        foreach (var pre in node.preRequires)
                        {
                            if (pre.preRequireId != node.config.nodeId)
                                newPreRequires.Add(pre);
                        }

                        node.preRequires = newPreRequires;
                    }
                    
                    EditorUtility.SetDirty(node);
                }
            }
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

        [Button]
        public void UpdateNodeSprites(CharacterClass classType)
        {
            var spriteMap = GetSpritesMapById(classType);
            
            var nodes = GetComponentsInChildren<UIUpgradeNode>();
            foreach (var node in nodes)
            {
                var id = node.config.nodeId;
                if (spriteMap.TryGetValue(id, out var spriteInfo))
                {
                    node.SetIconNormal(spriteInfo.normalSprite);
                    node.SetIconLocked(spriteInfo.lockedSprite);
                }
            }
            
            EditorUtility.SetDirty(this);
        }
        
        public static Dictionary<int, NodeSpriteInfo> GetSpritesMapById(CharacterClass classType)
        {
            var nodeSkillPath = spritesPath;
            if (classType == CharacterClass.Archer) nodeSkillPath += "/Skill_Archer";
            else if (classType == CharacterClass.Knight) nodeSkillPath += "/Skill_Knight";
            
            // Get all sprites from path
            string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { spritesPath });
            string[] skillGuids = AssetDatabase.FindAssets("t:Sprite", new[] { nodeSkillPath });
            Sprite[] sprites = new Sprite[guids.Length];
            Sprite[] skillSprites = new Sprite[skillGuids.Length];
        
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                sprites[i] = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }
            
            for (int i = 0; i < skillGuids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(skillGuids[i]);
                skillSprites[i] = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }
            
            // Sprite should be named as one of the below, the name with "locked" is the locked state of the node
            // [nodeId]_UI_Icon_[nodeType]_[nodeName]
            // [nodeId]_UI_Icon_[nodeType]_locked_[nodeName]
            var map = new Dictionary<int, NodeSpriteInfo>();
            foreach (var sprite in sprites)
            {
                var nameParts = sprite.name.Split('_');
                if (nameParts.Length == 0)
                {
                    Debug.LogError($"Invalid name: {sprite.name}");
                    continue;
                }

                if (!int.TryParse(nameParts[0], out var id))
                {
                    Debug.LogError($"Invalid id: {sprite.name}");
                    continue;
                }

                map.TryAdd(id, new NodeSpriteInfo());
                if (nameParts.Any(part => part == "locked"))
                    map[id].lockedSprite = sprite;
                else
                    map[id].normalSprite = sprite;
            }
            
            // Sprite should be named as one of the below, the name with "locked" is the locked state of the node
            // [nodeId]_UI_Icon_[nodeType]_[nodeName]
            // [nodeId]_UI_Icon_[nodeType]_locked_[nodeName]
            foreach (var sprite in skillSprites)
            {
                var nameParts = sprite.name.Split('_');
                if (nameParts.Length == 0)
                {
                    Debug.LogError($"Invalid name: {sprite.name}");
                    continue;
                }

                if (!int.TryParse(nameParts[0], out var id))
                {
                    Debug.LogError($"Invalid id: {sprite.name}");
                    continue;
                }

                map.TryAdd(id, new NodeSpriteInfo());
                if (nameParts.Any(part => part == "locked"))
                    map[id].lockedSprite = sprite;
                else
                    map[id].normalSprite = sprite;
            }

            return map;
        }

        [Button]
        public void ValidateSkillAndPassiveNodes()
        {
            return;
        }
#endif
    }
    
    [Serializable]
    public class NodeSpriteInfo
    {
        public Sprite normalSprite;
        public Sprite lockedSprite;
    }
}
