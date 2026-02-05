using System;
using System.Collections;
using Dark.Scripts.Audio;
using Dark.Scripts.AudioV2;
using Dark.Scripts.SceneNavigation;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Dark.Scripts.OutGame.Intro
{
    public class UIIntroScene : MonoBehaviour
    {
        [SerializeField] private Image imgCover;
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Color loadingGameBgColor;
        [SerializeField] private Button btnSkip;
        [SerializeField] private Button btnShowSkip;
        [SerializeField] private CanvasGroup cvgSkip;
        [SerializeField] private float delayShowBtnSkip = 3f;

        public static Action OnCompleteIntro { get; set; }
        private Coroutine coroutineIntro;

        private void Start()
        {
            btnShowSkip.onClick.RemoveAllListeners();
            btnShowSkip.onClick.AddListener(UpdateSkipButton);
            btnSkip.onClick.RemoveAllListeners();
            btnSkip.onClick.AddListener(() =>
            {
                btnSkip.interactable = false;
                DOTween.Kill(btnSkip);
                cvgSkip.alpha = 1f; 
                if (coroutineIntro != null) StopCoroutine(coroutineIntro);
                LoadGame(true);
            });
            btnSkip.gameObject.SetActive(false);
            ShowSkip();
            ShowIntro();
        }

        private void ShowIntro()
        {
            videoPlayer.Pause();
            coroutineIntro = StartCoroutine(IEIntro());
        }

        private IEnumerator IEIntro()
        {
            yield return new WaitForSeconds(2f);
            imgCover.gameObject.SetActive(false);
            videoPlayer.Play();
            yield return new WaitForSeconds((float)videoPlayer.length);
            LoadGame(false);
        }
        
        private void LoadGame(bool fadeSound)
        {
            Action actionLoadGame = () =>
            {
                Loading.Instance.OverrideQuickLoadBgColorOnce(loadingGameBgColor);
                OnCompleteIntro?.Invoke();
                OnCompleteIntro = null;
            };

            if (fadeSound)
            {
                DOTween.Kill(audioSource);
                DOTween.Sequence(audioSource)
                    .AppendCallback(() =>
                    {
                        imgCover.SetAlpha(0f);
                        imgCover.gameObject.SetActive(true);
                    })
                    .Append(DOTween.To(() => 1f, x => audioSource.volume = x, 0f, 1.5f))
                    .Join(imgCover.DOFade(1f, 1.5f))
                    .OnComplete(() =>
                    {
                        actionLoadGame?.Invoke();        
                    });
            }
            else
            {
                actionLoadGame?.Invoke();
            }
        }

        private void ShowSkip()
        {
            if (btnSkip.gameObject.activeSelf) return;
            cvgSkip.alpha = 0f;
            btnSkip.gameObject.SetActive(true);
            btnShowSkip.interactable = false;
            DOTween.Kill(btnSkip);
            DOTween.Sequence(btnSkip)
                .AppendInterval(delayShowBtnSkip)
                .Append(cvgSkip.DOFade(1f, 1f))
                .AppendCallback(() =>
                {
                    btnShowSkip.interactable = true;
                });
            // .Append(cvgSkip.DOFade(0f, 1f))
            // .AppendCallback(() => btnSkip.gameObject.SetActive(false));
        }

        private void UpdateSkipButton()
        {
            DOTween.Kill(btnSkip);
            var seq = DOTween.Sequence(btnSkip);
            if (btnSkip.gameObject.activeSelf)
            {
                seq.Append(cvgSkip.DOFade(0f, 1f))
                    .AppendCallback(() =>
                    {
                        btnSkip.gameObject.SetActive(false);
                    });
            }
            else
            {
                seq.AppendCallback(() =>
                {
                    cvgSkip.alpha = 0f;
                    cvgSkip.gameObject.SetActive(true);
                })
                .Append(cvgSkip.DOFade(1f, 1f));
            }

            seq.Play();
        }
        
        private void OnDestroy()
        {
            OnCompleteIntro = null;
        }
    }
}