using System;
using System.Collections.Generic;
using System.Net;
using Dark.Tools.GoogleSheetTool;
using Dark.Tools.Utils;
using InGame;
using UnityEditor;
using UnityEngine;

public class ImportDataWindow : EditorWindow
{
    public GoogleSheetTabs tabName; // The name of the tab (page) to fetch
    
    private GoogleSheetConfig gsConfig;

    [MenuItem("Tools/Dark/Google Sheets/Import Data")]
    public static void ShowWindow()
    {
        GetWindow<ImportDataWindow>("Data Importer");
    }

    void OnGUI()
    {
        SerializedObject so = new SerializedObject(this);
        tabName = (GoogleSheetTabs)EditorGUILayout.EnumPopup("TabName", tabName);

        if (GUILayout.Button("Import Data"))
        {
            ImportData();
        }

        so.ApplyModifiedProperties();
    }

    void ImportData()
    {
        if (string.IsNullOrEmpty(GoogleSheetConst.SpreadsheetId))
        {
            Debug.LogError("Sheet ID or Tab Name is empty!");
            return;
        }

        gsConfig ??= AssetDatabase.LoadAssetAtPath<GoogleSheetConfig>(GoogleSheetConfig.Path); 

        // string url = $"https://docs.google.com/spreadsheets/d/{GoogleSheetConst.SpreadsheetId}/gviz/tq?tqx=out:csv&sheet={Uri.EscapeDataString(tabName.ToString())}";
        var url = $"https://docs.google.com/spreadsheets/d/{GoogleSheetConst.SpreadsheetId}/export?format=csv&gid=0";

        string csvContent;
        using (WebClient client = new WebClient())
        {
            csvContent = client.DownloadString(url);
        }

        string[] lines = csvContent.Split('\n');
        if (lines.Length < 2)
        {
            Debug.LogError("CSV has no data!");
            return;
        }

        var csvTable = UtilCsvParser.Parse(csvContent);

        switch (tabName)
        {
            case GoogleSheetTabs.Enemy:
                EnemyConfigImporter.Import(gsConfig.enemies.configs, csvTable);
                break;
            case GoogleSheetTabs.Passive:
                break;
            default:
                break;
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Enemy data imported successfully from tab \"{tabName}\".");
    }
}