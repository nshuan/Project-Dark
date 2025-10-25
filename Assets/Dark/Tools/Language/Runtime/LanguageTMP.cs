using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;

namespace Dark.Tools.Language.Runtime
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LanguageTMP : SerializedMonoBehaviour
    {
        public string key;

        [ReadOnly, NonSerialized, OdinSerialize]
        private Dictionary<LanguageType, string> valueMap;

        [ReadOnly, NonSerialized, OdinSerialize]
        private Dictionary<LanguageType, TMP_FontAsset> fontMap;

        private TextMeshProUGUI txt;

        private void Awake()
        {
            Validate();
        }

        private void OnEnable()
        {
            txt = GetComponent<TextMeshProUGUI>();
            UpdateText();
        }

        public void UpdateText()
        {
            if (valueMap == null) return;
            if (fontMap == null) return;
            
            var currentLanguage = LanguageManager.Instance.CurrentLanguage;
            txt.font = fontMap[currentLanguage];
            txt.SetText(valueMap[currentLanguage]);
        }
        
        [Button]
        public void Validate()
        {
            var data = LanguageData.GetLanguageItem(key);
            if (data == null) return;
            
            valueMap = new Dictionary<LanguageType, string>();
            fontMap = new Dictionary<LanguageType, TMP_FontAsset>();
            foreach (var item in data.languageMap)
            {
                valueMap.Add(item.Key, item.Value);
                fontMap.Add(item.Key, LanguageData.GetFontAsset(item.Key));
            }
        }
    }
}