using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dark.Tools.GoogleSheetTool;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dark.Scripts.Common.Lore
{
    public class LoreManifest : SerializedScriptableObject
    {
        public static string Path = "LoreManifest";
        private static string FilePath = "Assets/Dark/Resources/LoreManifest.asset";

        public Dictionary<LoreKey, LoreInfo> loreMap;
        
        public static LoreInfo GetRandom()
        {
            var instance = Resources.Load<LoreManifest>(Path);
            if (instance.loreMap == null || instance.loreMap.Count == 0) return new LoreInfo();
            var result = instance.loreMap.Values.ToArray()[Random.Range(0, instance.loreMap.Count)];
            Resources.UnloadAsset(instance);
            return result;
        }
        
#if UNITY_EDITOR
        [MenuItem("Dark/Lore/Generate Lore Manifest")]
        public static void CreateInstance()
        {
            if (File.Exists(FilePath))
            {
                var instance = AssetDatabase.LoadAssetAtPath<LoreManifest>(FilePath);
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = instance;   
                Debug.LogError("Lore Manifest file already exists!");
                return;
            }
            
            LoreManifest asset = ScriptableObject.CreateInstance<LoreManifest>();

            // Create and save the asset
            AssetDatabase.CreateAsset(asset, FilePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Select the new asset
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;    
        }
#endif
    }

    public enum LoreKey
    {
        TheSightSunder,
        EchoPiercer
    }

    [Serializable]
    public struct LoreInfo
    {
        public string name;
        public string lore;
    }
}