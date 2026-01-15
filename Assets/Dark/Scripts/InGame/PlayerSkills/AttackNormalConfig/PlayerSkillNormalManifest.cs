using System.Collections.Generic;
using System.Linq;
using Dark.Tools.Utils;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace InGame.AttackNormalConfig
{
    public class PlayerSkillNormalManifest : SerializedScriptableObject
    {
        public static string Path = "PlayerSkillNormalManifest";
        private static string FilePath = "Assets/Dark/Resources/PlayerSkillNormalManifest.asset";

        public Dictionary<NormalType, PlayerSkillNormalConfig> configMap;
        
        public static PlayerSkillNormalConfig GetRandom()
        {
            var instance = Resources.Load<PlayerSkillNormalManifest>(Path);
            if (instance.configMap == null || instance.configMap.Count == 0) return null;
            var result = instance.configMap.Values.ToArray()[RandomUtil.Range(0, instance.configMap.Count)];
            Resources.UnloadAsset(instance);
            return result;
        }

        public static PlayerSkillNormalConfig Get(NormalType type)
        {
            var instance = Resources.Load<PlayerSkillNormalManifest>(Path);
            if (instance.configMap == null || instance.configMap.Count == 0) return null;
            var result = instance.configMap.GetValueOrDefault(type);
            Resources.UnloadAsset(instance);
            return result;
        }

#if UNITY_EDITOR
        public static PlayerSkillNormalConfig EditorGet(NormalType type)
        {
            var instance = AssetDatabase.LoadAssetAtPath<PlayerSkillNormalManifest>(FilePath);
            if (instance.configMap == null || instance.configMap.Count == 0) return null;
            return instance.configMap.GetValueOrDefault(type);
        }

        private static string ConfigFolderPath = "Assets/Dark/Config/SkillNormalConfig";
        public static void GetAllConfig()
        {
            var instance = AssetDatabase.LoadAssetAtPath<PlayerSkillNormalManifest>(FilePath);
            
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + nameof(PlayerSkillNormalConfig), new[] { ConfigFolderPath });
            List<PlayerSkillNormalConfig> assets = new List<PlayerSkillNormalConfig>();

            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                PlayerSkillNormalConfig asset = UnityEditor.AssetDatabase.LoadAssetAtPath<PlayerSkillNormalConfig>(path);
                if (asset != null)
                    assets.Add(asset);
            }
            
            instance.configMap = assets.Select((config) => new KeyValuePair<int,PlayerSkillNormalConfig>(config.id, config)).ToDictionary(x => (NormalType)x.Key, x => x.Value);
            EditorUtility.SetDirty(instance);
        }
#endif
        
#if UNITY_EDITOR
        [MenuItem("Dark/Manifest/Player Skill Normal Manifest")]
        public static void CreateInstance()
        {
            AssetDatabaseUtils.CreateSOInstance<PlayerSkillNormalManifest>(FilePath);
        }
#endif
    }
}