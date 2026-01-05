using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using InGame.Upgrade.DynamicCost;
using UnityEditor;
using UnityEngine;

namespace Dark.Tools.GoogleSheetTool
{
    public class ConfigDynamicImporter
    {
        static NumberFormatInfo format = new NumberFormatInfo { NumberDecimalSeparator = "," };

        public static void Import(ScriptableObject config, List<string[]> csvData)
        {
            if (config is not DynamicVestigeConfig costConfig)
            {
                Debug.LogError($"Invalid dynamic vestige config!");
                return;
            }
            
            // Header is field names
            var fields = csvData[0];
            
            var costMap = new Dictionary<int, (int[], int, int[], int)>();
            for (int i = 1; i < csvData.Count; i++) // Skip header
            {
                var cols = csvData[i];
                if (cols == null || cols.Length == 0) continue;

                if (int.TryParse(cols[0], out var resultId))
                {
                    if (costMap.ContainsKey(resultId))
                    {
                        Debug.LogError($"There are 2 line with the same id {resultId}, line {i}");
                        continue;
                    }

                    var cost5 = new List<int>();
                    var cost1 = 0;
                    var cost5Echoes = new List<int>();
                    var cost1Echoes = 0;
                    
                    if (cols.Length > 1)
                    {
                        var costValueStr = cols[1].Trim(' ').Split(",");
                        var parseCostValueSuccess = true;
                        foreach (var str in costValueStr)
                        {
                            if (!int.TryParse(str, out var value))
                            {
                                parseCostValueSuccess = false;
                                break;
                            }
                            else
                            {
                                cost5.Add(value);
                            }
                        }

                        if (!parseCostValueSuccess)
                        {
                            Debug.LogError($"In valid vestige by stack at line {i}");
                            continue;
                        }
                    }
                    else
                    {
                        Debug.LogError($"In valid vestige by stack at line {i}");
                        continue;
                    }
                        
                    if (cols.Length > 2 && int.TryParse(cols[2], out var resultVestige))
                    {
                        cost1 = resultVestige;
                    }
                    else
                    {
                        Debug.LogError($"In valid vestige for all at line {i}");
                        continue;
                    }
                    
                    if (cols.Length > 3)
                    {
                        var costValueStr = cols[3].Trim(' ').Split(",");
                        var parseCostValueSuccess = true;
                        foreach (var str in costValueStr)
                        {
                            if (!int.TryParse(str, out var value))
                            {
                                parseCostValueSuccess = false;
                                break;
                            }
                            else
                            {
                                cost5Echoes.Add(value);
                            }
                        }

                        if (!parseCostValueSuccess)
                        {
                            Debug.LogError($"In valid echoes by stack at line {i}");
                            continue;
                        }
                    }
                    else
                    {
                        Debug.LogError($"In valid echoes by stack at line {i}");
                        continue;
                    }
                        
                    if (cols.Length > 4 && int.TryParse(cols[4], out var resultEchoes))
                    {
                        cost1Echoes = resultEchoes;
                    }
                    else
                    {
                        Debug.LogError($"In valid echoes for all at line {i}");
                        continue;
                    }
                    
                    costMap.Add(resultId, (cost5.ToArray(), cost1, cost5Echoes.ToArray(), cost1Echoes));
                }
                else
                {
                    Debug.LogError($"Invalid Id {cols[0]} at line {i}");
                    continue;
                }
            }
            
            costConfig.costInfos = new List<UpgradeDynamicVestigeInfo>();
            foreach (var cost in costMap)
            {
                costConfig.costInfos.Add(new UpgradeDynamicVestigeInfo()
                {
                    index = cost.Key,
                    cost5Stages = cost.Value.Item1,
                    cost1Stage = cost.Value.Item2,
                    cost5Echoes = cost.Value.Item3,
                    cost1Echoes = cost.Value.Item4,
                });
            }
            costConfig.SortByIndexAscending();

#if UNITY_EDITOR
            EditorUtility.SetDirty(costConfig);

            // Select the new asset
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = costConfig;
#endif
        }
    }
}