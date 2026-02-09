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
        static NumberFormatInfo format = new NumberFormatInfo { NumberDecimalSeparator = "," };
        
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
                if (!float.TryParse(cols[3], NumberStyles.Float, format, out var scaleHp))
                {
                    Debug.LogWarning($"Row {i+1} skipped - invalid hp scale");
                    continue;
                }
                
                // The fifth column is scale damage
                if (!float.TryParse(cols[4], NumberStyles.Float, format, out var scaleDmg))
                {
                    Debug.LogWarning($"Row {i+1} skipped - invalid damage scale");
                    continue;
                }
                
                // The sixth column is scale speed
                if (!float.TryParse(cols[5], NumberStyles.Float, format, out var scaleSpe))
                {
                    Debug.LogWarning($"Row {i+1} skipped - invalid speed scale");
                    continue;
                }
                
                // The seventh column is scale exp
                if (!float.TryParse(cols[6], NumberStyles.Float, format, out var scaleExp))
                {
                    Debug.LogWarning($"Row {i+1} skipped - invalid exp scale");
                    continue;
                }
                
                // The eighth column is scale vestige
                if (!float.TryParse(cols[7], NumberStyles.Float, format, out var scaleVestige))
                {
                    Debug.LogWarning($"Row {i+1} skipped - invalid vestige scale");
                    continue;
                }
                
                // The nineth column is vestige unit value
                if (!int.TryParse(cols[8], out var vestigeUnitValue))
                {
                    Debug.LogWarning($"Row {i+1} skipped - invalid vestige unit value");
                    continue;
                }

                if (configDict.TryGetValue(level, out LevelConfig config))
                {
                    for (var fieldIndex = 0; fieldIndex < cols.Length; fieldIndex++)
                    {
                        SetValue(config, wave, scaleHp, scaleDmg, scaleSpe, scaleExp, scaleVestige, vestigeUnitValue);
                    }

                    EditorUtility.SetDirty(config);
                }
                else
                {
                    Debug.LogError($"No matching config found for level {level}");
                }
            }
        }
        
        public static void SetValue(LevelConfig level, int wave, float scaleHp, float scaleDamage, float scaleSpeed, float scaleExp, float scaleVestige, int vestigeUnitValue)
        {
            foreach (var waveInfo in level.waveInfo)
            {
                // Do wave trên sheet đánh số từ 1
                if (waveInfo.waveIndex + 1 == wave)
                {
                    waveInfo.scaleHp = scaleHp;
                    waveInfo.scaleDmg = scaleDamage;
                    waveInfo.scaleSpeed = scaleSpeed;
                    waveInfo.expRatio = scaleExp;
                    waveInfo.darkRatio = scaleVestige;
                    waveInfo.darkUnitValue = vestigeUnitValue;
                    break;
                }
            }
        }
    }
}