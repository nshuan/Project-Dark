using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Sirenix.OdinInspector;
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
        [SerializeField] private AudioSourceInfo[] sources;
            
        [Header("Settings")]
        public int initialPoolSize = 10; // starting number of AudioSources

        protected override void Awake()
        {
            base.Awake();
            
            Refresh();
        }

        private void Start()
        {
            for (int index = 0; index < sources.Length; index++)
            {
                sources[index].component.Source = sources[index].component.GetComponent<AudioSource>();
                var pool = new List<AudioSource>();
                for (int i = 0; i < initialPoolSize; i++)
                {
                    var sourceSub = new GameObject($"{sources[index].component.name} - {i}");
                    sourceSub.transform.SetParent(sources[index].component.transform);
                    var sourceComponent = sourceSub.AddComponent<AudioSource>();
                    pool.Add(sourceComponent);
                }
                sources[index].component.SourcePool = pool;
                sources[index].component.UpdateSourceInPool();
                sources[index].component.NextIndex = 0;
            }
        }

        /// <summary>
        /// Plays a sound effect immediately or after a delay.
        /// </summary>
        public void PlaySFX(int index, float volume = -1f, float pitch = -10f, float delay = 0f)
        {
            sources[index].component.PlaySFX(volume, pitch, delay);
        }
        
        [Serializable]
        public class AudioSourceInfo
        {
            public int index;
            public AudioSourceComponent component;
        }

        [Button]
        private void Refresh()
        {
            var components = GetComponentsInChildren<AudioSourceComponent>();
            sources = new AudioSourceInfo[components.Length];
            for (var i = 0; i < sources.Length; i++)
            {
                sources[i] = new AudioSourceInfo()
                {
                    index = i,
                    component = components[i]
                };
            }
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            Refresh();
        }
#endif
    }
}