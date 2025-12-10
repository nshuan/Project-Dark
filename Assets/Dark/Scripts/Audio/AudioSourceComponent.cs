using System;
using System.Collections.Generic;
using Core;
using Dark.Scripts.AudioV2;
using Dark.Scripts.Settings;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI.Extensions;
using Random = UnityEngine.Random;

namespace Dark.Scripts.Audio
{
    public class AudioSourceComponent : MonoBehaviour
    {
        [SerializeField] private AudioChannel audioType;
        
        [Header("Customization")] 
        [SerializeField] private bool enableRandomPitch = false;
        [SerializeField] private float randomPitchMin;
        [SerializeField] private float randomPitchMax;
        
        public AudioSource Source { get; set; }
        public List<AudioSource> SourcePool { get; set; }
        public int NextIndex { get; set; }
        private int settingVolumeEnabled = 1;

        [Button]
        public void ApplyChange()
        {
            UpdateSourceInPool();
        }
        
        [Button]
        public void PlayTest()
        {
            PlaySFX();
        }

        private void Start()
        {
            OnSettingsUpdated();
            
            GameSettings.OnSettingUpdated += OnSettingsUpdated;
        }

        private void OnDestroy()
        {
            GameSettings.OnSettingUpdated -= OnSettingsUpdated;
        }

        private void OnSettingsUpdated()
        {
            switch (audioType)
            {
                case AudioChannel.Ui:
                    settingVolumeEnabled = GameSettings.EnableUISound ? 1 : 0;
                    break;
                case AudioChannel.Music:
                    settingVolumeEnabled = GameSettings.EnableMusic ? 1 : 0;
                    break;
                case AudioChannel.InGame:
                    settingVolumeEnabled = GameSettings.EnableInGameSound ? 1 : 0;
                    break;
                case AudioChannel.OutGame:
                    settingVolumeEnabled = GameSettings.EnableOutGameSound ? 1 : 0;
                    break;
            }   

            foreach (var sourceInPool in SourcePool)
            {
                sourceInPool.volume = Source.volume * settingVolumeEnabled;
            }
        }

        /// <summary>
        /// Plays a sound effect immediately or after a delay.
        /// </summary>
        public void PlaySFX(float volume = -1f, float pitch = -10f, float delay = 0f)
        {
            AudioSource src = SourcePool[NextIndex];
            NextIndex = (NextIndex + 1) % SourcePool.Count;
            
            src.volume = volume < 0 ? src.volume : volume * settingVolumeEnabled;
            src.pitch = pitch < -9f ? src.pitch : pitch;
            if (pitch < -9f)
            {
                src.pitch = enableRandomPitch ? RandomUtil.Range(randomPitchMin, randomPitchMax) : src.pitch;
            }
            else
            {
                src.pitch = pitch;
            }

            if (delay > 0f)
                src.PlayDelayed(delay);
            else
                src.Play();
        }

        public void UpdateSourceInPool()
        {
            foreach (var sourceInPool in SourcePool)
            {
                sourceInPool.clip = Source.clip;
                sourceInPool.playOnAwake = Source.playOnAwake;
                sourceInPool.loop = Source.loop;
                sourceInPool.priority = Source.priority;
                sourceInPool.volume = Source.volume * settingVolumeEnabled;
                sourceInPool.pitch = Source.pitch;
                sourceInPool.panStereo = Source.panStereo;
                sourceInPool.spatialBlend = Source.spatialBlend;
                sourceInPool.reverbZoneMix = Source.reverbZoneMix;
                sourceInPool.playOnAwake = false;
            }
        }
    }
}