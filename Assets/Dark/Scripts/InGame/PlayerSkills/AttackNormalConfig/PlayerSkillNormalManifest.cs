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

        public Dictionary<NormalType, PlayerSkillNormalConfig> archerConfigMap;
        public Dictionary<NormalType, KnightSkillNormalConfig> knightConfigMap;
        
        public static PlayerSkillNormalConfig GetRandom(CharacterClass.CharacterClass classType)
        {
            var instance = Resources.Load<PlayerSkillNormalManifest>(Path);
            PlayerSkillNormalConfig result = null;
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

        public static PlayerSkillNormalConfig Get(CharacterClass.CharacterClass classType, NormalType type)
        {
            var instance = Resources.Load<PlayerSkillNormalManifest>(Path);
            PlayerSkillNormalConfig result = null;

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
        public static PlayerSkillNormalConfig EditorGet(CharacterClass.CharacterClass classType, NormalType type)
        {
            var instance = AssetDatabase.LoadAssetAtPath<PlayerSkillNormalManifest>(FilePath);
            
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

        private static string ConfigFolderPath = "Assets/Dark/Config/SkillNormalConfig";
        [Button]
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

            instance.archerConfigMap = assets.Where((config) => config is not KnightSkillNormalConfig).Select((config) => new KeyValuePair<int, PlayerSkillNormalConfig>(config.id, config)).ToDictionary(x => (NormalType)x.Key, x => x.Value);
            instance.knightConfigMap = assets.Where((config) => config is KnightSkillNormalConfig).Select((knightConfig) => new KeyValuePair<int, KnightSkillNormalConfig>(knightConfig.id, (KnightSkillNormalConfig)knightConfig)).ToDictionary(x => (NormalType)x.Key, x => x.Value);
            
            
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