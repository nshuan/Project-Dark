using System;
using TMPro;
using UnityEngine;

namespace Dark.Tools.Language.Runtime
{
    public class LanguageTMPComponentReplaceKey : MonoBehaviour
    {
        public string key;
        public LanguageReplacePair[] replacePairs;

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
            if (string.IsNullOrEmpty(key)) return;
            var font = LanguageData.Instance.GetFontAssetRuntime(language);
            txt.font = font;

            var text = LanguageData.Instance.GetLocalizedString(key, language);
            if (replacePairs != null)
            {
                foreach (LanguageReplacePair pair in replacePairs)
                {
                    text = text.Replace(pair.key, pair.replaceTo);
                }
            }
            txt.SetText(text);
        }

        private void OnForceUpdate()
        {
            UpdateText(LanguageManager.Instance.CurrentLanguage);
        }
        
        [Serializable]
        public class LanguageReplacePair
        {
            public string key;
            public string replaceTo;
        }
    }
}