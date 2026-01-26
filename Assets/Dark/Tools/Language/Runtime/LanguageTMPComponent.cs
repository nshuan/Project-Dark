using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;

namespace Dark.Tools.Language.Runtime
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LanguageTMPComponent : SerializedMonoBehaviour
    {
        public string key;

        [ReadOnly, NonSerialized, OdinSerialize]
        private Dictionary<LanguageType, string> valueMap;

        [ReadOnly, NonSerialized, OdinSerialize]
        private Dictionary<LanguageType, TMP_FontAsset> fontMap;

        private TextMeshProUGUI txt;

        private void OnEnable()
        {
            txt = GetComponent<TextMeshProUGUI>();
            UpdateText();
        }

        public void UpdateText()
        {
            var currentLanguage = LanguageManager.Instance.CurrentLanguage;
            UpdateText(currentLanguage);
        }

        public void UpdateText(LanguageType language)
        {
            if (fontMap != null)
            {
                txt.font = fontMap[language];
            }

            if (valueMap != null)
            {
                if (!valueMap.ContainsKey(language)) txt.SetText(valueMap[LanguageType.english]);
                else txt.SetText(valueMap[language]);
            }
        }
        
#if UNITY_EDITOR
        [Button]
        public void Validate()
        {
            fontMap = new Dictionary<LanguageType, TMP_FontAsset>();
            foreach (LanguageType language in Enum.GetValues(typeof(LanguageType)))
            {
                fontMap.Add(language, LanguageData.GetFontAsset(language));
            }
            
            var data = LanguageData.GetLanguageItem(key);
            if (data == null) return;
            
            valueMap = new Dictionary<LanguageType, string>();
            foreach (var item in data.languageMap)
            {
                valueMap.Add(item.Key, item.Value);
            }
        }

        [Button]
        public void SetLanguage(LanguageType language)
        {
            txt = GetComponent<TextMeshProUGUI>();
            UpdateText(language);
        }
#endif
    }
}