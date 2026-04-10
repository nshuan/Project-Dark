using System;
using Dark.Scripts.SceneNavigation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace InGame.GateEditorV2
{
    public class LevelEditorButtonBackToEditor : MonoBehaviour, IPointerClickHandler
    {
        public GameObject btnVisual;
        public string sceneName;
        
        private bool interactable = false;
        
        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Start()
        {
            if (SceneManager.GetActiveScene().name == sceneName)
            {
                btnVisual.SetActive(false);
                interactable = false;
            }
            else 
            {
                btnVisual.SetActive(true);
                interactable = true;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            if (scene.name == sceneName)
            {
                btnVisual.SetActive(false);
                interactable = false;
            }
            else 
            {
                btnVisual.SetActive(true);
                interactable = true;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!interactable) return;
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}