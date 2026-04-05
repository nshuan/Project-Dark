using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using InGame;
using InGame.EndlessLevel;
using UnityEditor;
using UnityEngine;

namespace Dark.Tools.GoogleSheetTool
{
    public class ConfigEndlessLevelImporter
    {
        static NumberFormatInfo format = new NumberFormatInfo { NumberDecimalSeparator = "," };

        public static void Import(ScriptableObject config, List<string[]> csvData)
        {
            if (!config) return;
            if (config is not LevelEndlessConfig levelConfig) return;
            
            var waveInfos = new List<WaveEndlessInfo>();
            
            // Header is field names
            var fields = csvData[0];
            
            for (int i = 1; i < csvData.Count; i++) // Skip header
            {
                var cols = csvData[i];
                if (cols == null || cols.Length == 0) continue;
                
                if (!int.TryParse(cols[0], out var csvWaveId))
                {
                    Debug.LogWarning($"Row {i+1} skipped — invalid waveId");
                    continue;
                }

                var newWaveInfo = new WaveEndlessInfo();
                for (var fieldIndex = 0; fieldIndex < cols.Length; fieldIndex++)
                {
                    SetValue(ref newWaveInfo, fields[fieldIndex], cols[fieldIndex]);
                }
                    
                waveInfos.Add(newWaveInfo);

            }
            
            levelConfig.waveInfo = waveInfos.ToArray();
            EditorUtility.SetDirty(config);
        }

        public static void SetValue(ref WaveEndlessInfo instance, string fieldName, string value)
        {
            Type type = instance.GetType();

            // Find the field
            FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null)
            {
                try
                {
                    // Get the field type
                    Type fieldType = field.FieldType;

                    // Parse targetValue to the field type
                    object parsedValue;
                    if (fieldType.IsEnum)
                    {
                        parsedValue = Enum.Parse(fieldType, value, ignoreCase: true);
                    }
                    else
                    {
                        if (fieldType == typeof(float))
                            parsedValue = Convert.ChangeType(value, fieldType, format);
                        else
                            parsedValue = Convert.ChangeType(value, fieldType);
                    }

                    // Set the field value
                    field.SetValue(instance, parsedValue);

                    Debug.Log($"Field '{fieldName}' set to {field.GetValue(instance)} (type: {fieldType.Name})");
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Could not convert value to {field.FieldType.Name}: {ex.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"No field found with name '{fieldName}'");
            }
        }
    }
}