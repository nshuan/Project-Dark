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

        public (string, string) TryGetTooltip(string message)
        {
            if (tooltipMap == null) return (string.Empty, string.Empty);
            
            ReadOnlySpan<char> msgSpan = message.ToLower().AsSpan();
            var msgLength = msgSpan.Length;
            foreach (var pair in tooltipMap)
            {
                ReadOnlySpan<char> keySpan = pair.Key.ToLower().AsSpan();
                if (msgLength < keySpan.Length) continue;
                if (msgLength == keySpan.Length)
                {
                    if (msgSpan.SequenceEqual(keySpan)) return (pair.Key, pair.Value);
                }
                
                if (msgSpan.Slice(0, keySpan.Length).SequenceEqual(keySpan) && msgSpan[keySpan.Length] == ' ')
                {
                    return (pair.Key, pair.Value);
                }
                
                if (msgSpan.Slice(msgLength - keySpan.Length, keySpan.Length).SequenceEqual(keySpan) && msgSpan[msgLength - keySpan.Length - 1] == ' ')
                {
                    return (pair.Key, pair.Value);
                }
                    
                ReadOnlySpan<char> keySpanWithSpace = (' ' + pair.Key.ToLower() + ' ').AsSpan();
                if (msgSpan.Contains(keySpanWithSpace, StringComparison.CurrentCulture))
                    return (pair.Key, pair.Value);
            }
            
            return (string.Empty, string.Empty);
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