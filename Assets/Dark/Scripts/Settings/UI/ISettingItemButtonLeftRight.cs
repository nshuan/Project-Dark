using System;
using Dark.Scripts.Settings.Resolution;
using TMPro;
using UnityEngine.UI;

namespace Dark.Scripts.Settings.UI
{
    public interface ISettingItemButtonLeftRight : ISettingItemLogic<Button>
    {
        Button ButtonLeft { get; set; }
        Button ButtonRight { get; set; }
        TextMeshProUGUI DisplayText { get; set; }
        new void Initialize(Button button)
        {
            ButtonLeft = button;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClickLeft);
        }

        public void InitializeButtonRight(Button button)
        {
            ButtonRight = button;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClickRight);
        }

        void OnClickLeft();
        void OnClickRight();
    }
    
    [Serializable]
    public class SettingResolution : ISettingItemButtonLeftRight
    {
        public Button ButtonLeft { get; set; }
        public Button ButtonRight { get; set; }
        public TextMeshProUGUI DisplayText { get; set; }

        private int width;
        private int height;
        private int selectedIndex;
        
        public void Initialize(Button button)
        {
            ButtonLeft = button;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClickLeft);
            
            selectedIndex = ResolutionSettings.GetSelectedResolutionIndex();
            
            UpdateValue(true);
        }

        public void InitializeButtonRight(Button button)
        {
            ButtonRight = button;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClickRight);
        }

        public void Save()
        {
            GameSettings.ResolutionWidth = width;
            GameSettings.ResolutionHeight = height;
            GameSettings.Save();
            ResolutionSettings.SetResolutionByIndex(selectedIndex, apply: true);
        }

        public void OnClickLeft()
        {
            if (selectedIndex == -1) return;

            selectedIndex -= 1;
            if (selectedIndex < 0)
                selectedIndex = ResolutionSettings.SupportedResolutions.Count - 1;
            
            var selectedEntry = ResolutionSettings.SupportedResolutions[selectedIndex];
            width = selectedEntry.width;
            height = selectedEntry.height;
            UpdateValue(false);
        }

        public void OnClickRight()
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
}