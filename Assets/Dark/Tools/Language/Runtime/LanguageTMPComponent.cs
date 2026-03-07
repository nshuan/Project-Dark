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

        [OdinSerialize, NonSerialized] private Dictionary<LanguageType, TMP_FontAsset> overrideFont;

        // [ReadOnly, NonSerialized, OdinSerialize]
        // private Dictionary<LanguageType, TMP_FontAsset> fontMap;

        private TextMeshProUGUI txt;

        private void Start()
        {
            LanguageManager.Instance.RegisterForceUpdate(OnForceUpdate);
        }

        private void OnDestroy()
        {
            LanguageManager.Instance.UnregisterForceUpdate(OnForceUpdate);
        }

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
            if (overrideFont != null && overrideFont.TryGetValue(language, out var font))
            {
                txt.font = font;
            }
            else
            {
                font = LanguageData.Instance.GetFontAssetRuntime(language);
                txt.font = font;
            }
            
            if (string.IsNullOrEmpty(key)) return;

            txt.SetText(LanguageData.Instance.GetLocalizedString(key, language));
        }

        private void OnForceUpdate()
        {
            UpdateText(LanguageManager.Instance.CurrentLanguage);
        }
        
#if UNITY_EDITOR
        [Button]
        public void Validate()
        {
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