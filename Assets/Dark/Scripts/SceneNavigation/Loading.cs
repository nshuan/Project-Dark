using System;
using System.Collections;
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
        [SerializeField] private Image imgQuickLoadBg;
        [SerializeField] private Color quickLoadDefaultColor;

        private float normalLoadDelayClose = 0.5f;
        private float normalLoadHideBlankDuration = 0.3f;
        private float normalLoadHideDuration = 0.5f;
        
        private float quickLoadOpenBlankDuration = 0.3f;
        private float quickLoadHideBlankDuration = 0.5f;
        private float overrideQuickLoadHideDuration = -1f;
        
        public Action onStartLoading;
        public Action onSceneLoaded;
        private Action onLoadingComplete;
        private bool isQuickLoad;
        private float currentCloseDuration;

        private Coroutine coroutineOpen;
        private Coroutine coroutineClose;

        public float CurrentTotalDurationAfterSceneLoaded { get; private set; }
        public float TotalDurationAfterSceneLoaded => normalLoadDelayClose + currentCloseDuration + normalLoadHideBlankDuration + normalLoadHideDuration;
        public float TotalDurationAfterSceneQuickLoaded => quickLoadHideBlankDuration;
        
        private void OnSceneLoaded(Scene scene)
        {
            DebugUtility.LogWarning($"Scene {scene.name} is loaded!");
            if (isQuickLoad)
            {
                if (coroutineClose != null) StopCoroutine(coroutineClose);
                if (coroutineOpen != null) StopCoroutine(coroutineOpen);
                coroutineClose = StartCoroutine(IEQuickClose(0.5f,
                    overrideQuickLoadHideDuration < 0 ? quickLoadHideBlankDuration : overrideQuickLoadHideDuration,
                    () =>
                    {
                        overrideQuickLoadHideDuration = -1f;
                        onLoadingComplete?.Invoke();
                        onLoadingComplete = null;
                    }));
            }
            else
            {
                if (coroutineClose != null) StopCoroutine(coroutineClose);
                if (coroutineOpen != null) StopCoroutine(coroutineOpen);
                coroutineClose = StartCoroutine(IEClose(currentCloseDuration, normalLoadHideBlankDuration,
                    normalLoadHideDuration, () =>
                    {
                        onLoadingComplete?.Invoke();
                        onLoadingComplete = null;
                    }));
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
            if (coroutineOpen != null) StopCoroutine(coroutineOpen);
            if (coroutineClose != null) StopCoroutine(coroutineClose);
            
            // Cache duration for closing loading scene
            currentCloseDuration = RandomUtil.Range(minDuration, maxDuration);
            CurrentTotalDurationAfterSceneLoaded = TotalDurationAfterSceneLoaded;
            
            coroutineOpen = StartCoroutine(IEOpen(0.3f, delay, sceneName));
        }

        private IEnumerator IEOpen(float duration, float delay, string sceneName)
        {
            loadingPanel.alpha = 0f;
            loadingPanel.gameObject.SetActive(false);
            blankPanel.alpha = 0f;
            blankPanel.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(delay);
            yield return blankPanel.DOFade(1f, duration).WaitForCompletion();

            isQuickLoad = false;
            
            yield return new WaitForEndOfFrame();
            Scene currentScene = SceneManager.GetActiveScene();
            
            // Load blank scene additively
            AsyncOperation loadBlankOp = SceneManager.LoadSceneAsync("Blank", LoadSceneMode.Additive);
            yield return loadBlankOp;
            DebugUtility.LogWarning($"Scene Blank is loaded!");

            // Set the new scene active
            Scene loadedBlankScene = SceneManager.GetSceneByName("Blank");
            SceneManager.SetActiveScene(loadedBlankScene);

            var lastSceneName = currentScene.name;
            // Unload previous scene
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentScene);
            yield return unloadOp;
            DebugUtility.LogWarning($"Scene {lastSceneName} is unloaded!");
                
            currentScene = SceneManager.GetActiveScene();
            yield return new WaitForSecondsRealtime(0.1f);
            
            // Load additively
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            yield return loadOp;

            // Set the new scene active
            Scene loadedScene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(loadedScene);

            // Unload previous scene
            unloadOp = SceneManager.UnloadSceneAsync(currentScene);
            yield return unloadOp;
            
            yield return new WaitForEndOfFrame();
            
            OnSceneLoaded(loadedScene);
        }

        private IEnumerator IEClose(float duration, float hideBlankDuration, float hideDuration,
            Action callbackComplete = null)
        {
            loadingPanel.alpha = 1f;
            loadingPanel.gameObject.SetActive(true);
            progress.fillAmount = 0f;
            progressText.SetText($"0%");
            yield return new WaitForSecondsRealtime(0.5f);
            yield return blankPanel.DOFade(0f, hideBlankDuration).SetUpdate(true).WaitForCompletion();
            yield return DOTween.To(() => 0f, x =>
            {
                progress.fillAmount = x;
                progressText.SetText($"{(int)(x * 100)}%");
            }, 1f, duration).SetUpdate(true).WaitForCompletion();
            yield return loadingPanel.DOFade(0f, hideDuration).SetUpdate(true).WaitForCompletion();
            loadingPanel.gameObject.SetActive(false);
            blankPanel.gameObject.SetActive(false);
            callbackComplete?.Invoke(); 
        }
        
        #endregion

        #region QuickLoad

        public void OverrideQuickLoadBgColorOnce(Color color)
        {
            imgQuickLoadBg.color = color;
        }
        
        public void QuickLoadScene(string sceneName, Action completeCallback = null, float delay = 0f, float overrideOpenDuration = -1f, float overrideHideDuration = -1f)
        {
            DebugUtility.LogWarning($"Loading (quick) scene {sceneName}");
            onLoadingComplete = completeCallback;
            onStartLoading?.Invoke();
            
            if (coroutineOpen != null) StopCoroutine(coroutineOpen);
            if (coroutineClose != null) StopCoroutine(coroutineClose);

            CurrentTotalDurationAfterSceneLoaded = TotalDurationAfterSceneQuickLoaded;
            
            overrideQuickLoadHideDuration = overrideHideDuration;
            coroutineOpen = StartCoroutine(IEQuickOpen(overrideOpenDuration > 0 ? overrideOpenDuration : quickLoadOpenBlankDuration, delay, sceneName));
        }
        
        private IEnumerator IEQuickOpen(float duration, float delay, string sceneName)
        {
            loadingPanel.gameObject.SetActive(false);
            blankPanel.alpha = 0f;
            blankPanel.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(delay);
            yield return blankPanel.DOFade(1f, duration).SetUpdate(true).WaitForCompletion();
            
            isQuickLoad = true;
            
            yield return new WaitForEndOfFrame();
            Scene currentScene = SceneManager.GetActiveScene();
            
            var sceneCount = SceneManager.sceneCount;
            AsyncOperation levelMapScene = null;
            for (int i = 0; i < sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (scene.isLoaded && scene.name.ToLower().Contains("towers"))
                {
                    levelMapScene = SceneManager.UnloadSceneAsync(scene);
                    break;
                }
            }
            if (levelMapScene != null) yield return levelMapScene;

            // Load blank scene additively
            AsyncOperation loadBlankOp = SceneManager.LoadSceneAsync("Blank", LoadSceneMode.Additive);
            yield return loadBlankOp;
            DebugUtility.LogWarning($"Scene Blank is loaded!");

            // Set the new scene active
            Scene loadedBlankScene = SceneManager.GetSceneByName("Blank");
            SceneManager.SetActiveScene(loadedBlankScene);

            var lastSceneName = currentScene.name;
            // Unload previous scene
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentScene);
            yield return unloadOp;
            DebugUtility.LogWarning($"Scene {lastSceneName} is unloaded!");
                
            currentScene = SceneManager.GetActiveScene();
            yield return new WaitForSecondsRealtime(0.1f);
            
            // Load additively
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            yield return loadOp;

            // Set the new scene active
            Scene loadedScene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(loadedScene);

            // Unload previous scene
            AsyncOperation unloadBlankOp = SceneManager.UnloadSceneAsync(currentScene);
            yield return unloadBlankOp;
            
            yield return new WaitForEndOfFrame();
            OnSceneLoaded(loadedScene);
        }
        
        private IEnumerator IEQuickClose(float delay, float hideBlankDuration, Action callbackComplete = null)
        {
            yield return new WaitForSeconds(delay);
            yield return blankPanel.DOFade(0f, hideBlankDuration).SetUpdate(true).WaitForCompletion();
            blankPanel.gameObject.SetActive(false);
            loadingPanel.gameObject.SetActive(false);
            imgQuickLoadBg.color = quickLoadDefaultColor;
            callbackComplete?.Invoke();
        }

        #endregion
    }
}