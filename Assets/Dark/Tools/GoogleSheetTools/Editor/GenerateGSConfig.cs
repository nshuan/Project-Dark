using System.IO;
using Dark.Tools.GoogleSheetTool;
using UnityEditor;
using UnityEngine;

public class GenerateGSConfig
{
    [MenuItem("Tools/Dark/Google Sheets/Generate Config")]
    public static void CreateInstance()
    {
        if (File.Exists(GoogleSheetConfig.Path))
        {
            var instance = AssetDatabase.LoadAssetAtPath<GoogleSheetConfig>(GoogleSheetConfig.Path);
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = instance;   
            Debug.LogError("Google Sheet Config file already exists!");
            return;
        }
            
        GoogleSheetConfig asset = ScriptableObject.CreateInstance<GoogleSheetConfig>();

        // Create and save the asset
        AssetDatabase.CreateAsset(asset, GoogleSheetConfig.Path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Select the new asset
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;    
    }
}