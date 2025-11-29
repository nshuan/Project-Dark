using System;
using Dark.Scripts.SceneNavigation;
using InGame.Pause;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonBackToUpgrade : MonoBehaviour
    {
        private Button button;

        private float delayEnableButton = 2f;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void Start()
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                if (delayEnableButton > 0f) return;
                // Loading.Instance.QuickLoadScene(SceneConstants.SceneUpgrade);
                PauseGame.Instance.Pause();
            });
        }

        private void Update()
        {
            if (delayEnableButton > 0f) delayEnableButton -= Time.deltaTime;
        }
    }
}