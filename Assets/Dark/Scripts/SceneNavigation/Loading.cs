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
        [SerializeField] private Image progress;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private float minDuration = 0.5f;
        [SerializeField] private float maxDuration = 1.5f;
        
        public Action onStartLoading;
        private Action onSceneLoaded;
        
        protected override void Awake()
        {
            base.Awake();
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        public void LoadScene(string sceneName, Action completeCallback = null)
        {
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
            DoClose(Random.Range(minDuration, maxDuration), 0.2f);
            onSceneLoaded?.Invoke();
            onSceneLoaded = null;
        }

        private Tween DoOpen(float duration)
        {
            loadingPanel.alpha = 0f;
            progress.fillAmount = 0f;
            progressText.SetText($"0%");
            loadingPanel.gameObject.SetActive(true);
            return loadingPanel.DOFade(1f, duration).SetUpdate(true);
        }

        private Tween DoClose(float duration, float hideDuration)
        {
            DOTween.Kill(this);
            var seq = DOTween.Sequence(this).SetUpdate(true);
            seq.Append(DOTween.To(() => 0f, x =>
                {
                    progress.fillAmount = x;
                    progressText.SetText($"{(int)(x * 100)}%");
                }, 1f, duration))
                .Append(loadingPanel.DOFade(0f, hideDuration))
                .OnComplete(() => loadingPanel.gameObject.SetActive(false));
            return seq.Play();
        }
    }
}