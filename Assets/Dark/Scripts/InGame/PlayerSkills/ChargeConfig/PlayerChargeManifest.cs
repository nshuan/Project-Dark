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

        public Dictionary<ChargeType, PlayerChargeConfig> archerConfigMap;
        public Dictionary<ChargeType, PlayerChargeConfig> knightConfigMap;
        
        public static PlayerChargeConfig GetRandom(CharacterClass.CharacterClass classType)
        {
            var instance = Resources.Load<PlayerChargeManifest>(Path);
            PlayerChargeConfig result = null;;
            if (classType == CharacterClass.CharacterClass.Archer)
            {
                if (instance.archerConfigMap == null || instance.archerConfigMap.Count == 0) return null;
                result = instance.archerConfigMap.Values.ToArray()[RandomUtil.Range(0, instance.archerConfigMap.Count)];
            }
            else if (classType == CharacterClass.CharacterClass.Knight)
            {
                if (instance.knightConfigMap == null || instance.knightConfigMap.Count == 0) return null;
                result = instance.knightConfigMap.Values.ToArray()[RandomUtil.Range(0, instance.knightConfigMap.Count)];
            }
            Resources.UnloadAsset(instance);
            return result;
        }

        public static PlayerChargeConfig Get(CharacterClass.CharacterClass classType, ChargeType type)
        {
            var instance = Resources.Load<PlayerChargeManifest>(Path);
            PlayerChargeConfig result = null;
            if (classType == CharacterClass.CharacterClass.Archer)
            {
                if (instance.archerConfigMap == null || instance.archerConfigMap.Count == 0) return null;
                result = instance.archerConfigMap.GetValueOrDefault(type);
            }
            else if (classType == CharacterClass.CharacterClass.Knight)
            {
                if (instance.knightConfigMap == null || instance.knightConfigMap.Count == 0) return null;
                result = instance.knightConfigMap.GetValueOrDefault(type);
            }
            Resources.UnloadAsset(instance);
            return result;
        }

#if UNITY_EDITOR
        public static PlayerChargeConfig EditorGet(CharacterClass.CharacterClass classType, ChargeType type)
        {
            var instance = AssetDatabase.LoadAssetAtPath<PlayerChargeManifest>(FilePath);
            PlayerChargeConfig result = null;
            if (classType == CharacterClass.CharacterClass.Archer)
            {
                if (instance.archerConfigMap == null || instance.archerConfigMap.Count == 0) return null;
                return instance.archerConfigMap.GetValueOrDefault(type);
            }
            else if (classType == CharacterClass.CharacterClass.Knight)
            {
                if (instance.knightConfigMap == null || instance.knightConfigMap.Count == 0) return null;
                return instance.knightConfigMap.GetValueOrDefault(type);
            }

            return null;
        }

        private static string ConfigFolderPath = "Assets/Dark/Config/ChargeConfig";
        [Button]
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
            
            instance.archerConfigMap = assets.Where((config) => !config.name.ToLower().Contains("knight")).Select((config) => new KeyValuePair<int,PlayerChargeConfig>(config.id, config)).ToDictionary(x => (ChargeType)x.Key, x => x.Value);
            instance.knightConfigMap = assets.Where((config) => config.name.ToLower().Contains("knight")).Select((config) => new KeyValuePair<int,PlayerChargeConfig>(config.id, config)).ToDictionary(x => (ChargeType)x.Key, x => x.Value);
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