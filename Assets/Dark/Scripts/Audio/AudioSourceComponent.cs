using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI.Extensions;
using Random = UnityEngine.Random;

namespace Dark.Scripts.Audio
{
    public class AudioSourceComponent : MonoBehaviour
    {
        [Header("Customization")] 
        [SerializeField] private bool enableRandomPitch = false;
        [SerializeField] private float randomPitchMin;
        [SerializeField] private float randomPitchMax;
        
        public AudioSource Source { get; set; }
        public List<AudioSource> SourcePool { get; set; }
        public int NextIndex { get; set; }

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
        
        /// <summary>
        /// Plays a sound effect immediately or after a delay.
        /// </summary>
        public void PlaySFX(float volume = -1f, float pitch = -10f, float delay = 0f)
        {
            AudioSource src = SourcePool[NextIndex];
            NextIndex = (NextIndex + 1) % SourcePool.Count;
            
            src.volume = volume < 0 ? src.volume : volume;
            src.pitch = pitch < -9f ? src.pitch : pitch;
            if (pitch < -9f)
            {
                src.pitch = enableRandomPitch ? Random.Range(randomPitchMin, randomPitchMax) : src.pitch;
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
                sourceInPool.volume = Source.volume;
                sourceInPool.pitch = Source.pitch;
                sourceInPool.panStereo = Source.panStereo;
                sourceInPool.spatialBlend = Source.spatialBlend;
                sourceInPool.reverbZoneMix = Source.reverbZoneMix;
                sourceInPool.playOnAwake = false;
            }
        }
    }
}