using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using InGame;
using UnityEditor;
using UnityEngine;

namespace Dark.Tools.GoogleSheetTool
{
    public class ConfigLevelImporter
    {
        public static void Import(ScriptableObject[] configs, List<string[]> csvData)
        {
            var configDict = new Dictionary<int, LevelConfig>();
            foreach (var config in configs)
            {
                // Validate configs type
                if (config is not LevelConfig levelConfig)
                {
                    Debug.LogError($"Invalid level config: {config.name}");
                    continue;
                }

                configDict.TryAdd(levelConfig.level, levelConfig);
            }


            // Header is field names
            var fields = csvData[0];

            for (int i = 1; i < csvData.Count; i++) // Skip header
            {
                var cols = csvData[i];
                if (cols == null || cols.Length == 0) continue;
                
                // The first column is index
                if (!int.TryParse(cols[0], out var index))
                {
                    Debug.LogWarning($"Row {i+1} skipped — invalid index");
                    continue;
                }
                
                // The second column is level
                if (!int.TryParse(cols[1], out var level))
                {
                    Debug.LogWarning($"Row {i+1} skipped - invalid level");
                    continue;
                }
                
                // The third column is wave
                if (!int.TryParse(cols[2], out var wave))
                {
                    Debug.LogWarning($"Row {i+1} skipped - invalid wave");
                    continue;
                }
                
                // The fourth column is scale hp
                if (!float.TryParse(cols[3], NumberStyles.Float, new NumberFormatInfo { NumberDecimalSeparator = "," }, out var scaleHp))
                {
                    Debug.LogWarning($"Row {i+1} skipped - invalid hp scale");
                    continue;
                }
                
                // The fifth column is scale damage
                if (!float.TryParse(cols[4], NumberStyles.Float, new NumberFormatInfo { NumberDecimalSeparator = "," }, out var scaleDmg))
                {
                    Debug.LogWarning($"Row {i+1} skipped - invalid damage scale");
                    continue;
                }
                
                // The sixth column is scale speed
                if (!float.TryParse(cols[5], NumberStyles.Float, new NumberFormatInfo { NumberDecimalSeparator = "," }, out var scaleSpe))
                {
                    Debug.LogWarning($"Row {i+1} skipped - invalid speed scale");
                    continue;
                }

                if (configDict.TryGetValue(level, out LevelConfig config))
                {
                    for (var fieldIndex = 0; fieldIndex < cols.Length; fieldIndex++)
                    {
                        SetValue(config, wave, scaleHp, scaleDmg, scaleSpe);
                    }

                    EditorUtility.SetDirty(config);
                }
                else
                {
                    Debug.LogError($"No matching config found for level {level}");
                }
            }
        }
        
        public static void SetValue(LevelConfig level, int wave, float scaleHp, float scaleDamage, float scaleSpeed)
        {
            foreach (var waveInfo in level.waveInfo)
            {
                if (waveInfo.waveIndex == wave)
                {
                    waveInfo.scaleHp = scaleHp;
                    waveInfo.scaleDmg = scaleDamage;
                    waveInfo.scaleSpeed = scaleSpeed;
                    break;
                }
            }
        }
    }
}