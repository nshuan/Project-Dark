using System;
using System.Collections.Generic;
using Dark.Scripts.Audio;
using Dark.Scripts.AudioV2;
using Dark.Scripts.Settings.Resolution;
using Dark.Tools.Language.Runtime;
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

        public void Save()
        {
            
        }

        public void UpdateValue(bool onEnable)
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
            
            UpdateValue(true);
        }

        public void Save()
        {
            
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
            
            UpdateValue(false);
            GameSettings.Save();
        }

        public void UpdateValue(bool onEnable)
        {
            switch (settingType)
            {
                case AudioChannel.Ui:
                    if (DisplayText) DisplayText.SetText(GameSettings.EnableUISound ? "On" : "Off");
                    break;
                case AudioChannel.Music:
                    if (DisplayText) DisplayText.SetText(GameSettings.EnableMusic ? "On" : "Off");
                    break;
                case AudioChannel.InGame:
                    if (DisplayText) DisplayText.SetText(GameSettings.EnableInGameSound ? "On" : "Off");
                    break;
                case AudioChannel.OutGame:
                    if (DisplayText) DisplayText.SetText(GameSettings.EnableOutGameSound ? "On" : "Off");
                    break;
            }
        }
    }

    [Serializable]
    public class SettingFullScreen : ISettingItemButton
    {
        public Button Button { get; set; }
        public TextMeshProUGUI DisplayText { get; set; }

        private bool isWindowedMode;
        
        public void Initialize(Button button)
        {
            Button = button;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
            
            UpdateValue(true);
        }

        public void Save()
        {
            GameSettings.WindowedMode = isWindowedMode;
            GameSettings.Save();
            ResolutionSettings.SetFullscreen(!isWindowedMode, apply: true);
        }

        public void OnClick()
        {
            isWindowedMode = !isWindowedMode;
            UpdateValue(false);
        }

        public void UpdateValue(bool onEnable)
        {
            if (onEnable) isWindowedMode = GameSettings.WindowedMode;
            DisplayText.SetTextLanguage(isWindowedMode ? "key_off" : "key_on");
        }
    }

    [Serializable]
    public class SettingResolution : ISettingItemButton
    {
        public Button Button { get; set; }
        public TextMeshProUGUI DisplayText { get; set; }

        private int width;
        private int height;
        private int selectedIndex;
        
        public void Initialize(Button button)
        {
            Button = button;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
            
            selectedIndex = ResolutionSettings.GetSelectedResolutionIndex();
            
            UpdateValue(true);
        }
        
        public void Save()
        {
            GameSettings.ResolutionWidth = width;
            GameSettings.ResolutionHeight = height;
            GameSettings.Save();
            ResolutionSettings.SetResolutionByIndex(selectedIndex, apply: true);
        }

        public void OnClick()
        {
            if (selectedIndex == -1) return;

            selectedIndex += 1;
            if (selectedIndex >= ResolutionSettings.SupportedResolutions.Count)
                selectedIndex = 0;
            
            var selectedEntry = ResolutionSettings.SupportedResolutions[selectedIndex];
            width = selectedEntry.width;
            height = selectedEntry.height;
            UpdateValue(false);
        }
        
        public void UpdateValue(bool onEnable)
        {
            if (onEnable)
            {
                width = GameSettings.ResolutionWidth;
                height = GameSettings.ResolutionHeight;
            }
            DisplayText.SetText($"{width}x{height}");
        }
    }
    
    [Serializable]
    public class SettingVSync : ISettingItemButton
    {
        public Button Button { get; set; }
        public TextMeshProUGUI DisplayText { get; set; }

        private bool vSync;

        public void Initialize(Button button)
        {
            Button = button;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
        
        public void OnClick()
        {
            vSync = !vSync;
            UpdateValue(false);
        }

        public void Save()
        {
            
        }

        public void UpdateValue(bool onEnable)
        {
            if (onEnable)
            {
                vSync = GameSettings.EnableVSync;
            }

            DisplayText.SetText(vSync ? "On" : "Off");
        }
    }
    
    [Serializable]
    public class SettingFrameRateCap : ISettingItemButton
    {
        public void Save()
        {
            
        }

        public void UpdateValue(bool onEnable)
        {
            
        }

        public Button Button { get; set; }
        public TextMeshProUGUI DisplayText { get; set; }
        public void OnClick()
        {
            
        }
    }
}