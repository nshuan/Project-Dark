using Dark.Scripts.CoreUI;
using Dark.Scripts.SceneNavigation;
using DG.Tweening;
using InGame.Pause;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace InGame.UI
{
    public class PopupPause : MonoBehaviour
    {
        [SerializeField] private UIPopup ui;
        [SerializeField] private GameObject imgBlockRaycast;
        
        [Space]
        [SerializeField] private Button btnBackToTree;
        [SerializeField] private Button btnResume;

        private void Start()
        {
            PauseGame.Instance.onPause += OnPauseGame;
            LevelManager.Instance.OnWin += OnLevelCompleted;
            LevelManager.Instance.OnLose += OnLevelCompleted;
        }

        private void OnPauseGame(bool isPaused)
        {
            if (isPaused)
            {
                UpdateUI();
                imgBlockRaycast.SetActive(true);
                ui.DoOpen();
            }
            else
            {
                imgBlockRaycast.SetActive(false);
                ui.DoClose();
            }
        }

        private void OnLevelCompleted()
        {
            PauseGame.Instance.onPause -= OnPauseGame;
        }

        private void UpdateUI()
        {
            btnBackToTree.onClick.RemoveAllListeners();
            btnBackToTree.onClick.AddListener(() =>
            {
                PauseGame.Instance.Resume();
                Loading.Instance.LoadScene(SceneConstants.SceneUpgrade);
            });
            
            // Todo reload level
            btnResume.onClick.RemoveAllListeners();
            btnResume.onClick.AddListener(() =>
            {
                PauseGame.Instance.Resume();
            });
        }
    }
}