using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Dark.Tools.Description.Runtime;
// using Dark.Tools.Utils;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace Dark.Tools.Description.Editor
{
    public class WindowImportDescription
    {
        [MenuItem("Tools/Dark/Description/Import Descriptions From CSV")]
        public static void Import()
        {
            if (string.IsNullOrEmpty(DescriptionConstants.SpreadsheetId))
            {
                Debug.LogError("Sheet ID or Tab Name is empty!");
                return;
            }
            
            var url = $"https://docs.google.com/spreadsheets/d/{DescriptionConstants.SpreadsheetId}/export?format=csv&gid=0";

            string csvContent;
            using (WebClient client = new WebClient())
            {
                csvContent = client.DownloadString(url);
            }

            // string[] lines = File.ReadAllLines(csvPath);
            //
            // // First row: headers
            // string[] headers = SplitCsvLine(lines[0]);
            // List<string> languageNames = new List<string>();
            //
            // for (int i = 1; i < headers.Length; i++)
            //     languageNames.Add(headers[i]);
            //
            // // Generate enum file
            // string enumPath = "Assets/Scripts/DescriptionType.cs";
            // UtilGenerateEnum.GenerateEnumScript(enumPath, languageNames.ToList(), enumPath, "Dark.Tools.Description");
            //
            // // Create ScriptableObject
            // DescriptionConfig config = AssetDatabase.LoadAssetAtPath<DescriptionConfig>(DescriptionConfig.Path);
            // config.dataMap = new Dictionary<string, DescriptionData>();
            //
            // // Parse rows
            // for (int row = 1; row < lines.Length; row++)
            // {
            //     string[] values = SplitCsvLine(lines[row]);
            //     if (values.Length < 2) continue; // skip empty
            //
            //     string key = values[0];
            //     DescriptionData data = new DescriptionData();
            //     data.descriptionMap = new Dictionary<DescriptionType, string>();
            //
            //     for (int col = 1; col < headers.Length; col++)
            //     {
            //         if (string.IsNullOrEmpty(values[col])) continue;
            //         DescriptionType type = (DescriptionType)System.Enum.Parse(typeof(DescriptionType), languageNames[col - 1]);
            //         data.descriptionMap[type] = values[col];
            //     }
            //
            //     config.dataMap[key] = data;
            // }
            //
            // // Save ScriptableObject
            // string assetPath = "Assets/DescriptionConfig.asset";
            // AssetDatabase.CreateAsset(config, assetPath);
            // AssetDatabase.SaveAssets();
            // AssetDatabase.Refresh();
            //
            // Debug.Log("Descriptions imported successfully!");
        }

        static string[] SplitCsvLine(string line)
        {
            // Basic CSV split; you can improve if needed
            return line.Split(',');
        }        
    }
}

#endif
