using System;
using System.Collections.Generic;
using System.Linq;
using Dark.Scripts.SceneNavigation;
using DG.Tweening;
using InGame;
using InGame.Upgrade;
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
        [ReadOnly, OdinSerialize, NonSerialized] private Dictionary<int, List<UIUpgradeNode>> nodeChildrenMap;
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

        public int LastUpgradeNodeId { get; set; } = -1;
        public Action<UIUpgradeNode> OnNodeUpgraded { get; set; }
        
        public void UpdateChildren(int id, bool isUnlock)
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
                    else childNode.UpdateUI();
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
                UpgradeManager.Instance.UpgradeNode(nodeBase.config.nodeId,
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
            
            UpdateNodeSprites();
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
        public void UpdateNodeSprites()
        {
            var spriteMap = GetSpritesMapById();
            
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
        
        public static Dictionary<int, NodeSpriteInfo> GetSpritesMapById()
        {
            // Get all sprites from path
            string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { spritesPath });
            Sprite[] sprites = new Sprite[guids.Length];
        
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                sprites[i] = AssetDatabase.LoadAssetAtPath<Sprite>(path);
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

            return map;
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
