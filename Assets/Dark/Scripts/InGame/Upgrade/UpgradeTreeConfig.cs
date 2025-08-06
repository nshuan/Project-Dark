using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace InGame.Upgrade
{
    /// <summary>
    /// An instance of tree config should be in the same folder with the folder containing node configs
    /// Name of the instance should be the same as name of the folder containing node configs
    /// </summary>
    [CreateAssetMenu(menuName = "Dark/Upgrade/Upgrade Tree Config", fileName = "UpgradeTreeConfig")]
    public class UpgradeTreeConfig : SerializedScriptableObject
    {
        [NonSerialized, OdinSerialize] private Dictionary<int, UpgradeNodeConfig> nodeMapById = new Dictionary<int, UpgradeNodeConfig>();

        public int TotalNodes => nodeMapById.Count;
        
        public void ActivateTree(List<UpgradeNodeData> nodeData, ref UpgradeBonusInfo bonusInfo)
        {
            foreach (var node in nodeData)
            {
                if (nodeMapById.TryGetValue(node.id, out var nodeConfig))
                {
                    nodeConfig.ActivateNode(node.level, ref bonusInfo);
                }
            }
        }

        public UpgradeNodeConfig GetNodeById(int id)
        {
            return nodeMapById.GetValueOrDefault(id);
        }

#if UNITY_EDITOR
        [Button]
        public void GetConfigsFromPath()
        {
            var treeConfigPath = UnityEditor.AssetDatabase.GetAssetPath(this);
            var configFolderPath = Path.Combine(
                Path.GetDirectoryName(treeConfigPath),
                Path.GetFileNameWithoutExtension(treeConfigPath)
            );
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + nameof(UpgradeNodeConfig), new[] { configFolderPath });
            List<UpgradeNodeConfig> assets = new List<UpgradeNodeConfig>();

            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                UpgradeNodeConfig asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UpgradeNodeConfig>(path);
                if (asset != null)
                    assets.Add(asset);
            }

            nodeMapById = assets.Select((config) => new KeyValuePair<int,UpgradeNodeConfig>(config.nodeId, config)).ToDictionary(x => x.Key, x => x.Value);
            EditorUtility.SetDirty(this);
        }
#endif
    }
}