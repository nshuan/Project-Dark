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
        [SerializeField] private AudioSourceComponent[] sources;
            
        [Header("Settings")]
        public int initialPoolSize = 10; // starting number of AudioSources

        private void Start()
        {
            for (int index = 0; index < sources.Length; index++)
            {
                sources[index].Source = sources[index].GetComponent<AudioSource>();
                var pool = new List<AudioSource>();
                for (int i = 0; i < initialPoolSize; i++)
                {
                    var sourceSub = new GameObject($"{sources[index].name} - {i}");
                    sourceSub.transform.SetParent(sources[index].transform);
                    var sourceComponent = sourceSub.AddComponent<AudioSource>();
                    pool.Add(sourceComponent);
                }
                sources[index].SourcePool = pool;
                sources[index].UpdateSourceInPool();
                sources[index].NextIndex = 0;
            }
        }

        /// <summary>
        /// Plays a sound effect immediately or after a delay.
        /// </summary>
        public void PlaySFX(int index, float volume = -1f, float pitch = -10f, float delay = 0f)
        {
            sources[index].PlaySFX(volume, pitch, delay);
        }
    }
}