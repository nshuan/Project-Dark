using Dark.Scripts.Settings;
using UnityEngine;

namespace Dark.Scripts.AudioV2
{
    public class AudioSettingController : MonoBehaviour
    {
        AudioManagerV2 audioManager;
        
        private void Awake()
        {
            audioManager = AudioManagerV2.Instance;
            OnSettingsUpdated();
            GameSettings.OnSettingUpdated += OnSettingsUpdated;
        }

        private void OnSettingsUpdated()
        {
            audioManager.BlockPlayInGame = !GameSettings.EnableInGameSound;
            audioManager.BlockPlayOutGame = !GameSettings.EnableOutGameSound;
            audioManager.BlockPlayUi = !GameSettings.EnableUISound;
        }        
    }
}