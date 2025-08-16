#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
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

    [MenuItem("Tools/Dark/Google Sheets/Generate Enum From Tabs (API)")]
    public static void GenerateFromApi()
    {
        if (string.IsNullOrEmpty(GoogleSheetConst.SpreadsheetId) || string.IsNullOrEmpty(GoogleSheetConst.ApiKey))
        {
            Debug.LogError("Please set SpreadsheetId and ApiKey in GenerateGoogleSheetsEnum.");
            return;
        }

        var url = $"https://sheets.googleapis.com/v4/spreadsheets/{GoogleSheetConst.SpreadsheetId}?fields=sheets.properties(title,sheetId)&key={GoogleSheetConst.ApiKey}";
        EditorCoroutineUtility.StartCoroutineOwnerless(FetchAndWriteEnum(url, isApi:true));
    }

    // // ======= OPTIONAL: XLSX FALLBACK (no API key) =======
    // // Share the sheet “Anyone with the link: Viewer”, then we can export as xlsx.
    // [MenuItem("Tools/Dark/Google Sheets/Generate Enum From Tabs (XLSX export)")]
    // public static void GenerateFromXlsx()
    // {
    //     var url = $"https://docs.google.com/spreadsheets/d/{SpreadsheetId}/export?format=xlsx";
    //     EditorCoroutineUtility.StartCoroutineOwnerless(FetchAndWriteEnum(url, isApi:false));
    // }
    // // ====================================================

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

            var code = BuildEnumWithGids(EnumName, "GoogleSheetTool", tabs);
            WriteCode(OutputPath, code);
            Debug.Log($"Generated enum '{EnumName}' (with gid values) at: {OutputPath}");
        }
    }

    // --- Parse JSON from Sheets API v4: { "sheets":[ { "properties": { "title":"Sheet1" }}, ... ] }
    [Serializable] private class SheetProps { public string title; public int sheetId; }
    [Serializable] private class Sheet      { public SheetProps properties; }
    [Serializable] private class Root       { public List<Sheet> sheets; }

    private static List<string> ParseSheetNamesFromApiJson(string json)
    {
        var root = JsonUtility.FromJson<Root>(json);
        var list = new List<string>();
        if (root?.sheets != null)
        {
            foreach (var s in root.sheets)
            {
                if (!string.IsNullOrEmpty(s?.properties?.title))
                    list.Add(s.properties.title);
            }
        }
        return list;
    }

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

    private static string BuildEnumWithGids(string enumName, string ns, List<TabInfo> tabs)
    {
        var used = new HashSet<string>();
        var sb = new StringBuilder();
        sb.AppendLine("// Auto-generated. Do not edit by hand.");
        sb.AppendLine($"namespace {ns}");
        sb.AppendLine("{");
        sb.AppendLine($"    public enum {enumName} : int");
        sb.AppendLine("    {");
        foreach (var t in tabs)
        {
            var ident = ToValidIdentifier(t.Title);
            var final = EnsureUnique(ident, used);
            sb.AppendLine($"        {final} = {t.Gid}, // \"{t.Title}\" (gid {t.Gid})");
        }
        sb.AppendLine("    }");
        sb.AppendLine();
        // Optional convenience: maps between enum and original titles
        sb.AppendLine($"    public static class {enumName}Info");
        sb.AppendLine("    {");
        sb.AppendLine($"        public static readonly System.Collections.Generic.Dictionary<" +
                      $"{enumName}, string> TitleByEnum = new System.Collections.Generic.Dictionary<{enumName}, string>");
        sb.AppendLine("        {");
        foreach (var t in tabs)
        {
            var ident = EnsureUnique(ToValidIdentifier(t.Title), null); // not adding to shared set
            sb.AppendLine($"            {{ {enumName}.{ident}, \"{EscapeString(t.Title)}\" }},");
        }
        sb.AppendLine("        };");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string EnsureUnique(string ident, HashSet<string> usedOrNull)
    {
        if (usedOrNull == null) return ident; // for read-only mapping emit
        var final = ident;
        int suffix = 1;
        while (usedOrNull.Contains(final))
            final = ident + "_" + (++suffix).ToString();
        usedOrNull.Add(final);
        return final;
    }
    
    private static string ToValidIdentifier(string s)
    {
        if (string.IsNullOrEmpty(s)) return "_Empty";
        // Replace non-alphanumeric with underscores
        string id = Regex.Replace(s, @"[^a-zA-Z0-9_]", "_");
        // If starts with digit, prefix underscore
        if (Regex.IsMatch(id, @"^[0-9]")) id = "_" + id;
        // Collapse multiple underscores
        id = Regex.Replace(id, @"_+", "_");
        // Trim underscores from ends
        id = id.Trim('_');
        return string.IsNullOrEmpty(id) ? "_Unnamed" : id;
    }
    
    private static string EscapeString(string s) => s.Replace("\\", "\\\\").Replace("\"", "\\\"");

    private static void WriteCode(string path, string code)
    {
        var dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        File.WriteAllText(path, code, new UTF8Encoding(false));
        AssetDatabase.Refresh();
    }
}

public struct TabInfo
{
    public string Title; // e.g. "GameplayConfig"
    public int Gid;      // e.g. 123456789
}

#endif
