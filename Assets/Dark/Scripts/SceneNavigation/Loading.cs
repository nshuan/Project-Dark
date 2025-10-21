using System;
using Core;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Dark.Scripts.SceneNavigation
{
    public class Loading : MonoSingleton<Loading>
    {
        [SerializeField] private CanvasGroup loadingPanel;
        [SerializeField] private CanvasGroup blankPanel;
        [SerializeField] private Image progress;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private float minDuration = 0.5f;
        [SerializeField] private float maxDuration = 1.5f;
        
        public Action onStartLoading;
        private Action onSceneLoaded;
        private Action onLoadingComplete;
        private bool isQuickLoad;
        
        protected override void Awake()
        {
            base.Awake();
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            DebugUtility.LogWarning($"Scene {scene.name} is loaded!");
            if (isQuickLoad)
            {
                DoQuickClose(0.5f)
                    .OnComplete(() =>
                    {
                        onLoadingComplete?.Invoke();
                        onLoadingComplete = null;
                    });
            }
            else
            {
                DoClose(Random.Range(minDuration, maxDuration), 0.3f,0.5f)
                    .OnComplete(() =>
                    {
                        onLoadingComplete?.Invoke();
                        onLoadingComplete = null;
                    });
            }
            onSceneLoaded?.Invoke();
            onSceneLoaded = null;
        }
        
        #region Normal Load
        
        public void LoadScene(string sceneName, Action completeCallback = null, float delay = 0f)
        {
            DebugUtility.LogWarning($"Loading scene {sceneName}");
            onLoadingComplete = completeCallback;
            onStartLoading?.Invoke();
            DoOpen(0.3f, delay).OnComplete(() =>
            {
                isQuickLoad = false;
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            });
        }

        private Tween DoOpen(float duration, float delay)
        {
            loadingPanel.alpha = 0f;
            loadingPanel.gameObject.SetActive(false);
            blankPanel.alpha = 0f;
            blankPanel.gameObject.SetActive(true);
            DOTween.Kill(this);
            var seq = DOTween.Sequence(this).SetUpdate(true);
            seq.AppendInterval(delay);
            seq.Append(blankPanel.DOFade(1f, duration));
            return seq;
        }

        private Tween DoClose(float duration, float hideBlankDuration, float hideDuration)
        {
            DOTween.Kill(this);
            var seq = DOTween.Sequence(this).SetUpdate(true);
            seq.AppendCallback(() =>
                {
                    loadingPanel.alpha = 1f;
                    loadingPanel.gameObject.SetActive(true);
                    progress.fillAmount = 0f;
                    progressText.SetText($"0%");
                })
                .AppendInterval(0.5f)
                .Append(blankPanel.DOFade(0f, hideBlankDuration))
                .Append(DOTween.To(() => 0f, x =>
                {
                    progress.fillAmount = x;
                    progressText.SetText($"{(int)(x * 100)}%");
                }, 1f, duration))
                .Append(loadingPanel.DOFade(0f, hideDuration))
                .AppendCallback(() =>
                {
                    blankPanel.gameObject.SetActive(false);
                    loadingPanel.gameObject.SetActive(false);
                });
            return seq.Play();
        }

        #endregion

        #region QuickLoad

        public void QuickLoadScene(string sceneName, Action completeCallback = null, float delay = 0f)
        {
            DebugUtility.LogWarning($"Loading (quick) scene {sceneName}");
            onLoadingComplete = completeCallback;
            onStartLoading?.Invoke();
            DoQuickOpen(0.3f, delay).OnComplete(() =>
            {
                isQuickLoad = true;
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            });
        }

        private Tween DoQuickOpen(float duration, float delay)
        {
            loadingPanel.gameObject.SetActive(false);
            blankPanel.alpha = 0f;
            blankPanel.gameObject.SetActive(true);
            DOTween.Kill(this);
            var seq = DOTween.Sequence(this).SetUpdate(true);
            seq.AppendInterval(delay);
            seq.Append(blankPanel.DOFade(1f, duration));
            return seq;
        }

        private Tween DoQuickClose(float hideBlankDuration)
        {
            DOTween.Kill(this);
            var seq = DOTween.Sequence(this).SetUpdate(true);
            seq.Append(blankPanel.DOFade(0f, hideBlankDuration).SetEase(Ease.InQuad))
                .AppendCallback(() =>
                {
                    blankPanel.gameObject.SetActive(false);
                    loadingPanel.gameObject.SetActive(false);
                });
            return seq.Play();
        }

        #endregion
    }
}