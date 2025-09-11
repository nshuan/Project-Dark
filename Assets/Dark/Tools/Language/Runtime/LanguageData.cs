using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Dark.Tools.Language;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Dark.Tools.Language.Runtime
{
    public class LanguageData : SerializedScriptableObject
    {
        public static string Path = "Assets/Dark/Tools/Language/Runtime/LanguageData.asset";
            
        [NonSerialized, OdinSerialize] public Dictionary<string, LanguageItem> dataMap;
        [NonSerialized, OdinSerialize] public Dictionary<LanguageType, TMP_FontAsset> fontMap;
        
#if UNITY_EDITOR
        public static LanguageItem GetLanguageItem(string key)
        {
            if (!File.Exists(Path))
            {
                Debug.LogError("LanguageData asset is missing!!!");
                return null;
            }

            var instance = AssetDatabase.LoadAssetAtPath<LanguageData>(Path);
            if (instance.dataMap.TryGetValue(key, out var result))
                return result;
            
            Debug.LogError($"LanguageData asset doesn't have any data for key [{key}]");
            return null;
        }

        public static TMP_FontAsset GetFontAsset(LanguageType languageType)
        {
            if (!File.Exists(Path))
            {
                Debug.LogError("LanguageData asset is missing!!!");
                return null;
            }

            var instance = AssetDatabase.LoadAssetAtPath<LanguageData>(Path);
            if (instance.fontMap.TryGetValue(languageType, out var result))
                return result;
            
            Debug.LogError($"LanguageData asset doesn't have any font for type [{languageType}]");
            return null;
        }
#endif
    }
    
    [Serializable]
    public class LanguageItem
    {
        public Dictionary<LanguageType, string> languageMap;
    }
}
