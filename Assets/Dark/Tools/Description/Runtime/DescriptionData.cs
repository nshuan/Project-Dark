using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Dark.Tools.Description.Runtime
{
    public class DescriptionData : SerializedScriptableObject
    {
        public static string Path = "Assets/Dark/Tools/Description/Runtime/DescriptionData.asset";
            
        [NonSerialized, OdinSerialize] public Dictionary<string, DescriptionItem> dataMap;
    }
    
    [Serializable]
    public class DescriptionItem
    {
        public Dictionary<DescriptionType, string> descriptionMap;
    }
}
