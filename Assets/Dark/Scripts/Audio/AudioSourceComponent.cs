using System;
using System.Collections.Generic;
using Core;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI.Extensions;
using Random = UnityEngine.Random;

namespace Dark.Scripts.Audio
{
    public class AudioSourceComponent : MonoBehaviour
    {
        [SerializeField] private AudioPlayType audioType;
        
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
            if (audioType == AudioPlayType.Sound) settingVolumeEnabled = Settings.GameSettings.EnableSound ? 1 : 0;
            else settingVolumeEnabled = Settings.GameSettings.EnableMusic ? 1 : 0;
            
            Settings.GameSettings.OnSettingUpdated += OnSettingUpdated;
        }

        private void OnDestroy()
        {
            Settings.GameSettings.OnSettingUpdated -= OnSettingUpdated;
        }

        private void OnSettingUpdated()
        {
            if (audioType == AudioPlayType.Sound) settingVolumeEnabled = Settings.GameSettings.EnableSound ? 1 : 0;
            else settingVolumeEnabled = Settings.GameSettings.EnableMusic ? 1 : 0;

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