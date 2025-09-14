using System;
using Dark.Scripts.CoreUI;
using Dark.Scripts.SceneNavigation;
using DG.Tweening;
using Economic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace InGame.UI
{
    public class PopupLose : MonoBehaviour
    {
        [SerializeField] private UIPopup ui;
        [SerializeField] private CanvasGroup imgBlockRaycast;
        [SerializeField] private float delayShowPopup = 5f; // Do có vfx endgame khi trụ bị phá nên cần delay xong vfx mới show popup
        
        [Space]
        [SerializeField] private Button btnBackToTree;
        [SerializeField] private Button btnReplay;

        public static event Action onShowPopup;
        
        private void Start()
        {
            LevelManager.Instance.OnLose += OnLose;
        }

        private void OnDestroy()
        {
            onShowPopup = null;
        }

        private void OnLose()
        {
            UpdateUI();
            DOVirtual.DelayedCall(delayShowPopup, () =>
            {
                imgBlockRaycast.alpha = 0f;
                imgBlockRaycast.gameObject.SetActive(true);
                imgBlockRaycast.DOFade(1f, 0.2f);
            });
            ui.DoOpen().SetDelay(delayShowPopup).OnComplete(() => onShowPopup?.Invoke());
        }

        private void UpdateUI()
        {
            btnBackToTree.onClick.RemoveAllListeners();
            btnBackToTree.onClick.AddListener(() =>
            {
                btnReplay.interactable = false;
                btnBackToTree.interactable = false;
                Loading.Instance.LoadScene(SceneConstants.SceneUpgrade);
            });
            
            // Todo reload level
            btnReplay.onClick.RemoveAllListeners();
            btnReplay.onClick.AddListener(() =>
            {
                btnReplay.interactable = false;
                btnBackToTree.interactable = false;
                Loading.Instance.LoadScene(SceneConstants.SceneInGame, () =>
                {
                    LevelManager.Instance.LoadLevel(1);
                });
            });
        }
    }
}