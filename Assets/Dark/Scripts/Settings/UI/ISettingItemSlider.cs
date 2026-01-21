using System;
using Dark.Scripts.AudioV2;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.Settings.UI
{
    public interface ISettingItemSlider : ISettingItemLogic<Slider>
    {
        Slider Slider { get; set; }
        TextMeshProUGUI DisplayValue { get; set; }

        new void Initialize(Slider slider)
        {
            Slider = slider;
            slider.onValueChanged.RemoveAllListeners();
            slider.onValueChanged.AddListener(OnValueChanged);
        }

        void OnValueChanged(float value);
    }

    [Serializable]
    public class SettingAudioVolume : ISettingItemSlider
    {
        [SerializeField] private AudioChannel settingType;
        [SerializeField, Range(0, 10)] private int minValue;
        [SerializeField, Range(0, 10)] private int maxValue;

        public Slider Slider { get; set; }
        public TextMeshProUGUI DisplayValue { get; set; }

        public void Initialize(Slider slider)
        {
            Slider = slider;
            Slider.minValue = minValue;
            Slider.maxValue = maxValue;
            slider.onValueChanged.RemoveAllListeners();
            InitSlider();
            slider.onValueChanged.AddListener(OnValueChanged);
            
            UpdateValue(false);
        }

        public void Save()
        {
            
        }

        private void InitSlider()
        {
            var volume = settingType switch
            {
                AudioChannel.Ui => GameSettings.VolumeUI,
                AudioChannel.Music => GameSettings.VolumeMusic,
                AudioChannel.InGame => GameSettings.VolumeInGame,
                AudioChannel.OutGame => GameSettings.VolumeOutGame,
                _ => 0f
            };

            Slider.value = Mathf.RoundToInt(Slider.minValue + (Slider.maxValue - Slider.minValue) * volume);
        }

        public void OnValueChanged(float value)
        {
            switch (settingType)
            {
                case AudioChannel.Ui:
                    GameSettings.VolumeUI = (Slider.value - minValue) / (maxValue - minValue);
                    GameSettings.EnableUISound = Slider.value > 0;
                    break;
                case AudioChannel.Music:
                    GameSettings.VolumeMusic = (Slider.value - minValue) / (maxValue - minValue);
                    // GameSettings.EnableMusic = Slider.value > 0;
                    break;
                case AudioChannel.InGame:
                    GameSettings.VolumeInGame = (Slider.value - minValue) / (maxValue - minValue);
                    GameSettings.EnableInGameSound = Slider.value > 0;
                    break;
                case AudioChannel.OutGame:
                    GameSettings.VolumeOutGame = (Slider.value - minValue) / (maxValue - minValue);
                    GameSettings.EnableOutGameSound = Slider.value > 0;
                    break;
            }
            
            UpdateValue(false);
            GameSettings.Save();
        }
        
        public void UpdateValue(bool onEnable)
        {
            DisplayValue.SetText(Mathf.RoundToInt(Slider.value).ToString());
        }
    }
}