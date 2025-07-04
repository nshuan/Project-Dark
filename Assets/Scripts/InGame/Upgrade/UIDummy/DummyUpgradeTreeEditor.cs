using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InGame.Upgrade.UI;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace InGame.Upgrade.UIDummy
{
    public class DummyUpgradeTreeEditor : SerializedMonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private DummyUpgradeTreeConfig upgradeTreeConfig; 
        
        [Space]
        [SerializeField] private DummyUpgradeTree treePrefab;
        [SerializeField] private DummyUpgradeNode nodePrefab;
        
        private DummyUpgradeNode[] allNodes;

        [field: ReadOnly, NonSerialized, OdinSerialize]
        public Dictionary<UpgradeNodeConfig, DummyUpgradeNode> NodeMapByConfig { get; private set; } = new Dictionary<UpgradeNodeConfig, DummyUpgradeNode>();

        private void OnDrawGizmos()
        {
            if (allNodes == null) return;
            if (NodeMapByConfig == null) return;
            
            foreach (var node in allNodes)
            {
                if (node == null) continue;
                if (node.nodeConfig == null || node.nodeConfig.preRequire == null) continue;
                foreach (var preNodeConfig in node.nodeConfig.preRequire)
                {
                    if (preNodeConfig == null) continue;
                    if (!NodeMapByConfig.TryGetValue(preNodeConfig, out var preNode)) continue;
                    if (preNode == null) continue;
                    Gizmos.DrawLine(node.transform.position, preNode.transform.position);
                }
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            EditorGetAllNodes();
        }

        [Button]
        public void EditorCreateNode(UpgradeNodeConfig config)
        {
            if (config == null)
            {
                DebugUtility.LogError("No config to create!!!");
                return;
            }
            
            var node = PrefabUtility.InstantiatePrefab(nodePrefab, canvas.transform) as DummyUpgradeNode;
            node.nodeConfig = config;
            EditorGetAllNodes();
        }
        
        [Button]
        public void EditorGetAllNodes()
        {
            allNodes = FindObjectsByType<DummyUpgradeNode>(FindObjectsSortMode.None).Where((node) => node.nodeConfig != null).ToArray();
            NodeMapByConfig = allNodes.ToDictionary((node) => node.nodeConfig, (node) => node);
        }

        [Button]
        public void EditorShowNodeVisual()
        {
            if (allNodes == null) return;
            foreach (var node in allNodes)
            {
                while (node.transform.childCount > 0)
                {
                    DestroyImmediate(node.transform.GetChild(0).gameObject);
                }
                
                var nodeVisual = PrefabUtility.InstantiatePrefab(node.nodeConfig.nodePrefab, node.transform) as UIUpgradeNode;
                nodeVisual.nodeConfig = node.nodeConfig;
                nodeVisual.transform.localPosition = Vector3.zero;
                nodeVisual.gameObject.hideFlags = HideFlags.NotEditable;
                node.nodeUI = nodeVisual;
                node.HidePlaceHolder();
            }
        }

        [Button]
        public bool Validate()
        {
            var message = "[VALIDATE] ";
            
            if (allNodes == null)
            {
                DebugUtility.LogError(message + "All nodes are null!!!");
                return false;
            }

            if (allNodes.Length == 0)
            {
                DebugUtility.LogError(message + "All nodes are empty!!!");
                return false;
            }

            var rootCount = allNodes.Count((node) =>
                node.nodeConfig.preRequire == null || node.nodeConfig.preRequire.Length == 0);
            if (rootCount < 1)
            {
                DebugUtility.LogError(message + "There is no ROOT NODE!!!\n(Root node is a node with no pre-required nodes.");
                return false;
            }

            var nodesWithSameConfig = allNodes.GroupBy((node) => node.nodeConfig).FirstOrDefault((group) => group.Count() > 1);
            if (nodesWithSameConfig != null)
            {
                message = message + "These nodes has the same node config:" +
                          "\n";

                foreach (var node in nodesWithSameConfig)
                {
                    message += "\n" + node.name;
                }
                
                DebugUtility.LogError(message);
                return false;
            }
            
            var nodesWithSameId = allNodes.GroupBy((node) => node.nodeConfig.nodeId).FirstOrDefault((group) => group.Count() > 1);
            if (nodesWithSameId != null)
            {
                message = message + "These nodes has the same node ID:" +
                          "\n";
                
                foreach (var node in nodesWithSameId)
                {
                    message += "\n" + node.name;
                }
                
                DebugUtility.LogError(message);
                return false;
            }
            
            DebugUtility.LogWarning(message + "Successful!!!");
            
            return true;
        }
        
        [Button]
        public void EditorGenerateTree(string treeName)
        {
            if (string.IsNullOrEmpty(treeName))
            {
                DebugUtility.LogError("Tree name can not be empty!!!");
                return;
            }
            
            var allExistTree =
                FindObjectsByType<DummyUpgradeTree>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var existTree in allExistTree)
            {
                if (PrefabUtility.IsPartOfPrefabInstance(existTree.gameObject))
                {
                    // Unlink it by unpacking the prefab
                    PrefabUtility.UnpackPrefabInstance(existTree.gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction);
                }
                
                while (existTree.transform.childCount > 0)
                {
                    existTree.transform.GetChild(0).SetParent(null);
                }
                DestroyImmediate(existTree.gameObject);
            }
            
            EditorGetAllNodes();
            if (!Validate())
                return;

            if (allNodes == null || allNodes.Length == 0)
            {
                DebugUtility.LogError("Can not find any nodes!!!");
                return;
            }
            
            // Create new tree
            var tree = Instantiate(treePrefab, canvas.transform);
            tree.name = treeName;
            tree.Nodes = new DummyUpgradeNode[allNodes.Length];
            for (var i = 0; i < allNodes.Length; i++)
            {
                allNodes[i].transform.SetParent(tree.transform);
                tree.Nodes[i] = allNodes[i];
            }
            
            // Save prefab
            if (upgradeTreeConfig == null)
            {
                DebugUtility.LogError("Upgrade tree config is missing! Create one first.");
                return;
            }
            
            GameObject treeGameObject = tree.gameObject;
      
            string path = "Assets/Prefabs/UpgradeTrees/" + treeGameObject.name + ".prefab";

            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs/UpgradeTrees"))
                AssetDatabase.CreateFolder("Assets/Prefabs", "UpgradeTrees");
            
            if (File.Exists(path))
            {
                DebugUtility.LogError("Prefab already exists!!!");
                return;
                // bool replace = EditorUtility.DisplayDialog(
                //     "Prefab Exists",
                //     $"A prefab named '{treeGameObject.name}' already exists.\nDo you want to replace it?",
                //     "Yes", "No"
                // );
                //
                // if (!replace)
                // {
                //     Debug.Log("Save tree canceled.");
                //     return;
                // }
            }
            
            var prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(treeGameObject, path, InteractionMode.UserAction);
            upgradeTreeConfig.treePrefab = prefab.transform.GetComponent<DummyUpgradeTree>();
            
            // Copy dictionary too config
            upgradeTreeConfig.nodeMapById = new Dictionary<int, UpgradeNodeConfig>();
            foreach (var pair in NodeMapByConfig)
            {
                upgradeTreeConfig.nodeMapById.Add(pair.Key.nodeId, pair.Key);
            }
            
            Debug.Log($"Saved prefab at: {path}");
            AssetDatabase.Refresh();
            
            DebugUtility.LogWarning("Generate tree successfully!!!");
        }
#endif
    }
}