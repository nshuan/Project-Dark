using System;
using Dark.Scripts.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.Settings.UI
{
    public interface ISettingItemButton : ISettingItemLogic<Button>
    {
        Button Button { get; set; }
        TextMeshProUGUI DisplayText { get; set; }
        new void Initialize(Button button)
        {
            Button = button;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }

        void OnClick();
    }
    
    [Serializable]
    public class SettingPauseKey : ISettingItemButton
    {
        public Button Button { get; set; }
        public TextMeshProUGUI DisplayText { get; set; }

        public void OnClick()
        {
            
        }
    }
    
    [Serializable]
    public class SettingActiveAudio : ISettingItemButton
    {
        [SerializeField] private AudioPlayType settingType;

        public Button Button { get; set; }
        public TextMeshProUGUI DisplayText { get; set; }

        public void Initialize(Button button)
        {
            Button = button;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
            
            UpdateText();
        }

        public void OnClick()
        {
            switch (settingType)
            {
                case AudioPlayType.Sound:
                    GameSettings.EnableSound = !GameSettings.EnableSound;
                    break;
                case AudioPlayType.Music:
                    GameSettings.EnableMusic = !GameSettings.EnableMusic;
                    break;
            }
            
            UpdateText();
            GameSettings.Save();
        }

        private void UpdateText()
        {
            switch (settingType)
            {
                case AudioPlayType.Sound:
                    if (DisplayText) DisplayText.SetText(GameSettings.EnableSound ? "on" : "off");
                    break;
                case AudioPlayType.Music:
                    if (DisplayText) DisplayText.SetText(GameSettings.EnableMusic ? "on" : "off");
                    break;
            }
        }
    }
}