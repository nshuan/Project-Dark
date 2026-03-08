using System;
using Dark.Scripts.Settings.UI;
using Dark.Tools.Language;
using Dark.Tools.Language.Runtime;
using TMPro;
using UnityEngine.UI;

namespace Dark.Scripts.Settings.Language
{
    [Serializable]
    public class SettingLanguage : ISettingItemButtonLeftRight
    {
        private LanguageType language;
        
        public void Initialize(Button button)
        {
            ButtonLeft = button;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClickLeft);

            language = LanguageManager.Instance.CurrentLanguage;
            UpdateValue(true);
        }

        public void InitializeButtonRight(Button button)
        {
            ButtonRight = button;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClickRight);
        }

        public void OnClickLeft()
        {
            var index = (int)language;
            index -= 1;
            if (index < 0) index = Enum.GetValues(typeof(LanguageType)).Length - 1;
            language = (LanguageType)index;
            UpdateValue(false);
        }
        
        public void OnClickRight()
        {
            var index = (int)language;
            index += 1;
            if (index >= Enum.GetValues(typeof(LanguageType)).Length) index = 0;
            language = (LanguageType)index;
            UpdateValue(false);
        }

        public void Save()
        {
            LanguageManager.Instance.UpdateDefaultLanguage(language);
        }

        public void UpdateValue(bool onEnable)
        {
            DisplayText.SetText(language.ToString());
        }

        public Button ButtonLeft { get; set; }
        public Button ButtonRight { get; set; }
        public TextMeshProUGUI DisplayText { get; set; }
    }
}