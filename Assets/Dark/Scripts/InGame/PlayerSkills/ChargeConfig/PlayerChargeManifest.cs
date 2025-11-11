using System.Collections.Generic;
using System.Linq;
using Dark.Tools.Utils;
using InGame.Upgrade;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace InGame.ChargeConfig
{
    public class PlayerChargeManifest : SerializedScriptableObject
    {
        public static string Path = "PlayerChargeManifest";
        private static string FilePath = "Assets/Dark/Resources/PlayerChargeManifest.asset";

        public Dictionary<ChargeType, PlayerChargeConfig> configMap;
        
        public static PlayerChargeConfig GetRandom()
        {
            var instance = Resources.Load<PlayerChargeManifest>(Path);
            if (instance.configMap == null || instance.configMap.Count == 0) return null;
            var result = instance.configMap.Values.ToArray()[RandomUtil.Range(0, instance.configMap.Count)];
            Resources.UnloadAsset(instance);
            return result;
        }

        public static PlayerChargeConfig Get(ChargeType type)
        {
            var instance = Resources.Load<PlayerChargeManifest>(Path);
            if (instance.configMap == null || instance.configMap.Count == 0) return null;
            var result = instance.configMap.GetValueOrDefault(type);
            Resources.UnloadAsset(instance);
            return result;
        }

#if UNITY_EDITOR
        public static PlayerChargeConfig EditorGet(ChargeType type)
        {
            var instance = AssetDatabase.LoadAssetAtPath<PlayerChargeManifest>(FilePath);
            if (instance.configMap == null || instance.configMap.Count == 0) return null;
            return instance.configMap.GetValueOrDefault(type);
        }

        private static string ConfigFolderPath = "Assets/Dark/Config/ChargeConfig";
        public static void GetAllConfig()
        {
            var instance = AssetDatabase.LoadAssetAtPath<PlayerChargeManifest>(FilePath);
            
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + nameof(PlayerChargeConfig), new[] { ConfigFolderPath });
            List<PlayerChargeConfig> assets = new List<PlayerChargeConfig>();

            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                PlayerChargeConfig asset = UnityEditor.AssetDatabase.LoadAssetAtPath<PlayerChargeConfig>(path);
                if (asset != null)
                    assets.Add(asset);
            }
            
            instance.configMap = assets.Select((config) => new KeyValuePair<int,PlayerChargeConfig>(config.id, config)).ToDictionary(x => (ChargeType)x.Key, x => x.Value);
            EditorUtility.SetDirty(instance);
        }
#endif
        
#if UNITY_EDITOR
        [MenuItem("Dark/Manifest/Player Charge Manifest")]
        public static void CreateInstance()
        {
            AssetDatabaseUtils.CreateSOInstance<PlayerChargeManifest>(FilePath);
        }
#endif
    }
}