using System;
using Dark.Scripts.Settings.UI;
using Dark.Tools.Language;
using Dark.Tools.Language.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Dark.Scripts.Settings.Language
{
    [Serializable]
    public class SettingLanguage : ISettingItemButton
    {
        private LanguageType language;
        
        public void Initialize(Button button)
        {
            Button = button;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);

            language = LanguageManager.Instance.CurrentLanguage;
            UpdateValue(true);
        }
        
        public void Save()
        {
            LanguageManager.Instance.UpdateDefaultLanguage(language);
        }

        public void UpdateValue(bool onEnable)
        {
            DisplayText.SetText(language.ToString());
        }

        public Button Button { get; set; }
        public TextMeshProUGUI DisplayText { get; set; }
        public void OnClick()
        {
            var index = (int)language;
            index += 1;
            if (index >= Enum.GetValues(typeof(LanguageType)).Length) index = 0;
            language = (LanguageType)index;
            UpdateValue(false);
        }
    }
}