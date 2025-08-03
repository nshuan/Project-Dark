using System;
using System.IO;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade.UIUpgradeTreeCreator
{
    public class JsonToTreePrefabConverter : SerializedMonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private UICreatorConfigLoader configLoader;
        [SerializeField] private UIUpgradeTree treePrefab;
        
        [Space]
        [Header("Prefabs mapped by ID")]
        [OdinSerialize, NonSerialized] private Dictionary<NodeType, GameObject> nodePrefabs;

        [Space] [Header("Information")] 
        [SerializeField] private string treeJsonFilename;
        [SerializeField] private string outPutTreeName;
    
        private string jsonFilePath = Application.dataPath + "/Dark/JSON/";
        private string outputPrefabPath = "Assets/Dark/Prefabs/UIUpgradeTrees/";
    
    #if UNITY_EDITOR
        [Button]
        public void ConvertJsonToPrefab()
        {
            var truePath = jsonFilePath + treeJsonFilename + ".json";
            if (!File.Exists(truePath))
            {
                Debug.LogError("JSON file not found: " + truePath);
                return;
            }
    
            string json = File.ReadAllText(truePath);
            TreeDataStruct treeData = JsonUtility.FromJson<TreeDataStruct>(json);
            if (treeData == null || treeData.nodes == null)
            {
                Debug.LogError("Invalid JSON data.");
                return;
            }
    
            var root = Instantiate(treePrefab.gameObject, canvas.transform);
    
            foreach (var node in treeData.nodes)
            {
                if (!nodePrefabs.TryGetValue((NodeType)node.idPrefab, out var prefab))
                {
                    Debug.LogWarning($"Missing prefab for idPrefab: {node.idPrefab}, skipping.");
                    continue;
                }
    
                GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                go.transform.SetParent(root.transform.Find("Nodes"));
                go.transform.localPosition = node.position;
                go.GetComponent<UIUpgradeNode>().config = configLoader.GetNodeConfig(node.id);
            }
            
            root.GetComponent<UIUpgradeTree>().ValidateNodes();
    
            var outputPath = outputPrefabPath + outPutTreeName + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(root, outputPath);
            DestroyImmediate(root);
    
            Debug.Log("Prefab saved to: " + outputPath);
        }
    #endif
    }

}