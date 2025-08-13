using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FolderOnlyAttribute))]
public class FolderOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw object field
        var picked = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(DefaultAsset), false);

        // Validate selection: allow null or folders only
        if (picked != null)
        {
            string path = AssetDatabase.GetAssetPath(picked);
            if (!AssetDatabase.IsValidFolder(path))
            {
                Debug.LogWarning($"'{picked.name}' is not a folder. Please select a folder.");
                picked = property.objectReferenceValue; // revert to previous valid value
            }
        }

        property.objectReferenceValue = picked;
        EditorGUI.EndProperty();
    }
}