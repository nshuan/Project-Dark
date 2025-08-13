#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public static class GenerateGoogleSheetsEnum
    {
        // ======= CONFIG =======
        // Example: https://docs.google.com/spreadsheets/d/<SPREADSHEET_ID>/edit
        private const string SpreadsheetId = "1EeL2LxGsFcmdnR9xg6cvTWSgvDIQHNJ_V4xq-zcLI8Q";
        private const string ApiKey        = "AIzaSyAr352v95jUQKhTfpi7XpC_Dk-hCJIlGpY";

        private const string EnumName   = "GoogleSheetTabs";                 // enum type name
        private const string OutputPath = "Assets/Dark/Tools/GoogleSheetTools/GoogleSheetTabs.cs"; // where to write the enum file
        // ======================

        [MenuItem("Tools/Dark/Google Sheets/Generate Enum From Tabs (API)")]
        public static void GenerateFromApi()
        {
            if (string.IsNullOrEmpty(SpreadsheetId) || string.IsNullOrEmpty(ApiKey))
            {
                Debug.LogError("Please set SpreadsheetId and ApiKey in GenerateGoogleSheetsEnum.");
                return;
            }

            var url = $"https://sheets.googleapis.com/v4/spreadsheets/{SpreadsheetId}?fields=sheets.properties.title&key={ApiKey}";
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

                List<string> sheetNames;
                try
                {
                    sheetNames = isApi ? ParseSheetNamesFromApiJson(req.downloadHandler.text)
                                       : ParseSheetNamesFromXlsx(req.downloadHandler.data);
                }
                catch (Exception e)
                {
                    Debug.LogError("Parsing error: " + e);
                    yield break;
                }

                if (sheetNames == null || sheetNames.Count == 0)
                {
                    Debug.LogWarning("No sheets found.");
                    yield break;
                }

                var enumCode = BuildEnumSource(EnumName, sheetNames);
                var directory = Path.GetDirectoryName(OutputPath);
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
                File.WriteAllText(OutputPath, enumCode, new UTF8Encoding(false));
                AssetDatabase.Refresh();

                Debug.Log($"Generated enum '{EnumName}' with {sheetNames.Count} entries at: {OutputPath}");
            }
        }

        // --- Parse JSON from Sheets API v4: { "sheets":[ { "properties": { "title":"Sheet1" }}, ... ] }
        [Serializable] private class SheetProps { public string title; }
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

        // --- Parse XLSX (no external libs, super-light “peek” for sheet names)
        // This minimal reader scans [Content_Types].xml and xl/workbook.xml for sheet titles.
        // For robust .xlsx parsing, consider NPOI, but this works for getting tab names without extra packages.
        private static List<string> ParseSheetNamesFromXlsx(byte[] xlsxBytes)
        {
            // XLSX is a zip; we’ll extract xl/workbook.xml and read <sheet name="...">
            // We’ll use System.IO.Compression which is available in .NET 4.x (Player settings: Api Compatibility Level .NET 4.x)
            using (var ms = new MemoryStream(xlsxBytes))
            using (var archive = new System.IO.Compression.ZipArchive(ms, System.IO.Compression.ZipArchiveMode.Read))
            {
                var entry = archive.GetEntry("xl/workbook.xml");
                if (entry == null) throw new Exception("workbook.xml not found in xlsx");

                using (var stream = entry.Open())
                using (var sr = new StreamReader(stream, Encoding.UTF8))
                {
                    var xml = sr.ReadToEnd();
                    // crude but effective: match name="Sheet Name"
                    var matches = Regex.Matches(xml, "name=\"([^\"]+)\"");
                    var list = new List<string>();
                    foreach (Match m in matches)
                    {
                        var name = m.Groups[1].Value;
                        if (!string.IsNullOrEmpty(name)) list.Add(name);
                    }
                    return list;
                }
            }
        }

        private static string BuildEnumSource(string enumName, List<string> rawNames)
        {
            var used = new HashSet<string>();
            var sb = new StringBuilder();
            sb.AppendLine("// Auto-generated. Do not edit by hand.");
            sb.AppendLine("namespace GoogleSheetTool");
            sb.AppendLine("{");
            sb.AppendLine($"    public enum {enumName}");
            sb.AppendLine("    {");

            foreach (var raw in rawNames)
            {
                var ident = ToValidIdentifier(raw);
                // ensure unique
                var final = ident;
                int suffix = 1;
                while (used.Contains(final))
                    final = ident + "_" + (++suffix).ToString();

                used.Add(final);
                sb.AppendLine($"        {final}, // \"{raw}\"");
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
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
    }

#endif
