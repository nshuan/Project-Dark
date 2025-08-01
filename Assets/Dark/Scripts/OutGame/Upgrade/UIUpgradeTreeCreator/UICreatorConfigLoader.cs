using System;
using System.Collections.Generic;
using System.Linq;
using InGame.Upgrade;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade.UIUpgradeTreeCreator
{
    public class UICreatorConfigLoader : SerializedMonoBehaviour
    {
        public string configPath;
        [OdinSerialize, NonSerialized] private Dictionary<int, UpgradeNodeConfig> nodeConfigMap;

        public UpgradeNodeConfig GetNodeConfig(int nodeId)
        {
            return nodeConfigMap.GetValueOrDefault(nodeId);
        }
        
#if UNITY_EDITOR
        [Button]
        public void GetConfigsFromPath()
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + nameof(UpgradeNodeConfig), new[] { configPath });
            List<UpgradeNodeConfig> assets = new List<UpgradeNodeConfig>();

            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                UpgradeNodeConfig asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UpgradeNodeConfig>(path);
                if (asset != null)
                    assets.Add(asset);
            }

            nodeConfigMap = assets.Select((config) => new KeyValuePair<int,UpgradeNodeConfig>(config.nodeId, config)).ToDictionary(x => x.Key, x => x.Value);
            EditorUtility.SetDirty(this);
        }
#endif
    }
}