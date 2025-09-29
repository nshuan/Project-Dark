using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dark.Tools.Utils;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace InGame.ConfigManager
{
    public class ClassConfigManifest : SerializedScriptableObject
    {
        private static string Path = "ClassConfigManifest";
        private static string FilePath = "Assets/Dark/Resources/ClassConfigManifest.asset";
        
        [ReadOnly, NonSerialized, OdinSerialize]
        private PlayerSkillConfig[] skillConfig;

        public static PlayerSkillConfig GetConfig(int id)
        {
            var instance = Resources.Load<ClassConfigManifest>(Path);
            if (instance.skillConfig == null || instance.skillConfig.Length == 0) return null;
            var result = instance.skillConfig.FirstOrDefault((config) => config.skillId == id);
            Resources.UnloadAsset(instance);
            return result;
        }
        
#if UNITY_EDITOR
        private const string PATH_CLASS_CONFIG = "Assets/Dark/Config/PlayerSkill";
        [Button]
        public void ValidateSkillConfig()
        {
            skillConfig = AssetUtility.LoadAllScriptableObjectsInFolder<PlayerSkillConfig>(PATH_CLASS_CONFIG).ToArray();
        }
        
        [MenuItem("Dark/Manifest/Generate Class Config Manifest")]
        public static void CreateInstance()
        {
            AssetDatabaseUtils.CreateSOInstance<ClassConfigManifest>(FilePath);
        }
#endif
    }
}