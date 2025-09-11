using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Dark.Tools.Language.Runtime
{
    public class LanguageLocalization : SerializedMonoBehaviour
    {
        public string key;

        [ReadOnly, NonSerialized, OdinSerialize]
        private Dictionary<LanguageType, string> valueMap;

#if UNITY_EDITOR
        [Button]
        private void Validate()
        {
            var data = LanguageData.GetLanguageItem(key);
            if (data == null) return;
            
            valueMap = new Dictionary<LanguageType, string>();
            foreach (var item in data.languageMap)
            {
                valueMap.Add(item.Key, item.Value);
            }
        }
#endif
    }
}