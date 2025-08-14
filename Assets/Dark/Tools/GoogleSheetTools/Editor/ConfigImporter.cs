using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;

namespace GoogleSheetTool
{
    public abstract class ConfigImporter
    {
        public abstract void Import(ScriptableObject[] configs, List<string[]> csvData);

        public void SetValue(ScriptableObject instance, string fieldName, string value)
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