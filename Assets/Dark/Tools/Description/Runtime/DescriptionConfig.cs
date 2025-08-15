using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Dark.Tools.Description.Runtime
{
    public class DescriptionConfig : SerializedScriptableObject
    {
        public static string Path = "Assets/Dark/Tools/Description/Runtime/DescriptionConfig.asset";
            
        [NonSerialized, OdinSerialize] public Dictionary<string, DescriptionData> dataMap;
    }
    
    [Serializable]
    public class DescriptionData
    {
        public Dictionary<DescriptionType, string> descriptionMap;
    }
}
