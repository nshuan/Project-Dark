using System.Collections.Generic;
using Dark.Scripts.InGame.Upgrade;
using Economic;
using UnityEditor;
using UnityEngine;

namespace Dark.Tools.GoogleSheetTool
{
    public class ConfigNodeLevelUpImporter
    {
        public static void Import(ScriptableObject config, List<string[]> csvData)
        {
            if (config is not GradeConfig levelUpConfig)
            {
                Debug.LogError($"Invalid requirement config!");
                return;
            }
            
            // Header is field names
            var fields = csvData[0];

            var map = new Dictionary<int, int>();
            for (int i = 1; i < csvData.Count; i++) // Skip header
            {
                var cols = csvData[i];
                if (cols == null || cols.Length == 0) continue;

                if (int.TryParse(cols[0], out var resultId))
                {
                    if (map.ContainsKey(resultId))
                    {
                        Debug.LogError($"There are 2 line with the same id {resultId}, line {i}");
                        continue;
                    }

                    if (cols.Length > 1 && int.TryParse(cols[1], out var result))
                    {
                        map.Add(resultId, result);
                    }
                    else
                    {
                        Debug.LogError($"Invalid value at line {i}");
                    }
                }
                else
                {
                    Debug.LogError($"Invalid Id {cols[0]} at line {i}");
                    continue;
                }
            }
            
            levelUpConfig.gradeRequireMap = new List<int>();
            foreach (var value in map)
            {
                levelUpConfig.gradeRequireMap.Add(value.Value);
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(levelUpConfig);

            // Select the new asset
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = levelUpConfig;
#endif
        }
    }
}