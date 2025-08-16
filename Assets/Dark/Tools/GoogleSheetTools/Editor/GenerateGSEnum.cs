#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Dark.Tools.GoogleSheetTool;
using Unity.EditorCoroutines.Editor;
using Dark.Tools.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public static class GenerateGoogleSheetsEnum
{
    // ======= CONFIG =======
    private const string EnumName   = "GoogleSheetTabs";                 // enum type name
    private const string OutputPath = "Assets/Dark/Tools/GoogleSheetTools/GoogleSheetTabs.cs"; // where to write the enum file
    // ======================

    [MenuItem("Dark/Google Sheets/Generate Enum From Tabs (API)")]
    public static void GenerateFromApi()
    {
        var gsConfig = AssetDatabase.LoadAssetAtPath<GoogleSheetConfig>(GoogleSheetConfig.Path);
        if (string.IsNullOrEmpty(gsConfig.sheetId) || string.IsNullOrEmpty(gsConfig.sheetApiKey))
        {
            Debug.LogError("Please set SpreadsheetId and ApiKey in GoogleSheetConfig.asset.");
            return;
        }

        var url = $"https://sheets.googleapis.com/v4/spreadsheets/{gsConfig.sheetId}?fields=sheets.properties(title,sheetId)&key={gsConfig.sheetApiKey}";
        EditorCoroutineUtility.StartCoroutineOwnerless(FetchAndWriteEnum(url, isApi:true));
    }

    private static System.Collections.IEnumerator FetchAndWriteEnum(string url, bool isApi)
    {
        using (var req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();

#if UNITY_2020_2_OR_NEWER
            if (req.result != UnityWebRequest.Result.Success)
#else
            if (req.isNetworkError || req.isHttpError)
#endif
            {
                Debug.LogError($"Request failed: {req.error}\n{url}");
                yield break;
            }

            List<TabInfo> tabs;
            try { tabs = ParseTabsFromApiJson(req.downloadHandler.text); }
            catch (Exception e)
            {
                Debug.LogError("Parsing error: " + e);
                yield break;
            }

            if (tabs == null || tabs.Count == 0)
            {
                Debug.LogWarning("No sheets found.");
                yield break;
            }

            UtilGenerateEnum.GenerateEnumScript(OutputPath, EnumName, tabs.Select((tab) => tab.Title).ToList(),
                tabs.Select((tab) => tab.Gid).ToList(), "Dark.Tools.GoogleSheetTool");
        }
    }

    // --- Parse JSON from Sheets API v4: { "sheets":[ { "properties": { "title":"Sheet1" }}, ... ] }
    [Serializable] private class SheetProps { public string title; public int sheetId; }
    [Serializable] private class Sheet      { public SheetProps properties; }
    [Serializable] private class Root       { public List<Sheet> sheets; }

    private static List<TabInfo> ParseTabsFromApiJson(string json)
    {
        var root = JsonUtility.FromJson<Root>(json);
        var outList = new List<TabInfo>();
        if (root?.sheets != null)
        {
            foreach (var s in root.sheets)
            {
                var p = s?.properties;
                if (!string.IsNullOrEmpty(p?.title))
                    outList.Add(new TabInfo { Title = p.title, Gid = p.sheetId });
            }
        }
        return outList;
    }
}

public struct TabInfo
{
    public string Title; // e.g. "GameplayConfig"
    public int Gid;      // e.g. 123456789
}

#endif
