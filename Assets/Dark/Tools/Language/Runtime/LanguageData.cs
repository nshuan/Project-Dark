using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Dark.Tools.Language.Runtime
{
    public class LanguageData : SerializedScriptableObject
    {
        public static string Path = "Assets/Dark/Tools/Language/Runtime/LanguageData.asset";
            
        [NonSerialized, OdinSerialize] public Dictionary<string, LanguageItem> dataMap;
    }
    
    [Serializable]
    public class LanguageItem
    {
        public Dictionary<LanguageType, string> descriptionMap;
    }
}
