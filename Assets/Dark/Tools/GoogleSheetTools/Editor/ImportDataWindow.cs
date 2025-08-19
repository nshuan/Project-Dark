using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Dark.Tools.GoogleSheetTool;
using Dark.Tools.Utils;
using InGame;
using UnityEditor;
using UnityEngine;

public class ImportDataWindow : EditorWindow
{
    public string sheetLink;
    public GoogleSheetTabs tabName; // The name of the tab (page) to fetch
    public GoogleSheetConfig gsConfig;

    [MenuItem("Dark/Google Sheets/Import Data")]
    public static void ShowWindow()
    {
        GetWindow<ImportDataWindow>("Data Importer");
    }

    void OnGUI()
    {
        SerializedObject so = new SerializedObject(this);
        
        // Google sheet config
        gsConfig ??= AssetDatabase.LoadAssetAtPath<GoogleSheetConfig>(GoogleSheetConfig.Path);
        using (new EditorGUI.DisabledScope(true)) // disables editing
        {
            EditorGUILayout.ObjectField("Config", gsConfig, typeof(GoogleSheetConfig), false);
        }

        // Sheet link
        sheetLink = gsConfig.sheetLink;
        sheetLink = EditorGUILayout.TextField("Sheet Link", sheetLink);
        
        EditorGUILayout.Space();
        
        // Tab name
        tabName = (GoogleSheetTabs)EditorGUILayout.EnumPopup("TabName", tabName);

        if (GUILayout.Button("Import Data"))
        {
            ImportData();
        }

        so.ApplyModifiedProperties();
    }

    void ImportData()
    {
        if (string.IsNullOrEmpty(sheetLink))
        {
            Debug.LogError("Sheet ID or Tab Name is empty!");
            return;
        }

        gsConfig ??= AssetDatabase.LoadAssetAtPath<GoogleSheetConfig>(GoogleSheetConfig.Path);
        
        var url = $"{sheetLink}&gid={(int)tabName}";
        
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

        var listDataToUpdate = gsConfig.data.Where((data) => data.sheetName == tabName).ToArray();
        if (listDataToUpdate.Length == 0)
        {
            Debug.LogError($"No Data with sheet name {tabName} found!");
            return;
        }

        if (tabName.ToString().ToLower().Contains("nodeconfig"))
        {
            foreach (var data in listDataToUpdate)
            {
                ConfigNodeImporter.Import(data.configs, csvTable);
            }
        }
        else
        {
            foreach (var data in listDataToUpdate)
            {
                ConfigImporter.Import(data.configs, csvTable);
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Data imported successfully from tab \"{tabName}\".");
    }
}