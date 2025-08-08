using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Dark.Scripts.Audio
{
    /// <summary>
    /// Index
    /// 0 = Shoot
    /// 1 = Tower hit
    /// 2 = Lightning passive
    /// 3 = Thunder passive
    /// 4 = Dash
    /// 5 = Flash
    /// </summary>
    public class AudioManager : MonoSingleton<AudioManager>
    {
        [SerializeField] private AudioSource[] sources;

        private Dictionary<int, List<AudioSource>> sourcePoolMap;
            
        [Header("Settings")]
        public int initialPoolSize = 10; // starting number of AudioSources

        private Dictionary<int, int> nextIndexMap;

        private void Awake()
        {
            sourcePoolMap =  new Dictionary<int, List<AudioSource>>();
            nextIndexMap = new Dictionary<int, int>();
            for (int index = 0; index < sources.Length; index++)
            {
                var pool = new List<AudioSource>();
                for (int i = 0; i < initialPoolSize; i++)
                {
                    var sourceSub = new GameObject($"{sources[index].name} - {i}");
                    sourceSub.transform.SetParent(sources[index].transform);
                    var sourceComponent = sourceSub.AddComponent<AudioSource>();
                    
                    sourceComponent.clip = sources[index].clip;
                    sourceComponent.playOnAwake = sources[index].playOnAwake;
                    sourceComponent.loop = sources[index].loop;
                    sourceComponent.priority = sources[index].priority;
                    sourceComponent.volume = sources[index].volume;
                    sourceComponent.pitch = sources[index].pitch;
                    sourceComponent.panStereo = sources[index].panStereo;
                    sourceComponent.spatialBlend = sources[index].spatialBlend;
                    sourceComponent.reverbZoneMix = sources[index].reverbZoneMix;
                    sourceComponent.playOnAwake = false;
                    
                    pool.Add(sourceComponent);
                }
                sourcePoolMap[index] = pool;
                nextIndexMap[index] = 0;
            }
        }

        /// <summary>
        /// Plays a sound effect immediately or after a delay.
        /// </summary>
        public void PlaySFX(int index, float volume = -1f, float pitch = -10f, float delay = 0f)
        {
            AudioSource src = sourcePoolMap[index][nextIndexMap[index]];
            nextIndexMap[index] = (nextIndexMap[index] + 1) % sourcePoolMap[index].Count;
            
            src.volume = volume < 0 ? src.volume : volume;
            src.pitch = pitch < -9f ? src.pitch : pitch;

            if (delay > 0f)
                src.PlayDelayed(delay);
            else
                src.Play();
        }

    }
}