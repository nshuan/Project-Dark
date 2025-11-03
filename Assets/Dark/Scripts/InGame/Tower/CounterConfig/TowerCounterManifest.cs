using System.Collections.Generic;
using System.Linq;
using Dark.Tools.Utils;
using InGame.Upgrade;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace InGame.CounterConfig
{
    public class TowerCounterManifest : SerializedScriptableObject
    {
        public static string Path = "TowerCounterManifest";
        private static string FilePath = "Assets/Dark/Resources/TowerCounterManifest.asset";

        public Dictionary<NodeTowerCounter.CounterType, TowerCounterConfig> configMap;
        
        public static TowerCounterConfig GetRandom()
        {
            var instance = Resources.Load<TowerCounterManifest>(Path);
            if (instance.configMap == null || instance.configMap.Count == 0) return null;
            var result = instance.configMap.Values.ToArray()[Random.Range(0, instance.configMap.Count)];
            Resources.UnloadAsset(instance);
            return result;
        }

        public static TowerCounterConfig Get(NodeTowerCounter.CounterType type)
        {
            var instance = Resources.Load<TowerCounterManifest>(Path);
            if (instance.configMap == null || instance.configMap.Count == 0) return null;
            var result = instance.configMap.GetValueOrDefault(type);
            Resources.UnloadAsset(instance);
            return result;
        }

#if UNITY_EDITOR
        public static TowerCounterConfig EditorGet(NodeTowerCounter.CounterType type)
        {
            var instance = AssetDatabase.LoadAssetAtPath<TowerCounterManifest>(FilePath);
            if (instance.configMap == null || instance.configMap.Count == 0) return null;
            return instance.configMap.GetValueOrDefault(type);
        }

        private static string ConfigFolderPath = "Assets/Dark/Config/TowerCounter";
        public static void GetAllConfig()
        {
            var instance = AssetDatabase.LoadAssetAtPath<TowerCounterManifest>(FilePath);
            
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + nameof(TowerCounterConfig), new[] { ConfigFolderPath });
            List<TowerCounterConfig> assets = new List<TowerCounterConfig>();

            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                TowerCounterConfig asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TowerCounterConfig>(path);
                if (asset != null)
                    assets.Add(asset);
            }
            
            instance.configMap = assets.Select((config) => new KeyValuePair<int,TowerCounterConfig>(config.id, config)).ToDictionary(x => (NodeTowerCounter.CounterType)x.Key, x => x.Value);
            EditorUtility.SetDirty(instance);
        }
#endif
        
#if UNITY_EDITOR
        [MenuItem("Dark/Manifest/Tower Counter Manifest")]
        public static void CreateInstance()
        {
            AssetDatabaseUtils.CreateSOInstance<TowerCounterManifest>(FilePath);
        }
#endif
    }
}