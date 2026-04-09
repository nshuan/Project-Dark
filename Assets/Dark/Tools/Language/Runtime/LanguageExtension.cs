using TMPro;

namespace Dark.Tools.Language.Runtime
{
    public static class LanguageExtension
    {
        public static void SetTextLanguage(this TextMeshProUGUI txt, string key)
        {
            var currentLanguage = LanguageManager.Instance.CurrentLanguage;
            txt.font = LanguageData.Instance.GetFontAssetRuntime(currentLanguage);
            txt.SetText(LanguageData.Instance.GetLocalizedString(key, currentLanguage));
        }
        
        public static void SetTextLanguage(this TextMeshProUGUI txt, string key, params (string, string)[] replaces)
        {
            var currentLanguage = LanguageManager.Instance.CurrentLanguage;
            txt.font = LanguageData.Instance.GetFontAssetRuntime(currentLanguage);
            var text = LanguageData.Instance.GetLocalizedString(key, currentLanguage);
            foreach (var pair in replaces)
            {
                text = text.Replace(pair.Item1, pair.Item2);
            }
            txt.SetText(text);
        }
        
        public static void SetTextLanguageKeepFont(this TextMeshProUGUI txt, string key, params (string, string)[] replaces)
        {
            var currentLanguage = LanguageManager.Instance.CurrentLanguage;
            var text = LanguageData.Instance.GetLocalizedString(key, currentLanguage);
            foreach (var pair in replaces)
            {
                text = text.Replace(pair.Item1, pair.Item2);
            }
            txt.SetText(text);
        }

        public static void SetTextValueLanguage(this TextMeshProUGUI txt, string text)
        {
            var currentLanguage = LanguageManager.Instance.CurrentLanguage;
            txt.font = LanguageData.Instance.GetFontAssetRuntime(currentLanguage);
            txt.SetText(text);
        }
    }
}