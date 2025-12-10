using System;
using UnityEngine;

namespace Dark.Scripts.Settings
{
    public class GameSettings
    {
        [Header("Control settings")] 
        public static KeyCode KeyPause = KeyCode.Escape;

        [Header("Audio settings")] 
        public static bool EnableUISound = true;
        public static bool EnableInGameSound = true;
        public static bool EnableOutGameSound = true;
        public static bool EnableMusic = true;

        public static event Action OnSettingUpdated;
        
        public static void Initialize()
        {
            
        }

        public static void Save()
        {
            OnSettingUpdated?.Invoke();
        }
    }
}