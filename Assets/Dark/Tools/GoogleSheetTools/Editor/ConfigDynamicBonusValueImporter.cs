using System;
using System.Collections.Generic;
using System.Globalization;
using Economic;
using InGame.Upgrade.DynamicBonus;
using InGame.Upgrade.DynamicCost;
using UnityEditor;
using UnityEngine;

namespace Dark.Tools.GoogleSheetTool
{
    public class ConfigDynamicBonusValueImporter
    {
        static NumberFormatInfo format = new NumberFormatInfo { NumberDecimalSeparator = "," };

        public static void Import(ScriptableObject config, List<string[]> csvData)
        {
            if (config is not DynamicBonusValueConfig bonusConfig)
            {
                Debug.LogError($"Invalid dynamic bonus value config!");
                return;
            }
            
            // Header is field names
            var fields = csvData[0];
            
            var bonusMap = new Dictionary<NodeBonusTypeV2, List<UpgradeDynamicBonusValueInfo>>();
            for (int i = 1; i < csvData.Count; i++) // Skip header
            {
                var cols = csvData[i];
                if (cols == null || cols.Length == 0) continue;

                if (Enum.TryParse<NodeBonusTypeV2>(cols[1], out var bonusType))
                {
                    if (bonusMap.ContainsKey(bonusType))
                    {
                        Debug.LogError($"There are 2 line with the same bonus type {bonusType}, line {i}");
                        continue;
                    }
                    
                    var allBonusLine = new List<UpgradeDynamicBonusValueInfo>();
                    if (cols.Length > 3)
                    {
                        var allBonus5LineStr = cols[2].Trim(' ').Split(";");
                        var allBonus1LineStr = cols[3].Trim(' ').Split(";");
                        
                        if (allBonus1LineStr.Length != allBonus5LineStr.Length)
                        {
                            Debug.LogError($"Invalid allBonus1LineStr at line {i}");
                            continue;
                        }
                        var parseValueSuccess = true;
                        for (var index = 0; index < allBonus5LineStr.Length; index++)
                        {
                            var bonus5 = new List<float>();
                            var bonus1 = 0f;

                            if (!float.TryParse(allBonus5LineStr[index], NumberStyles.Float, format, out var value5))
                            {
                                Debug.LogError($"Invalid parseValueSuccess type 1");
                                parseValueSuccess = false;
                                break;
                            }
                            else
                            {
                                for (var j = 0; j < 5; j++)
                                    bonus5.Add(value5);
                            }
                            
                            if (!float.TryParse(allBonus1LineStr[index], NumberStyles.Float, format, out var value1))
                            {
                                Debug.LogError($"Invalid parseValueSuccess type 2");
                                parseValueSuccess = false;
                                break;
                            }
                            else
                                bonus1 = value1;
                            
                            allBonusLine.Add(new UpgradeDynamicBonusValueInfo()
                            {
                                index = index,
                                bonus1Stage = bonus1,
                                bonus5Stages = bonus5.ToArray()
                            });
                        }

                        if (!parseValueSuccess)
                        {
                            Debug.LogError($"Invalid parseValueSuccess at line {i}");
                            continue;
                        }
                    }
                    else
                    {
                        Debug.LogError($"Invalid column length at line {i}");
                        continue;
                    }

                    bonusMap[bonusType] = allBonusLine;
                }
                else
                {
                    Debug.LogError($"Invalid Id {cols[0]} at line {i}");
                    continue;
                }
            }

            bonusConfig.bonusInfos = bonusMap;

#if UNITY_EDITOR
            EditorUtility.SetDirty(bonusConfig);

            // Select the new asset
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = bonusConfig;
#endif
        }
    }
}