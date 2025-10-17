using System.IO;
using Dark.Tools.Utils;
using InGame;
using UnityEditor;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade.UIUpgradeTreeCreator.Grid
{
    public class GridConfig : ScriptableObject
    {
        public bool enableGrid = true;
        public Color lineColor = Color.white;
        public float thickness = 2.0f;
        public Vector2 spacing = new Vector2(8f, 8f);
        
#if UNITY_EDITOR
        private static string FilePath = "Assets/Dark/Scripts/OutGame/Upgrade/UIUpgradeTreeCreator/Grid/UpgradeCreatorGridConfig.asset";
        
        public static void CreateInstance()
        {
            AssetDatabaseUtils.CreateSOInstance<GridConfig>(FilePath);
        }

        private static GridConfig instance;
        public static GridConfig Instance
        {
            get
            {
                if (instance) return instance;
                if (!File.Exists(FilePath))
                {
                    CreateInstance();
                }

                instance = AssetDatabase.LoadAssetAtPath<GridConfig>(FilePath);
                return instance;
            }
        }
#endif
    }
}