using System;
using Dark.Scripts.CoreUI;
using Dark.Scripts.SceneNavigation;
using DG.Tweening;
using Economic;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI
{
    public class PopupWin : MonoBehaviour
    {
        [SerializeField] private UIPopup ui;
        [SerializeField] private GameObject imgBlockRaycast;
        
        [Space]
        [SerializeField] private Button btnBackToTree;
        [SerializeField] private Button btnNextLevel;

        public static event Action onShowPopup;
        
        private void Start()
        {
            LevelManager.Instance.OnWin += OnWin;
        }

        private void OnDestroy()
        {
            onShowPopup = null;
        }

        private void OnWin()
        {
            UpdateUI();
            imgBlockRaycast.SetActive(true);
            ui.DoOpen().OnComplete(() => onShowPopup?.Invoke());
        }

        private void UpdateUI()
        {
            btnBackToTree.onClick.RemoveAllListeners();
            btnBackToTree.onClick.AddListener(() =>
            {
                Loading.Instance.LoadScene(SceneConstants.SceneUpgrade);
            });
            
            // Todo load next level
            btnNextLevel.onClick.RemoveAllListeners();
            btnNextLevel.onClick.AddListener(() =>
            {
                imgBlockRaycast.SetActive(false);
                ui.gameObject.SetActive(false);
                Loading.Instance.LoadScene(SceneConstants.SceneInGame, () =>
                {
                    LevelManager.Instance.LoadLevel(LevelManager.Instance.Level.level % 2 + 1);
                });
            });
        }
    }
}