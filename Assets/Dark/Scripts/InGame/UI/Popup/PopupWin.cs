using System;
using Dark.Scripts.CoreUI;
using Dark.Scripts.SceneNavigation;
using Data;
using DG.Tweening;
using Economic;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI
{
    public class PopupWin : MonoBehaviour
    {
        [SerializeField] private UIPopup ui;
        
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
            ui.DoOpenFadeIn().OnComplete(() => onShowPopup?.Invoke());
        }

        private void UpdateUI()
        {
            btnBackToTree.onClick.RemoveAllListeners();
            btnBackToTree.onClick.AddListener(() =>
            {
                Loading.Instance.QuickLoadScene(SceneConstants.SceneUpgrade);
            });
            
            // Todo load next level
            btnNextLevel.onClick.RemoveAllListeners();
            btnNextLevel.onClick.AddListener(() =>
            {
                ui.gameObject.SetActive(false);
                Loading.Instance.QuickLoadScene(SceneConstants.SceneInGame, () =>
                {
                    LevelManager.Instance.LoadLevel(PlayerDataManager.Instance.Data.level + 1);
                });
            });
        }
    }
}