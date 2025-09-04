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
        [SerializeField] private GameObject imgBlockRaycast;
        [SerializeField] private float delayShowPopup = 5f; // Do có vfx endgame khi trụ bị phá nên cần delay xong vfx mới show popup
        
        [Space]
        [SerializeField] private Button btnBackToTree;
        [SerializeField] private Button btnReplay;

        private void Start()
        {
            LevelManager.Instance.OnLose += OnLose;
        }

        private void OnLose()
        {
            UpdateUI();
            imgBlockRaycast.SetActive(true);
            ui.DoOpen().SetDelay(delayShowPopup).Play();
        }

        private void UpdateUI()
        {
            btnBackToTree.onClick.RemoveAllListeners();
            btnBackToTree.onClick.AddListener(() =>
            {
                Loading.Instance.LoadScene(SceneConstants.SceneUpgrade);
            });
            
            // Todo reload level
            btnReplay.onClick.RemoveAllListeners();
            btnReplay.onClick.AddListener(() =>
            {
                imgBlockRaycast.SetActive(false);
                ui.gameObject.SetActive(false);
                Loading.Instance.LoadScene(SceneConstants.SceneInGame, () =>
                {
                    LevelManager.Instance.LoadLevel(1);
                });
            });
        }
    }
}