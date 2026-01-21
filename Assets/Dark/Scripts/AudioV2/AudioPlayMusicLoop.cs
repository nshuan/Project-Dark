using System;
using Dark.Scripts.Settings;
using UnityEngine;

namespace Dark.Scripts.AudioV2
{
    public class AudioPlayMusicLoop : MonoBehaviour
    {
        [SerializeField] private string introCueKey;
        [SerializeField] private string loopCueKey;
        [SerializeField] private float fadeDuration = -1f;
        
        private void Start()
        {
            if (GameSettings.EnableMusic)
            {
                AudioManagerV2.Instance.StopMusic();
                AudioManagerV2.Instance.PlayMusicIntroThenLoop(introCueKey, loopCueKey, fadeDuration);
            }

            // AudioSettingController.OnMusicSettingChanged += OnMusicSettingChanged;
        }

        private void OnDestroy()
        {
            // AudioSettingController.OnMusicSettingChanged -= OnMusicSettingChanged;
        }

        private void OnMusicSettingChanged()
        {
            AudioManagerV2.Instance.StopMusic();
            
            if (GameSettings.EnableMusic)
            {
                AudioManagerV2.Instance.PlayMusicIntroThenLoop(introCueKey, loopCueKey, fadeDuration);
            }
        }
    }
}