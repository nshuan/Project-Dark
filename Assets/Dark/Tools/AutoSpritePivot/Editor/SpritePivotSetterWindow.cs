namespace Dark.Tools.AutoSpritePivot.Editor
{
    using UnityEngine;
    using UnityEditor;

    public class SpritePivotSetterWindow : EditorWindow
    {
        private Texture2D texture;
        private Vector2 pivot = new Vector2(0.5f, 0.5f); // Default: center pivot

        [MenuItem("Dark/Sprite Pivot Setter")]
        public static void ShowWindow()
        {
            GetWindow<SpritePivotSetterWindow>("Sprite Pivot Setter");
        }

        void OnGUI()
        {
            GUILayout.Label("Set Pivot for Sprites", EditorStyles.boldLabel);

            texture = (Texture2D)EditorGUILayout.ObjectField("Sprite Texture", texture, typeof(Texture2D), false);
            pivot = EditorGUILayout.Vector2Field("Pivot (0–1)", pivot);

            if (GUILayout.Button("Apply Pivot") && texture != null)
            {
                SetPivotForAllSprites(texture, pivot);
            }
        }

        private void SetPivotForAllSprites(Texture2D tex, Vector2 newPivot)
        {
            string path = AssetDatabase.GetAssetPath(tex);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer == null) return;

            if (importer.spriteImportMode == SpriteImportMode.Single)
            {
                importer.spritePivot = newPivot;
            }
            else if (importer.spriteImportMode == SpriteImportMode.Multiple)
            {
                SpriteMetaData[] sheet = importer.spritesheet;

                for (int i = 0; i < sheet.Length; i++)
                {
                    sheet[i].alignment = (int)SpriteAlignment.Custom;
                    sheet[i].pivot = newPivot;
                }

                importer.spritesheet = sheet;
            }

            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();

            Debug.Log($"Pivot set to {newPivot} for {tex.name}");
        }
    }

}