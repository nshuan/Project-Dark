using System;
using Dark.Scripts.Audio;
using Dark.Scripts.AudioV2;
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
        [SerializeField] private AudioChannel settingType;

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
                case AudioChannel.Ui:
                    GameSettings.EnableUISound = !GameSettings.EnableUISound;
                    break;
                case AudioChannel.Music:
                    GameSettings.EnableMusic = !GameSettings.EnableMusic;
                    break;
                case AudioChannel.InGame:
                    GameSettings.EnableInGameSound = !GameSettings.EnableInGameSound;
                    break;
                case AudioChannel.OutGame:
                    GameSettings.EnableOutGameSound = !GameSettings.EnableOutGameSound;
                    break;
            }
            
            UpdateText();
            GameSettings.Save();
        }

        private void UpdateText()
        {
            switch (settingType)
            {
                case AudioChannel.Ui:
                    if (DisplayText) DisplayText.SetText(GameSettings.EnableUISound ? "on" : "off");
                    break;
                case AudioChannel.Music:
                    if (DisplayText) DisplayText.SetText(GameSettings.EnableMusic ? "on" : "off");
                    break;
                case AudioChannel.InGame:
                    if (DisplayText) DisplayText.SetText(GameSettings.EnableInGameSound ? "on" : "off");
                    break;
                case AudioChannel.OutGame:
                    if (DisplayText) DisplayText.SetText(GameSettings.EnableOutGameSound ? "on" : "off");
                    break;
            }
        }
    }
}