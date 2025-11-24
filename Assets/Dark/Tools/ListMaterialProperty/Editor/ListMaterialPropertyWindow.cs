using System.Text;
using Dark.Tools.AutoSpritePivot.Editor;
using UnityEditor;
using UnityEngine;

namespace Dark.Tools.ListMaterialProperty.Editor
{
    public class ListMaterialPropertyWindow : EditorWindow
    {
        private Material targetMaterial;
        private string propertyText = "";
        private Vector2 scrollPos;

        [MenuItem("Dark/Material Property Viewer")]
        public static void ShowWindow()
        {
            GetWindow<ListMaterialPropertyWindow>("Material Property Viewer");
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();

            targetMaterial = (Material)EditorGUILayout.ObjectField("Material", targetMaterial, typeof(Material), false);

            if (GUILayout.Button("Show Properties") && targetMaterial != null)
            {
                propertyText = GetMaterialProperties(targetMaterial);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Shader Properties", EditorStyles.boldLabel);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.TextArea(propertyText, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        private string GetMaterialProperties(Material mat)
        {
            var sb = new StringBuilder();
            Shader shader = mat.shader;
            int count = shader.GetPropertyCount();

            sb.AppendLine($"Shader: {shader.name}");
            sb.AppendLine($"Properties ({count}):");
            sb.AppendLine();

            for (int i = 0; i < count; i++)
            {
                string name = shader.GetPropertyName(i);
                var type = shader.GetPropertyType(i);
                sb.AppendLine($"{i}. {name} ({type})");
            }

            return sb.ToString();
        }
    }
}