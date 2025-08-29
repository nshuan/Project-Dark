#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Dark.Scripts.Utils
{
    public class CopySerializedField
    {
        public static void CopySerializedFieldValue(Object parent, string propertyPath)
        {
            if (parent == null) return;

            SerializedObject serializedObject = new SerializedObject(parent);
            SerializedProperty property = serializedObject.FindProperty(propertyPath);

            if (property == null)
            {
                Debug.LogWarning($"Property '{propertyPath}' not found on {parent.name}.");
                return;
            }

            EditorGUIUtility.systemCopyBuffer = GetSerializedPropertyValue(property);
            Debug.Log($"Copied '{propertyPath}': {EditorGUIUtility.systemCopyBuffer}");
        }

        public static void PasteSerializedFieldValue(Object parent, string propertyPath)
        {
            if (parent == null) return;

            SerializedObject serializedObject = new SerializedObject(parent);
            SerializedProperty property = serializedObject.FindProperty(propertyPath);

            if (property == null)
            {
                Debug.LogWarning($"Property '{propertyPath}' not found on {parent.name}.");
                return;
            }

            string clipboard = EditorGUIUtility.systemCopyBuffer;

            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    if (int.TryParse(clipboard, out int intValue))
                        property.intValue = intValue;
                    break;
                case SerializedPropertyType.Float:
                    if (float.TryParse(clipboard, out float floatValue))
                        property.floatValue = floatValue;
                    break;
                case SerializedPropertyType.Boolean:
                    if (bool.TryParse(clipboard, out bool boolValue))
                        property.boolValue = boolValue;
                    break;
                case SerializedPropertyType.String:
                    property.stringValue = clipboard;
                    break;
                case SerializedPropertyType.ObjectReference:
                    Object obj = AssetDatabase.LoadAssetAtPath<Object>(clipboard);
                    property.objectReferenceValue = obj;
                    break;
                default:
                    Debug.LogWarning($"Pasting not implemented for type: {property.propertyType}");
                    break;
            }

            serializedObject.ApplyModifiedProperties();
            Debug.Log($"Pasted into '{propertyPath}': {clipboard}");
        }

        private static string GetSerializedPropertyValue(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer: return property.intValue.ToString();
                case SerializedPropertyType.Boolean: return property.boolValue.ToString();
                case SerializedPropertyType.Float: return property.floatValue.ToString();
                case SerializedPropertyType.String: return property.stringValue;
                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue != null
                        ? AssetDatabase.GetAssetPath(property.objectReferenceValue)
                        : "null";
                default:
                    return property.ToString();
            }
        }
    }
}

#endif