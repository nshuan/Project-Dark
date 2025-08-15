using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using GoogleSheetTool;
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
        var url = $"https://docs.google.com/spreadsheets/d/{GoogleSheetConst.SpreadsheetId}/export?format=csv&gid={(int)tabName}";

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

        var csvTable = ParseCsv(csvContent);

        var listDataToUpdate = gsConfig.data.Where((data) => data.sheetName == tabName).ToArray();
        if (listDataToUpdate.Length == 0)
        {
            Debug.LogError($"No Data with sheet name {tabName} found!");
            return;
        }
        
        foreach (var data in listDataToUpdate)
        {
            ConfigImporter.Import(data.configs, csvTable);
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Enemy data imported successfully from tab \"{tabName}\".");
    }
    
    private List<string[]> ParseCsv(string csvContent)
    {
        var rows = new List<string[]>();
        var currentRow = new List<string>();
        var currentValue = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < csvContent.Length; i++)
        {
            char c = csvContent[i];

            if (c == '\"')
            {
                if (inQuotes && i + 1 < csvContent.Length && csvContent[i + 1] == '\"')
                {
                    // Escaped quote
                    currentValue.Append('\"');
                    i++; // Skip next quote
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                currentRow.Add(currentValue.ToString());
                currentValue.Clear();
            }
            else if ((c == '\n' || c == '\r') && !inQuotes)
            {
                // Handle \r\n (Windows newlines)
                if (c == '\r' && i + 1 < csvContent.Length && csvContent[i + 1] == '\n')
                    i++;

                currentRow.Add(currentValue.ToString());
                rows.Add(currentRow.ToArray());

                currentRow = new List<string>();
                currentValue.Clear();
            }
            else
            {
                currentValue.Append(c);
            }
        }

        // Add final value and row if needed
        if (inQuotes)
        {
            throw new FormatException("Malformed CSV: unmatched quote.");
        }

        if (currentValue.Length > 0 || currentRow.Count > 0)
        {
            currentRow.Add(currentValue.ToString());
            rows.Add(currentRow.ToArray());
        }

        return rows;
    }
}