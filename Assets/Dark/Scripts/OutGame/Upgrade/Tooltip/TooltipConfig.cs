using System;
using System.Collections.Generic;
using Dark.Tools.Utils;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace OutGame.Upgrade.Tooltip
{
    public class TooltipConfig : SerializedScriptableObject
    {
        public static string FilePath = "Assets/Dark/Resources/TooltipConfig.asset";

        public Dictionary<string, string> tooltipMap = new Dictionary<string, string>();

        public string GetTooltip(string key)
        {
            if (tooltipMap == null) return string.Empty;
            return tooltipMap.TryGetValue(key, out var tooltip) ? tooltip : string.Empty;
        }

        public string TryGetTooltip(string message)
        {
            if (tooltipMap == null) return string.Empty;
            
            ReadOnlySpan<char> msgSpan = message.ToLower().AsSpan();
            foreach (var pair in tooltipMap)
            {
                ReadOnlySpan<char> keySpan = pair.Key.ToLower().AsSpan();
                if (msgSpan.Contains(keySpan, StringComparison.CurrentCulture))
                    return pair.Value;
            }
            
            return string.Empty;
        }
        
        #region SINGLETON

        private static TooltipConfig instance;

        public static TooltipConfig Instance
        {
            get
            {
                if (instance == null)
                    instance = Resources.Load<TooltipConfig>("TooltipConfig");

                return instance;
            }
        }
        #endregion
        
#if UNITY_EDITOR
        [MenuItem("Dark/Tooltip/Tooltip Config")]
        public static void CreateInstance()
        {
            AssetDatabaseUtils.CreateSOInstance<TooltipConfig>(FilePath);
        }
#endif
    }

    [Serializable]
    public class TooltipInfo
    {
        
    }
}