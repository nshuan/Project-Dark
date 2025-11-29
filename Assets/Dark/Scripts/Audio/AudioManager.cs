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
                for (int i = 0; i < sources[index].poolSize; i++)
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

        public bool IsMuted => AudioListener.volume <= 0.0001f || AudioListener.pause; 
        public void Mute()
        {
            AudioListener.volume = 0f;
        }

        public void Unmute()
        {
            AudioListener.volume = 1f;
        }
        
        [Serializable]
        public class AudioSourceInfo
        {
            public int index;
            public AudioSourceComponent component;
            public int poolSize = 1;
        }

        [Button]
        private void Refresh()
        {
            var components = GetComponentsInChildren<AudioSourceComponent>();
            var sourceList = new List<AudioSourceInfo>();
            if (sources != null)
            {
                foreach (var source in sources)
                {
                    sourceList.Add(source);
                }
            }
            
            for (var i = 0; i < components.Length; i++)
            {
                if (i < sourceList.Count && ReferenceEquals(components[i], sourceList[i].component))
                    continue;
                if (i < sourceList.Count)
                {
                    sourceList[i] = new AudioSourceInfo()
                    {
                        index = i,
                        component = components[i],
                    };
                }
                else
                {
                    sourceList.Add(new  AudioSourceInfo()
                    {
                        index = i,
                        component = components[i],
                    });
                }
            }
            sources = sourceList.ToArray();
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            Refresh();
        }
#endif
    }
}