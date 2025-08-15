using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Dark.Tools.GoogleSheetTool
{
    public class ConfigImporter
    {
        public static void Import(ScriptableObject[] configs, List<string[]> csvData)
        {
            var configDict = new Dictionary<int, ScriptableObject>();
            foreach (var config in configs)
            {
                // Validate configs name
                var underscoreIndex = config.name.IndexOf('_');
                if (underscoreIndex <= 0)
                {
                    Debug.LogError($"Invalid config name: {config.name}");
                    continue;
                }

                // Validate config index
                if (int.TryParse(config.name.Substring(0, underscoreIndex), out var configId))
                {
                    configDict.Add(configId, config);
                }
                else
                {
                    Debug.LogError($"Invalid config id: {config.name}");
                    return;
                }
                
                configDict.TryAdd(configId, config);
            }
            
            // Header is field names
            var fields = csvData[0];
            
            for (int i = 1; i < csvData.Count; i++) // Skip header
            {
                var cols = csvData[i];
                if (cols == null || cols.Length == 0) continue;
                
                if (!int.TryParse(cols[0], out var csvEnemyId))
                {
                    Debug.LogWarning($"Row {i+1} skipped — invalid enemyId");
                    continue;
                }

                if (configDict.TryGetValue(csvEnemyId, out ScriptableObject config))
                {
                    for (var fieldIndex = 0; fieldIndex < cols.Length; fieldIndex++)
                    {
                        SetValue(config, fields[fieldIndex], cols[fieldIndex]);
                    }

                    EditorUtility.SetDirty(config);
                }
                else
                {
                    Debug.LogError($"No matching config found for enemyId {csvEnemyId}");
                }
            }
        }

        public static void SetValue(ScriptableObject instance, string fieldName, string value)
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
                        parsedValue = Convert.ChangeType(value, fieldType, CultureInfo.InvariantCulture);
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