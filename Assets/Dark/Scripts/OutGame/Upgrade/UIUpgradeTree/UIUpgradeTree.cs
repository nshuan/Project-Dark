using System;
using System.Collections.Generic;
using System.Linq;
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
        [Space] [Header("Editor")] 
        [SerializeField] private string spritesPath = "Assets/Dark/Config/Upgrade/Skill_Tree_Sprites";
        
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
                if (node.config.nodeSprite && node.config.nodeSpriteLock)
                    node.SetVisual(node.config.nodeSprite, node.config.nodeSpriteLock);
            }

            foreach (var node in nodes)
            {
                node.preRequireLines = new List<UIUpgradeLineInfo>();
                
                if (node.config.preRequire == null || node.config.preRequire.Length == 0) continue;
                foreach (var preRequireConfig in node.config.preRequire)
                {
                    var preRequireNode = GetNodeById(preRequireConfig.nodeId);
                    if (!preRequireNode) continue;
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
        
        private Dictionary<int, NodeSpriteInfo> GetSpritesMapById()
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

        [Serializable]
        class NodeSpriteInfo
        {
            public Sprite normalSprite;
            public Sprite lockedSprite;
        }
#endif
    }
}
