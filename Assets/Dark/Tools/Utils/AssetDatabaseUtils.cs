using System.IO;
using UnityEditor;
using UnityEngine;

namespace Dark.Tools.Utils
{
#if UNITY_EDITOR
    
    public class AssetDatabaseUtils
    {
        public static T CreateSOInstance<T>(string filePath) where T : ScriptableObject
        {
            if (File.Exists(filePath))
            {
                var instance = AssetDatabase.LoadAssetAtPath<T>(filePath);
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = instance;   
                Debug.LogError($"{filePath} file already exists!");
                return instance;
            }
            
            T asset = ScriptableObject.CreateInstance<T>();

            // Create and save the asset
            AssetDatabase.CreateAsset(asset, filePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Select the new asset
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            return asset;
        }
    }
    
#endif
}