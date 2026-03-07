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
        public static string Path = "Assets/Dark/Tools/Language/Runtime/Resources/LanguageData.asset";
            
        [NonSerialized, OdinSerialize] public Dictionary<string, LanguageItem> dataMap;
        [NonSerialized, OdinSerialize] public Dictionary<LanguageType, TMP_FontAsset> fontMap;

        public string GetLocalizedString(string key, LanguageType language)
        {
            if (dataMap == null || !dataMap.TryGetValue(key, out var data)) return string.Empty;
            if (data.languageMap.TryGetValue(language, out var term) && !string.IsNullOrEmpty(term)) return term;
            return data.languageMap[LanguageType.english];
        }
        
        public TMP_FontAsset GetFontAssetRuntime(LanguageType languageType)
        {
            if (fontMap.TryGetValue(languageType, out var result))
                return result;

            return fontMap[LanguageType.english];
        }
        
        #region Singleton

        private static LanguageData instance;

        public static LanguageData Instance
        {
            get
            {
                if (!instance) instance = Resources.Load<LanguageData>("LanguageData");
                return instance;
            }
        }

        #endregion
        
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
