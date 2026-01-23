using System.Collections.Generic;
using OutGame.Upgrade.Tooltip;
using UnityEditor;
using UnityEngine;

namespace Dark.Tools.GoogleSheetTool
{
    public class TooltipImporter
    {
        public static void Import(ScriptableObject config, List<string[]> csvData)
        {
            if (config is not TooltipConfig ttConfig)
            {
                Debug.LogError($"Invalid tooltip config!");
                return;
            }
            
            // Header is field names
            var fields = csvData[0];

            var ttMap = new Dictionary<string, string>();
            for (int i = 1; i < csvData.Count; i++) // Skip header
            {
                var cols = csvData[i];
                if (cols == null || cols.Length == 0) continue;

                if (int.TryParse(cols[0], out var ttId))
                {
                    if (cols.Length > 1 && !string.IsNullOrEmpty(cols[1]))
                    {
                        if (cols.Length > 2 && !string.IsNullOrEmpty(cols[2]))
                        {
                            ttMap[cols[1]] = cols[2];
                        }
                        else
                        {
                            Debug.LogError($"Invalid value at line {i}");
                        }
                    }
                    else
                    {
                        Debug.LogError($"Invalid key at line {i}");
                    }
                }
                else
                {
                    Debug.LogError($"Invalid Id {cols[0]} at line {i}");
                    continue;
                }
            }

            ttConfig.tooltipMap = new Dictionary<string, string>();
            foreach (var tt in ttMap)
            {
                ttConfig.tooltipMap.Add(tt.Key, tt.Value);
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(ttConfig);

            // Select the new asset
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = ttConfig;
#endif
        }
    }
}