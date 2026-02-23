using System.Collections.Generic;
using Dark.Tools.Utils;
using InGame;
using UnityEditor;
using UnityEngine;

namespace Dark.Scripts.ForDemo
{
    public class DemoConfig : ScriptableObject
    {
        public const bool IsDemo = false;
        
        private static string Path = "DemoConfig";
        private static string FilePath = "Assets/Dark/Scripts/ForDemo/Resources/DemoConfig.asset";
        
        [SerializeField] private string steamWishlistURL = "https://store.steampowered.com/app/3913310/Ash_Warden/";
        [SerializeField] private string feedbackURL = "https://forms.gle/C4zx8ouoPUoRji1v6";
        [SerializeField] private int maxDemoLevel = 3; // Tester can only play 3 levels in demo version
        [SerializeField] private List<int> lockedNodes;
        [SerializeField] private int collectLogicType = 0; // 0 = original, 1 = auto collect, 2 = mouse
        [SerializeField] private bool showTooltip = true;
        [SerializeField] private Color nodeActivatedColor;
        
        public static string SteamWishlistURL { get; private set; }
        public static string FeedbackURL { get; private set; }
        public static int MaxDemoLevel { get; private set; }
        public static int CollectLogicType { get; private set; }
        public static bool ShowTooltip { get; private set; }

        public void InitPublicProperties()
        {
            SteamWishlistURL = steamWishlistURL;
            FeedbackURL = feedbackURL;
            MaxDemoLevel = IsDemo ? maxDemoLevel : 100;
            CollectLogicType = collectLogicType;
            ShowTooltip = showTooltip;
        }
        
        #region SINGLETON

        private static DemoConfig instance;

        public static DemoConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<DemoConfig>("DemoConfig");
                }

                return instance;
            }
        }
        #endregion
        
#if UNITY_EDITOR
        [MenuItem("Dark/Demo/Generate Demo Config")]
        public static void CreateInstance()
        {
            AssetDatabaseUtils.CreateSOInstance<DemoConfig>(FilePath);
        }
#endif
    }
}