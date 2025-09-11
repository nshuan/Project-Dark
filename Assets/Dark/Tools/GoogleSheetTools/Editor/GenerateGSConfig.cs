using System.IO;
using Dark.Tools.GoogleSheetTool;
using Dark.Tools.Utils;
using UnityEditor;
using UnityEngine;

public class GenerateGSConfig
{
    [MenuItem("Dark/Google Sheets/Generate Config")]
    public static void CreateInstance()
    {
        AssetDatabaseUtils.CreateSOInstance<GoogleSheetConfig>(GoogleSheetConfig.Path);
    }
}