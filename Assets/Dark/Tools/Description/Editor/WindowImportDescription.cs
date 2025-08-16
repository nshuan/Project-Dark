using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Dark.Tools.Description.Runtime;
using Dark.Tools.Utils;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace Dark.Tools.Description.Editor
{
    /// <summary>
    /// Nếu thêm ngôn ngữ thì phải goi Generate Language Enum trước
    /// để tạo script enum cho Unity compile xong đã
    /// </summary>
    public class WindowImportDescription
    {
        private const string EnumClassName =  "DescriptionType";
        private const string EnumPath = "Assets/Dark/Tools/Description/Runtime/DescriptionType.cs";

        [MenuItem("Dark/Description/Generate Language Enum")]
        public static void GenerateLanguageEnum()
        {
            var sheetLink = AssetDatabase.LoadAssetAtPath<DescriptionConfig>(DescriptionConfig.Path).dataSheetLink;
            if (string.IsNullOrEmpty(sheetLink))
            {
                Debug.LogError("Sheet link is empty!");
                return;
            }

            var url = sheetLink;

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
            
            // First row: headers
            string[] headers = csvTable[0];
            List<string> languageNames = new List<string>();
            
            for (int i = 1; i < headers.Length; i++)
                languageNames.Add(headers[i]);
            
            // Generate enum file
            UtilGenerateEnum.GenerateEnumScript(EnumPath, EnumClassName, languageNames.ToList(), null, "Dark.Tools.Description");
        }
        
        [MenuItem("Dark/Description/Import Descriptions")]
        public static void Import()
        {
            if (!File.Exists(EnumPath))
            {
                Debug.LogError("Generate language enum first!");
                return;
            }
            
            var sheetLink = AssetDatabase.LoadAssetAtPath<DescriptionConfig>(DescriptionConfig.Path).dataSheetLink;
            if (string.IsNullOrEmpty(sheetLink))
            {
                Debug.LogError("Sheet link is empty!");
                return;
            }
            
            var url = sheetLink;

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
            if (!File.Exists(DescriptionData.Path))
                GenerateDescriptionConfig.CreateDataInstance();
            var config = AssetDatabase.LoadAssetAtPath<DescriptionData>(DescriptionData.Path);
            config.dataMap = new Dictionary<string, DescriptionItem>();
            
            // First row: headers
            string[] headers = csvTable[0];
            List<string> languageNames = new List<string>();
            for (int i = 1; i < headers.Length; i++)
                languageNames.Add(headers[i]);
            
            // Parse rows
            for (int row = 1; row < lines.Length; row++)
            {
                string[] values = csvTable[row];
                if (values.Length < 2) continue; // skip empty
            
                string key = values[0];
                DescriptionItem data = new DescriptionItem();
                data.descriptionMap = new Dictionary<DescriptionType, string>();
            
                for (int col = 1; col < headers.Length; col++)
                {
                    if (string.IsNullOrEmpty(values[col])) continue;
                    DescriptionType type = (DescriptionType)System.Enum.Parse(typeof(DescriptionType), languageNames[col - 1]);
                    data.descriptionMap.TryAdd(type, values[col]);
                }
            
                config.dataMap.TryAdd(key, data);
            }
            
            // Save ScriptableObject
            EditorUtility.SetDirty(config);
            AssetDatabase.Refresh();
            
            Debug.Log("Descriptions imported successfully!");
        }
    }
}

#endif
