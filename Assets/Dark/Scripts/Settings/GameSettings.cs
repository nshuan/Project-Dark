using System;
using Core;
using Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dark.Scripts.Settings
{
    public class GameSettings
    {
        private const string KeySettings = "game_settings";
        
        [Header("Control settings")] 
        public static KeyCode KeyPause;

        [Header("Audio settings")] 
        public static bool EnableUISound;
        public static bool EnableInGameSound;
        public static bool EnableOutGameSound;
        public static bool EnableMusic;

        public static event Action OnSettingUpdated;
        
        private static SerializedGameSettings _settings;
        
        public static void Initialize()
        {
            _settings = DataHandler.Load<SerializedGameSettings>(KeySettings, new SerializedGameSettings());

            KeyPause = _settings.keyPause;
            EnableUISound = _settings.enableUISound;
            EnableInGameSound = _settings.enableInGameSound;
            EnableOutGameSound = _settings.enableOutGameSound;
            EnableMusic = _settings.enableMusic;
        }

        private static void SaveSettingsData()
        {
            _settings ??= new SerializedGameSettings();

            _settings.keyPause = KeyPause;
            _settings.enableUISound = EnableUISound;
            _settings.enableInGameSound = EnableInGameSound;
            _settings.enableOutGameSound = EnableOutGameSound;
            _settings.enableMusic = EnableMusic;
        }

        public static void Save()
        {
            SaveSettingsData();
            DataHandler.Save<SerializedGameSettings>(KeySettings, _settings);
            OnSettingUpdated?.Invoke();
        }
        
        [Serializable]
        public class SerializedGameSettings
        {
            public KeyCode keyPause = KeyCode.Escape;

            public bool enableUISound = true;
            public bool enableInGameSound = true;
            public bool enableOutGameSound = true;
            public bool enableMusic = true;
        }
    }
}