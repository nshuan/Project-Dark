using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dark.Scripts.AudioV2
{
    public enum AudioChannel
    {
        Music,
        InGame,
        OutGame,
        Ui
    }

    [Serializable]
    public class AudioCue
    {
        [HorizontalGroup("Id")] public string key;
        [HorizontalGroup("Id")] public AudioChannel channel = AudioChannel.InGame;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        public bool loop = false;
        [Range(0f, 1f)] public float spatialBlend = 0f;
        [MinMaxSlider(0.5f, 2f, ShowFields = true)] public Vector2 pitchRange = Vector2.one;
        [MinValue(1)] public int preloadPool = 4;
        [MinValue(1)] public int maxPool = 16;
        [Tooltip("When the pool is saturated we steal the oldest source. Set to false to skip the play request instead.")]
        public bool stealWhenExhausted = true;
    }

    [Serializable]
    public class SceneMusicEntry
    {
        public string sceneName;
        public string cueKey;
        public float fadeDuration = 0.6f;
    }

    /// <summary>
    /// Optimized audio manager with pooled SFX (stacking) and music cross-fades.
    /// </summary>
    public sealed class AudioManagerV2 : MonoSingleton<AudioManagerV2>
    {
        [FoldoutGroup("General"), SerializeField] private List<AudioCue> cues = new();
        [FoldoutGroup("General"), SerializeField] private Transform poolRoot;
        [FoldoutGroup("Music"), SerializeField] private bool autoPlaySceneMusic = true;
        [FoldoutGroup("Music"), SerializeField] private List<SceneMusicEntry> sceneMusic = new();
        [FoldoutGroup("Music"), SerializeField] private float defaultMusicFade = 0.6f;

        private readonly Dictionary<string, AudioCuePool> pools = new();
        private readonly Dictionary<AudioChannel, float> channelVolumes = new();
        private AudioSource musicA;
        private AudioSource musicB;
        private bool musicUsingA = true;
        private string currentMusicKey;
        private Coroutine musicRoutine;

        protected override void Awake()
        {
            base.Awake();

            InitChannelVolumes();
            EnsurePoolRoot();
            BuildPools();
            InitMusicSources();

            if (autoPlaySceneMusic)
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
        }

        private void OnDestroy()
        {
            if (autoPlaySceneMusic)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }

        private void InitChannelVolumes()
        {
            channelVolumes[AudioChannel.Music] = 1f;
            channelVolumes[AudioChannel.InGame] = 1f;
            channelVolumes[AudioChannel.OutGame] = 1f;
            channelVolumes[AudioChannel.Ui] = 1f;
        }

        private void EnsurePoolRoot()
        {
            if (poolRoot != null) return;

            var go = new GameObject("AudioV2_Pools");
            poolRoot = go.transform;
            poolRoot.SetParent(transform);
            poolRoot.localPosition = Vector3.zero;
        }

        private void BuildPools()
        {
            pools.Clear();

            foreach (var cue in cues.Where(c => c.clip != null && !string.IsNullOrWhiteSpace(c.key)))
            {
                if (pools.ContainsKey(cue.key))
                    continue;

                pools.Add(cue.key, new AudioCuePool(cue, poolRoot));
            }
        }

        private void InitMusicSources()
        {
            musicA = CreateMusicSource("Music_A");
            musicB = CreateMusicSource("Music_B");
        }

        private AudioSource CreateMusicSource(string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>();
            src.loop = true;
            src.playOnAwake = false;
            src.spatialBlend = 0f;
            return src;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var match = sceneMusic.FirstOrDefault(m =>
                string.Equals(m.sceneName, scene.name, StringComparison.OrdinalIgnoreCase));

            if (match != null && !string.IsNullOrEmpty(match.cueKey))
            {
                PlayMusic(match.cueKey, match.fadeDuration > 0f ? match.fadeDuration : defaultMusicFade);
            }
        }
        
        private bool TryGetMusicPool(string key, out AudioCuePool pool)
        {
            if (!pools.TryGetValue(key, out pool) || pool.Cue.channel != AudioChannel.Music)
            {
                Debug.LogWarning($"[AudioManagerV2] Music cue '{key}' not found or not marked as Music.");
                return false;
            }
            return true;
        }

        #region Public API

        public bool BlockPlayInGame { get; set; }
        public AudioSource PlayInGame(string key, Vector3? worldPos = null, float volumeScale = 1f, float? pitch = null)
        {
            if (BlockPlayInGame) volumeScale = 0f;
            return PlayInternal(key, AudioChannel.InGame, worldPos, volumeScale, pitch);
        }

        public bool BlockPlayOutGame { get; set; }
        public AudioSource PlayOutGame(string key, Vector3? worldPos = null, float volumeScale = 1f, float? pitch = null)
        {
            if (BlockPlayOutGame) volumeScale = 0f;
            return PlayInternal(key, AudioChannel.OutGame, worldPos, volumeScale, pitch);
        }

        public bool BlockPlayUi { get; set; }
        public AudioSource PlayUi(string key, float volumeScale = 1f, float? pitch = null)
        {
            if (BlockPlayUi) volumeScale = 0f;
            return PlayInternal(key, AudioChannel.Ui, null, volumeScale, pitch);
        }

        public void PlayMusic(string cueKey, float fadeDuration = -1f)
        {
            if (!pools.TryGetValue(cueKey, out var pool) || pool.Cue.channel != AudioChannel.Music)
            {
                DebugUtility.LogWarning($"[AudioManagerV2] Music cue '{cueKey}' was not found or is not marked as Music.");
                return;
            }

            if (currentMusicKey == cueKey && ActiveMusicSource().isPlaying)
                return;

            var clip = pool.Cue.clip;
            var volume = pool.Cue.volume * GetChannelVolume(AudioChannel.Music);
            var fade = fadeDuration < 0f ? defaultMusicFade : fadeDuration;

            if (musicRoutine != null)
                StopCoroutine(musicRoutine);

            musicRoutine = StartCoroutine(CrossFadeMusic(clip, volume, fade));
            currentMusicKey = cueKey;
        }

        public void StopMusic(float fadeDuration = 0.25f)
        {
            if (musicRoutine != null)
                StopCoroutine(musicRoutine);

            musicRoutine = StartCoroutine(FadeOutActiveMusic(0f, fadeDuration));
            currentMusicKey = null;
        }

        public void FadeVolumeMusic(float volume, float fadeDuration = 0.25f)
        {
            if (musicRoutine != null)
                StopCoroutine(musicRoutine);
            
            musicRoutine = StartCoroutine(FadeActiveMusic(volume, fadeDuration));
        }

        public void SetChannelVolume(AudioChannel channel, float volume)
        {
            channelVolumes[channel] = Mathf.Clamp01(volume);
            if (channel == AudioChannel.Music)
            {
                ActiveMusicSource().volume = currentMusicKey == null ? 0f : GetMusicTargetVolume();
            }
        }

        public float GetChannelVolume(AudioChannel channel)
        {
            return channelVolumes.TryGetValue(channel, out var vol) ? vol : 1f;
        }
        
        // Public API: play an intro cue once, then loop another cue
        public void PlayMusicIntroThenLoop(string introCueKey, string loopCueKey, float fadeDuration = -1f)
        {
            if (!TryGetMusicPool(introCueKey, out var introPool) || !TryGetMusicPool(loopCueKey, out var loopPool))
                return;

            var fade = fadeDuration < 0f ? defaultMusicFade : fadeDuration;

            if (musicRoutine != null)
                StopCoroutine(musicRoutine);

            musicRoutine = StartCoroutine(PlayIntroThenLoopRoutine(introPool, loopPool, fade));
        }

        #endregion

        #region SFX

        private AudioSource PlayInternal(string key, AudioChannel expectedChannel, Vector3? worldPos,
            float volumeScale, float? pitchOverride)
        {
            if (!pools.TryGetValue(key, out var pool))
            {
                DebugUtility.LogWarning($"[AudioManagerV2] Cue '{key}' not found.");
                return null;
            }

            if (pool.Cue.channel != expectedChannel && pool.Cue.channel != AudioChannel.Music)
            {
                // still allow play but surface a warning to catch configuration mistakes
                DebugUtility.LogWarning($"[AudioManagerV2] Cue '{key}' belongs to {pool.Cue.channel} but was played as {expectedChannel}.");
            }

            var src = pool.Rent();
            if (src == null)
                return null;

            ConfigureSource(src, pool.Cue, worldPos, volumeScale, pitchOverride);
            src.Play();

            pool.MarkPlayed(src);
            return src;
        }

        private void ConfigureSource(AudioSource source, AudioCue cue, Vector3? worldPos,
            float volumeScale, float? pitchOverride)
        {
            source.clip = cue.clip;
            source.loop = cue.loop;
            source.playOnAwake = false;
            source.spatialBlend = cue.spatialBlend;
            if (worldPos.HasValue)
                source.transform.position = worldPos.Value;
            source.pitch = pitchOverride ?? UnityEngine.Random.Range(cue.pitchRange.x, cue.pitchRange.y);
            source.volume = cue.volume * volumeScale * GetChannelVolume(cue.channel);
        }

        #endregion

        #region Music

        private IEnumerator CrossFadeMusic(AudioClip nextClip, float targetVolume, float fadeDuration)
        {
            fadeDuration = Mathf.Max(0.01f, fadeDuration);

            var incoming = IdleMusicSource();
            var outgoing = ActiveMusicSource();
            musicUsingA = incoming == musicA;

            incoming.clip = nextClip;
            incoming.volume = 0f;
            incoming.pitch = 1f;
            incoming.Play();

            var startOutVolume = outgoing.isPlaying ? outgoing.volume : 0f;
            var elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                var t = elapsed / fadeDuration;
                incoming.volume = Mathf.Lerp(0f, targetVolume, t);
                if (outgoing.isPlaying)
                    outgoing.volume = Mathf.Lerp(startOutVolume, 0f, t);

                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            incoming.volume = targetVolume;
            if (outgoing.isPlaying)
                outgoing.Stop();

            musicRoutine = null;
        }

        private IEnumerator FadeOutActiveMusic(float fadeTo, float fadeDuration)
        {
            fadeDuration = Mathf.Max(0.01f, fadeDuration);

            var active = ActiveMusicSource();
            var startVolume = active.volume;
            var elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                var t = elapsed / fadeDuration;
                active.volume = Mathf.Lerp(startVolume, fadeTo, t);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            active.Stop();
            active.clip = null;
            musicRoutine = null;
        }
        
        private IEnumerator FadeActiveMusic(float fadeTo, float fadeDuration)
        {
            fadeDuration = Mathf.Max(0.01f, fadeDuration);

            var active = ActiveMusicSource();
            var startVolume = active.volume;
            var elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                var t = elapsed / fadeDuration;
                active.volume = Mathf.Lerp(startVolume, fadeTo, t);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            
            musicRoutine = null;
        }
        
        // Coroutine that handles intro + transition to loop
        private IEnumerator PlayIntroThenLoopRoutine(AudioCuePool introPool, AudioCuePool loopPool, float fadeDuration)
        {
            fadeDuration = Mathf.Max(0.01f, fadeDuration);

            var incoming = IdleMusicSource();
            var outgoing = ActiveMusicSource();
            musicUsingA = incoming == musicA;

            var introTarget = introPool.Cue.volume * GetChannelVolume(AudioChannel.Music);
            var loopTarget = loopPool.Cue.volume * GetChannelVolume(AudioChannel.Music);
            var startOut = outgoing.isPlaying ? outgoing.volume : 0f;

            // start intro, fade out old track if any
            incoming.clip = introPool.Cue.clip;
            incoming.loop = false;
            incoming.volume = 0f;
            incoming.Play();

            var elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                var t = elapsed / fadeDuration;
                incoming.volume = Mathf.Lerp(0f, introTarget, t);
                if (outgoing.isPlaying)
                    outgoing.volume = Mathf.Lerp(startOut, 0f, t);

                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            incoming.volume = introTarget;
            if (outgoing.isPlaying)
                outgoing.Stop();

            // wait for intro to finish
            yield return new WaitWhile(() => incoming.isPlaying);

            // switch to loop clip and keep playing
            incoming.clip = loopPool.Cue.clip;
            incoming.loop = true;
            incoming.volume = loopTarget;
            incoming.Play();

            currentMusicKey = loopPool.Cue.key;
            musicRoutine = null;
        }

        private float GetMusicTargetVolume()
        {
            if (!pools.TryGetValue(currentMusicKey ?? string.Empty, out var pool))
                return 0f;
            return pool.Cue.volume * GetChannelVolume(AudioChannel.Music);
        }

        private AudioSource ActiveMusicSource()
        {
            return musicUsingA ? musicA : musicB;
        }

        private AudioSource IdleMusicSource()
        {
            return musicUsingA ? musicB : musicA;
        }

        #endregion

        #region Pool class

        private class AudioCuePool
        {
            public AudioCue Cue { get; }
            private readonly Transform parent;
            private readonly List<PooledSource> sources = new();

            private class PooledSource
            {
                public AudioSource Source;
                public float LastPlayed;
            }

            public AudioCuePool(AudioCue cue, Transform parent)
            {
                Cue = cue;
                this.parent = parent;

                Prewarm(Mathf.Clamp(cue.preloadPool, 1, cue.maxPool));
            }

            public AudioSource Rent()
            {
                for (var i = 0; i < sources.Count; i++)
                {
                    if (!sources[i].Source.isPlaying)
                        return sources[i].Source;
                }

                if (sources.Count < Cue.maxPool)
                {
                    var created = CreateSource(sources.Count);
                    sources.Add(new PooledSource { Source = created, LastPlayed = Time.unscaledTime });
                    return created;
                }

                if (!Cue.stealWhenExhausted)
                    return null;

                // steal the oldest playing source to keep audio responsive
                var oldest = sources.OrderBy(s => s.LastPlayed).First();
                oldest.Source.Stop();
                oldest.LastPlayed = Time.unscaledTime;
                return oldest.Source;
            }

            public void MarkPlayed(AudioSource source)
            {
                var entry = sources.FirstOrDefault(s => s.Source == source);
                if (entry != null)
                    entry.LastPlayed = Time.unscaledTime;
            }

            private void Prewarm(int amount)
            {
                for (var i = 0; i < amount; i++)
                {
                    var src = CreateSource(i);
                    sources.Add(new PooledSource { Source = src, LastPlayed = Time.unscaledTime });
                }
            }

            private AudioSource CreateSource(int index)
            {
                var go = new GameObject($"[{Cue.channel}] {Cue.key} #{index}");
                go.transform.SetParent(parent);
                var src = go.AddComponent<AudioSource>();
                src.clip = Cue.clip;
                src.playOnAwake = false;
                src.loop = Cue.loop;
                src.spatialBlend = Cue.spatialBlend;
                src.volume = Cue.volume;
                return src;
            }
        }

        #endregion
    }
}

