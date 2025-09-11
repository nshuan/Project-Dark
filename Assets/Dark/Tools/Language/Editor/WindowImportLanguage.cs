using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Dark.Tools.Language.Runtime;
using Dark.Tools.Utils;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace Dark.Tools.Language.Editor
{
    /// <summary>
    /// Nếu thêm ngôn ngữ thì phải goi Generate Language Enum trước
    /// để tạo script enum cho Unity compile xong đã
    /// </summary>
    public class WindowImportLanguage
    {
        private const string EnumClassName =  "LanguageType";
        private const string EnumPath = "Assets/Dark/Tools/Language/Runtime/LanguageType.cs";

        [MenuItem("Dark/Language/Generate Language Enum")]
        public static void GenerateLanguageEnum()
        {
            var sheetLink = AssetDatabaseUtils.CreateSOInstance<LanguageConfig>(LanguageConfig.Path).dataSheetLink;
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
            UtilGenerateEnum.GenerateEnumScript(EnumPath, EnumClassName, languageNames.ToList(), null, "Dark.Tools.Language");
        }
        
        [MenuItem("Dark/Language/Import Languages")]
        public static void Import()
        {
            if (!File.Exists(EnumPath))
            {
                Debug.LogError("Generate language enum first!");
                return;
            }
            
            var sheetLink = AssetDatabase.LoadAssetAtPath<LanguageConfig>(LanguageConfig.Path).dataSheetLink;
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
            if (!File.Exists(LanguageData.Path))
                GenerateLanguageConfig.CreateDataInstance();
            var config = AssetDatabase.LoadAssetAtPath<LanguageData>(LanguageData.Path);
            config.dataMap = new Dictionary<string, LanguageItem>();
            
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
                LanguageItem data = new LanguageItem();
                data.languageMap = new Dictionary<LanguageType, string>();
            
                for (int col = 1; col < headers.Length; col++)
                {
                    if (string.IsNullOrEmpty(values[col])) continue;
                    LanguageType type = (LanguageType)System.Enum.Parse(typeof(LanguageType), languageNames[col - 1]);
                    data.languageMap.TryAdd(type, values[col]);
                }
            
                config.dataMap.TryAdd(key, data);
            }
            
            // Save ScriptableObject
            EditorUtility.SetDirty(config);
            AssetDatabase.Refresh();
            
            Debug.Log("Language imported successfully!");
        }
    }
}

#endif
