using System.Collections.Generic;
using System.Linq;
using Dark.Tools.Utils;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace InGame
{
    public class GateManifest : SerializedScriptableObject
    {
        public static string Path = "GateManifest";
        private static string FilePath = "Assets/Dark/Resources/GateManifest.asset";

        public Dictionary<int, GateEntity> prefabsMap;
        
        public static GateEntity GetRandom()
        {
            var instance = Resources.Load<GateManifest>(Path);
            if (instance.prefabsMap == null || instance.prefabsMap.Count == 0) return null;
            var result = instance.prefabsMap.Values.ToArray()[RandomUtil.Range(0, instance.prefabsMap.Count)];
            Resources.UnloadAsset(instance);
            return result;
        }

        public static GateEntity Get(int id)
        {
            var instance = Resources.Load<GateManifest>(Path);
            if (instance.prefabsMap == null || instance.prefabsMap.Count == 0) return null;
            var result = instance.prefabsMap.GetValueOrDefault(id);
            Resources.UnloadAsset(instance);
            return result;
        }

        public static Dictionary<int, GateEntity> GetAll()
        {
            var instance = Resources.Load<GateManifest>(Path);
            if (instance.prefabsMap == null || instance.prefabsMap.Count == 0) return null;
            var result = new Dictionary<int, GateEntity>();
            foreach (var pair in instance.prefabsMap)
            {
                result.Add(pair.Key, pair.Value);
            }
            Resources.UnloadAsset(instance);
            return result;
        }

#if UNITY_EDITOR
        public static GateEntity EditorGet(int id)
        {
            var instance = AssetDatabase.LoadAssetAtPath<GateManifest>(FilePath);
            if (instance.prefabsMap == null || instance.prefabsMap.Count == 0) return null;
            return instance.prefabsMap.GetValueOrDefault(id);
        }
#endif
        
#if UNITY_EDITOR
        [MenuItem("Dark/Manifest/Generate Gate Manifest")]
        public static void CreateInstance()
        {
            AssetDatabaseUtils.CreateSOInstance<GateManifest>(FilePath);
        }
#endif
    }
}