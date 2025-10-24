using System.Collections.Generic;
using System.Linq;
using Dark.Tools.Utils;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace InGame
{
    public class ProjectileManifest : SerializedScriptableObject
    {
        public static string Path = "ProjectileManifest";
        private static string FilePath = "Assets/Dark/Resources/ProjectileManifest.asset";

        public Dictionary<int, ProjectileEntity> prefabsMap;
        
        public static ProjectileEntity GetRandom()
        {
            var instance = Resources.Load<ProjectileManifest>(Path);
            if (instance.prefabsMap == null || instance.prefabsMap.Count == 0) return null;
            var result = instance.prefabsMap.Values.ToArray()[Random.Range(0, instance.prefabsMap.Count)];
            Resources.UnloadAsset(instance);
            return result;
        }

        public static ProjectileEntity Get(int id)
        {
            var instance = Resources.Load<ProjectileManifest>(FilePath);
            if (instance.prefabsMap == null || instance.prefabsMap.Count == 0) return null;
            var result = instance.prefabsMap.GetValueOrDefault(id);
            Resources.UnloadAsset(instance);
            return result;
        }

#if UNITY_EDITOR
        public static ProjectileEntity EditorGet(int id)
        {
            var instance = AssetDatabase.LoadAssetAtPath<ProjectileManifest>(FilePath);
            if (instance.prefabsMap == null || instance.prefabsMap.Count == 0) return null;
            return instance.prefabsMap.GetValueOrDefault(id);
        }
#endif
        
#if UNITY_EDITOR
        [MenuItem("Dark/Manifest/Generate Projectile Manifest")]
        public static void CreateInstance()
        {
            AssetDatabaseUtils.CreateSOInstance<ProjectileManifest>(FilePath);
        }
#endif
    }
}