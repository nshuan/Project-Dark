using System;
using Core;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dark.Scripts.SceneNavigation
{
    public class Loading : MonoSingleton<Loading>
    {
        [SerializeField] private CanvasGroup loadingPanel;
        [SerializeField] private Image progress;
        [SerializeField] private TextMeshProUGUI progressText;

        private AsyncOperation cacheAsync;
        private string preloadedSceneName;
        public Action onStartLoading;
        private Action onSceneLoaded;
        
        protected override void Awake()
        {
            base.Awake();
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void LoadSceneWithoutActivation(string sceneName)
        {
            cacheAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            if (cacheAsync != null)
            {
                preloadedSceneName = sceneName;
                cacheAsync.allowSceneActivation = false;
            }
        }

        private void ActivateCacheScene(Action completeCallback = null)
        {
            if (cacheAsync == null) return;
            DebugUtility.LogWarning($"Activating scene {preloadedSceneName}");
            onSceneLoaded = completeCallback;
            onStartLoading?.Invoke();
            DoOpen(0.2f).OnComplete(() =>
            {
                cacheAsync.allowSceneActivation = true;
            });
        }
        
        public void LoadScene(string sceneName, Action completeCallback = null)
        {
            if (preloadedSceneName == sceneName)
            {
                ActivateCacheScene(completeCallback);
                return;
            }
            
            DebugUtility.LogWarning($"Loading scene {sceneName}");
            onSceneLoaded = completeCallback;
            onStartLoading?.Invoke();
            DoOpen(0.2f).OnComplete(() =>
            {
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            });
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            DebugUtility.LogWarning($"Scene {scene.name} is loaded!");
            DoClose(0.2f);
            onSceneLoaded?.Invoke();
            onSceneLoaded = null;
        }

        private Tween DoOpen(float duration)
        {
            loadingPanel.alpha = 0f;
            progress.fillAmount = 0f;
            progressText.SetText($"0%");
            loadingPanel.gameObject.SetActive(true);
            return loadingPanel.DOFade(1f, duration);
        }

        private Tween DoClose(float duration)
        {
            DOTween.Kill(this);
            var seq = DOTween.Sequence(this);
            seq.Append(DOTween.To(() => 0f, x =>
                {
                    progress.fillAmount = x;
                    progressText.SetText($"{(int)(x * 100)}%");
                }, 1f, duration))
                .Append(loadingPanel.DOFade(0f, duration))
                .OnComplete(() => loadingPanel.gameObject.SetActive(false));
            return seq.Play();
        }
    }
}