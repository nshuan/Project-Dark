using System.Collections.Generic;
using System.IO;
using InGame.Upgrade;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Data
{
    public class TestSaveUpgradeTree : MonoBehaviour
    {
        // [SerializeField] private UpgradeTreeConfig treeConfig;
        //
        // private UpgradeTreeData ConvertData(UpgradeTreeConfig config)
        // {
        //     var data = new UpgradeTreeData();
        //
        //     var nodeDataMap = new Dictionary<UpgradeNodeConfig, UpgradeNodeData>();
        //     foreach (var node in config.upgradeNodes)
        //     {
        //         var nodeData = nodeDataMap.TryGetValue(node, out var d) ? d : new UpgradeNodeData(node);
        //         if (node.preRequire == null || node.preRequire.Length == 0) data.treeRoot = nodeData;
        //
        //         foreach (var preNode in node.preRequire)
        //         {
        //             var preNodeData = nodeDataMap.TryGetValue(preNode, out var pd) ? pd : new UpgradeNodeData(preNode);
        //             preNodeData.children.Add(nodeData);
        //             nodeDataMap[preNode] = preNodeData;
        //         }
        //         
        //         nodeDataMap[node] = nodeData;
        //     }
        //
        //     return data;
        // }
        //
        // [Button]
        // public void TestSave()
        // {
        //     var data = ConvertData(treeConfig);
        //     var jsonData = JsonUtility.ToJson(data);
        //     var folderPath = Application.dataPath;
        //     var fullPath = Path.Combine(folderPath, "testTreeData.json");
        //     File.WriteAllText(fullPath, jsonData);
        // }
        //
        // [Button]
        // void TestLoad(UpgradeTreeConfig targetConfig)
        // {
        //     var folderPath = Application.dataPath;
        //     var fullPath = Path.Combine(folderPath, "testTreeData.json");
        //     var json = File.ReadAllText(fullPath);
        //     var data = JsonUtility.FromJson<UpgradeTreeData>(json);
        //     
        //     targetConfig.upgradeNodes.Clear();
        //     
        // }
    }
}