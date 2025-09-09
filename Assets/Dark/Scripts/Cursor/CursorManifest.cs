using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InGame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace Dark.Scripts.Cursor
{
    public class CursorManifest : SerializedScriptableObject
    {
        private static string Path = "CursorManifest";
        private static string FilePath = "Assets/Dark/Resources/CursorManifest.asset";
        
        [NonSerialized, OdinSerialize] private Dictionary<CursorKind, CursorInfo> cursorMap = new Dictionary<CursorKind, CursorInfo>();

        public static CursorInfo GetCursorInfo(CursorKind kind)
        {
            var instance = Resources.Load<CursorManifest>(Path);
            if (instance.cursorMap == null || instance.cursorMap.Count == 0) return new CursorInfo();
            var result = instance.cursorMap.FirstOrDefault((config) => config.Key == kind);
            Resources.UnloadAsset(instance);
            return result.Value;
        }
        
#if UNITY_EDITOR
        [MenuItem("Dark/Manifest/Generate Cursor Manifest")]
        public static void CreateInstance()
        {
            if (File.Exists(FilePath))
            {
                var instance = AssetDatabase.LoadAssetAtPath<CursorManifest>(FilePath);
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = instance;   
                Debug.LogError("Cursor Manifest file already exists!");
                return;
            }
            
            CursorManifest asset = ScriptableObject.CreateInstance<CursorManifest>();

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
    
    public enum CursorKind
    {
        OutGame,
        InGame
    }
    
    [Serializable]
    public struct CursorInfo
    {
        public Texture2D tex;
        public Vector2 hotSpot;
        public CursorMode mode;
    }
}