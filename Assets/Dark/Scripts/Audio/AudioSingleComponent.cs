using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Dark.Scripts.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSingleComponent : MonoBehaviour
    {
        [Header("General")] 
        [SerializeField] private int loop = 1;
        [SerializeField] private bool playOnStart = true;
        [SerializeField] private float delay;
        
        [Space]
        [Header("Fade in")]
        [SerializeField] private Ease fadeInEasing;
        [SerializeField] private float fadeInDuration;
        
        [Space]
        [Header("Fade out")]
        [SerializeField] private Ease fadeOutEasing;
        [SerializeField] private float fadeOutDuration;
        
        private AudioSource audio;
        private Coroutine coroutinePlay;
        private float volume;
        private int tempLoop;

        private void Awake()
        {
            audio = GetComponent<AudioSource>();
            audio.playOnAwake = false;
            audio.loop = false;
            volume = audio.volume;
            audio.Stop();
        }

        private void Start()
        {
            if (playOnStart)
                Play();
        }

        public void Play(float delay = -1f)
        {
            if (delay < 0f) delay = this.delay;
            tempLoop = loop;
            if (coroutinePlay != null) StopCoroutine(coroutinePlay);
            coroutinePlay = StartCoroutine(IEPlay(delay));
        }

        private IEnumerator IEPlay(float delay)
        {
            if (fadeInDuration > audio.clip.length) fadeInDuration = audio.clip.length;
            if (fadeInDuration + fadeOutDuration > audio.clip.length) fadeOutDuration = audio.clip.length - fadeInDuration;
            yield return new WaitForSecondsRealtime(delay);

            while (tempLoop != 0)
            {
                audio.volume = 0f;
                audio.Play();
                yield return audio.DOFade(volume, fadeInDuration).SetEase(fadeInEasing).SetUpdate(true).WaitForCompletion();
                yield return new WaitForSeconds(audio.clip.length - fadeInDuration - fadeOutDuration);
                yield return audio.DOFade(0f, fadeOutDuration).SetEase(fadeOutEasing).SetUpdate(true).WaitForCompletion();
                tempLoop -= 1;
            }
        }
    }
}
