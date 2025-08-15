using System.IO;
using Dark.Tools.Description.Runtime;
using UnityEditor;
using UnityEngine;

namespace Dark.Tools.Description.Editor
{
    public class GenerateDescriptionConfig
    {
        [MenuItem("Tools/Dark/Description/Generate Config")]
        public static void CreateInstance()
        {
            if (File.Exists(DescriptionConfig.Path))
            {
                var instance = AssetDatabase.LoadAssetAtPath<DescriptionConfig>(DescriptionConfig.Path);
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = instance;   
                Debug.LogError("Description Config file already exists!");
                return;
            }
            
            DescriptionConfig asset = ScriptableObject.CreateInstance<DescriptionConfig>();

            // Create and save the asset
            AssetDatabase.CreateAsset(asset, DescriptionConfig.Path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Select the new asset
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;    
        }
    }
}