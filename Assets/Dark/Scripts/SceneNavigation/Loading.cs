using System;
using Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dark.Scripts.SceneNavigation
{
    public class Loading : MonoSingleton<Loading>
    {
        [SerializeField] private GameObject loadingPanel;

        private Action onSceneLoaded;
        
        protected override void Awake()
        {
            base.Awake();
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void LoadScene(string sceneName, Action completeCallback = null)
        {
            if (SceneManager.GetSceneByName(sceneName).IsValid()) return;
            onSceneLoaded = completeCallback;
            loadingPanel.SetActive(true);
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            loadingPanel.SetActive(false);
            onSceneLoaded?.Invoke();
            onSceneLoaded = null;
        }
    }
}