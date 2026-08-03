using System;
using System.Collections;
using System.Collections.Generic;
using Dark.Tools.Language.Runtime;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dark.Scripts.OutGame.Intro
{
    public class UIIntroSubtitleHandler : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtSubtitle;
        [SerializeField] private CanvasGroup cvgSubtitle;
        [SerializeField] private float subtitleFadeDuration = 0.35f;
        [SerializeField] private float defaultSubtitleDuration = 3f;
        [SerializeField] private IntroSubtitleCue[] subtitleCues;

        private Coroutine coroutineSubtitle;
        private Sequence sequenceSubtitle;
        private string currentSubtitleKey;
        private float subtitleTextBaseAlpha = 1f;

        private void Awake()
        {
            Initialize();
        }

        private void OnEnable()
        {
            Initialize();
            LanguageManager.Instance.RegisterForceUpdate(OnForceUpdateLanguage);
        }

        private void OnDisable()
        {
            LanguageManager.Instance.UnregisterForceUpdate(OnForceUpdateLanguage);
            StopSubtitles(true);
        }

        public void Play()
        {
            Initialize();
            StopSubtitles(true);

            if (txtSubtitle == null || subtitleCues == null || subtitleCues.Length == 0)
                return;

            coroutineSubtitle = StartCoroutine(IEPlaySubtitles());
        }

        public void StopSubtitles(bool clear)
        {
            if (coroutineSubtitle != null)
            {
                StopCoroutine(coroutineSubtitle);
                coroutineSubtitle = null;
            }

            sequenceSubtitle?.Kill();
            sequenceSubtitle = null;

            if (clear)
                ClearSubtitle();
        }

        private void Initialize()
        {
            currentSubtitleKey = string.Empty;

            if (txtSubtitle != null)
            {
                subtitleTextBaseAlpha = txtSubtitle.color.a > 0f ? txtSubtitle.color.a : 1f;
                txtSubtitle.SetText(string.Empty);
            }

            SetSubtitleAlpha(0f);
        }

        private IEnumerator IEPlaySubtitles()
        {
            var cues = GetSubtitleCues();

            for (var i = 0; i < cues.Count; i++)
            {
                var cue = cues[i];

                if (cue.delayBefore > 0f)
                    yield return new WaitForSeconds(cue.delayBefore);

                yield return ShowSubtitle(cue.languageKey).Play().WaitForCompletion();

                var duration = GetCueDuration(cue);
                if (duration > 0f)
                    yield return new WaitForSeconds(duration);

                if (i + 1 >= cues.Count || cues[i + 1].delayBefore > 0f)
                    yield return HideSubtitle().Play().WaitForCompletion();
            }

            coroutineSubtitle = null;
        }

        private List<IntroSubtitleCue> GetSubtitleCues()
        {
            var cues = new List<IntroSubtitleCue>();
            foreach (var cue in subtitleCues)
            {
                if (cue == null || string.IsNullOrEmpty(cue.languageKey))
                    continue;

                cues.Add(cue);
            }

            return cues;
        }

        private float GetCueDuration(IntroSubtitleCue cue)
        {
            if (cue.duration > 0f)
                return cue.duration;

            return Mathf.Max(0f, defaultSubtitleDuration);
        }

        private Tween ShowSubtitle(string languageKey)
        {
            if (txtSubtitle == null || string.IsNullOrEmpty(languageKey))
                return null;

            sequenceSubtitle?.Kill();

            var fadeDuration = Mathf.Max(0f, subtitleFadeDuration);
            sequenceSubtitle = DOTween.Sequence(txtSubtitle);

            if (fadeDuration > 0f && GetSubtitleAlpha() > 0f)
                sequenceSubtitle.Append(DOTween.To(GetSubtitleAlpha, SetSubtitleAlpha, 0f, fadeDuration));
            else
                SetSubtitleAlpha(0f);

            sequenceSubtitle.AppendCallback(() =>
            {
                currentSubtitleKey = languageKey;
                txtSubtitle.SetTextLanguage(languageKey);
            });

            if (fadeDuration > 0f)
                sequenceSubtitle.Append(DOTween.To(GetSubtitleAlpha, SetSubtitleAlpha, 1f, fadeDuration));
            else
                sequenceSubtitle.AppendCallback(() => SetSubtitleAlpha(1f));

            return sequenceSubtitle;
        }

        private Tween HideSubtitle()
        {
            if (txtSubtitle == null)
                return null;

            sequenceSubtitle?.Kill();

            var fadeDuration = Mathf.Max(0f, subtitleFadeDuration);
            sequenceSubtitle = DOTween.Sequence(txtSubtitle);

            if (fadeDuration > 0f)
            {
                sequenceSubtitle.Append(DOTween.To(GetSubtitleAlpha, SetSubtitleAlpha, 0f, fadeDuration))
                    .AppendCallback(ClearSubtitle);
            }
            else
            {
                ClearSubtitle();
            }

            return sequenceSubtitle;
        }

        private void ClearSubtitle()
        {
            currentSubtitleKey = string.Empty;
            if (txtSubtitle != null)
                txtSubtitle.SetText(string.Empty);

            SetSubtitleAlpha(0f);
        }

        private float GetSubtitleAlpha()
        {
            if (cvgSubtitle != null)
                return cvgSubtitle.alpha;

            if (txtSubtitle != null && subtitleTextBaseAlpha > 0f)
                return txtSubtitle.color.a / subtitleTextBaseAlpha;

            return 0f;
        }

        private void SetSubtitleAlpha(float alpha)
        {
            alpha = Mathf.Clamp01(alpha);

            if (cvgSubtitle != null)
            {
                cvgSubtitle.alpha = alpha;
                return;
            }

            if (txtSubtitle == null)
                return;

            var color = txtSubtitle.color;
            color.a = subtitleTextBaseAlpha * alpha;
            txtSubtitle.color = color;
        }

        private void OnForceUpdateLanguage()
        {
            if (txtSubtitle == null || string.IsNullOrEmpty(currentSubtitleKey))
                return;

            txtSubtitle.SetTextLanguage(currentSubtitleKey);
        }

        [Serializable]
        private class IntroSubtitleCue
        {
            public string languageKey;
            [FormerlySerializedAs("startTime")]
            [Min(0f)] public float delayBefore;
            [Min(0f)] public float duration = 3f;
        }
    }
}
