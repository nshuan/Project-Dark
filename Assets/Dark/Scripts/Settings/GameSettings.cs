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
        public static float VolumeUI;
        public static float VolumeInGame;
        public static float VolumeOutGame;
        public static float VolumeMusic;

        [Header("Hot keys settings")] 
        public static KeyCode KeyMoveTower0;
        public static KeyCode KeyMoveTower1;
        public static KeyCode KeyMoveTower2;

        [Header("In-game settings")] 
        public static bool ShowEnemyHealth;
        public static bool ShowBossHealth;
        
        public static event Action OnSettingUpdated;
        public static event Action OnSettingInitialized;
        
        private static SerializedGameSettings _settings;
        
        public static void Initialize()
        {
            _settings = DataHandler.Load<SerializedGameSettings>(KeySettings, new SerializedGameSettings());

            KeyPause = _settings.keyPause;
            EnableUISound = _settings.enableUISound;
            EnableInGameSound = _settings.enableInGameSound;
            EnableOutGameSound = _settings.enableOutGameSound;
            EnableMusic = _settings.enableMusic;
            VolumeUI = _settings.volumeUI;
            VolumeInGame = _settings.volumeInGame;
            VolumeOutGame = _settings.volumeOutGame;
            VolumeMusic = _settings.volumeMusic;
            KeyMoveTower0 = _settings.keyMoveTower0;
            KeyMoveTower1 = _settings.keyMoveTower1;
            KeyMoveTower2 = _settings.keyMoveTower2;
            ShowEnemyHealth = _settings.showEnemyHealth;
            ShowBossHealth = _settings.showBossHealth;
            
            OnSettingInitialized?.Invoke();
        }

        private static void SaveSettingsData()
        {
            _settings ??= new SerializedGameSettings();

            _settings.keyPause = KeyPause;
            _settings.enableUISound = EnableUISound;
            _settings.enableInGameSound = EnableInGameSound;
            _settings.enableOutGameSound = EnableOutGameSound;
            _settings.enableMusic = EnableMusic;
            _settings.volumeUI = VolumeUI;
            _settings.volumeInGame = VolumeInGame;
            _settings.volumeOutGame = VolumeOutGame;
            _settings.volumeMusic = VolumeMusic;
            _settings.keyMoveTower0 = KeyMoveTower0;
            _settings.keyMoveTower1 = KeyMoveTower1;
            _settings.keyMoveTower2 = KeyMoveTower2;
            _settings.showEnemyHealth = ShowEnemyHealth;
            _settings.showBossHealth = ShowBossHealth;
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
            public float volumeUI = 1f;
            public float volumeInGame = 1f;
            public float volumeOutGame = 1f;
            public float volumeMusic = 1f;

            public KeyCode keyMoveTower0 = KeyCode.Alpha1;
            public KeyCode keyMoveTower1 = KeyCode.Alpha2;
            public KeyCode keyMoveTower2 = KeyCode.Alpha3;

            public bool showEnemyHealth = true;
            public bool showBossHealth = true;
        }
    }
}