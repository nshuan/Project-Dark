using System;
using Dark.Scripts.Settings;
using UnityEngine;

namespace Dark.Scripts.AudioV2
{
    public class AudioSettingController : MonoBehaviour
    {
        AudioManagerV2 audioManager;
        
        public static Action OnMusicSettingChanged { get; set; }
        
        private void Awake()
        {
            audioManager = AudioManagerV2.Instance;
            OnSettingsUpdated();
            GameSettings.OnSettingInitialized += OnSettingsInitialized;
            GameSettings.OnSettingUpdated += OnSettingsUpdated;
        }

        private void OnSettingsInitialized()
        {
            OnSettingsUpdated();
        }

        private void OnSettingsUpdated()
        {
            audioManager.BlockPlayMusic = false;
            audioManager.BlockPlayInGame = !GameSettings.EnableInGameSound;
            audioManager.BlockPlayOutGame = !GameSettings.EnableOutGameSound;
            audioManager.BlockPlayUi = !GameSettings.EnableUISound;

            audioManager.SetChannelVolume(AudioChannel.Music, GameSettings.VolumeMusic);
            audioManager.SetChannelVolume(AudioChannel.Ui, GameSettings.VolumeUI);
            audioManager.SetChannelVolume(AudioChannel.InGame, GameSettings.VolumeInGame);
            audioManager.SetChannelVolume(AudioChannel.OutGame, GameSettings.VolumeOutGame);
            
            OnMusicSettingChanged?.Invoke();
            
            // if (AudioManagerV2.Instance.BlockPlayMusic == false && GameSettings.EnableMusic == false)
            // {
            //     audioManager.BlockPlayMusic = true;
            //     OnMusicSettingChanged?.Invoke();
            // }
            // else if (AudioManagerV2.Instance.BlockPlayMusic == true && GameSettings.EnableMusic == true)
            // {
            //     audioManager.BlockPlayMusic = false;
            //     OnMusicSettingChanged?.Invoke();
            // }
        }        
    }
}