using Dark.Scripts.Analytics;
using Dark.Scripts.CoreUI;
using Dark.Scripts.OutGame.Settings;
using Dark.Scripts.SceneNavigation;
using Data;
using InGame.Pause;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI
{
    public class PopupConfirmReturn : MonoBehaviour
    {
        [SerializeField] private UIPopup ui;
        
        [Space]
        [SerializeField] private Button btnBackToTree;
        [SerializeField] private Button btnResume;
        [SerializeField] private Button btnOptions;
        [SerializeField] private UIPopupSettings panelOptions;

        private void Start()
        {
            PauseGame.Instance.onPause += OnPauseGame;
            LevelManager.Instance.OnWin += OnLevelCompleted;
            LevelManager.Instance.OnLose += OnLevelCompleted;
            panelOptions.OnCloseComplete += () => PauseGame.Instance.BlockResume = false;
        }

        private void OnPauseGame(bool isPaused)
        {
            if (IgnorePause) return;
            if (isPaused)
            {
                UpdateUI();
                ui.DoOpenFadeIn();
            }
            else
            {
                ui.DoCloseFadeOut();
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
                PauseGame.Instance.onPause = null;
                PauseGame.Instance.Resume();
                Loading.Instance.QuickLoadScene(SceneConstants.SceneUpgrade);
                LogManager.Log(LogConst.EventLogQuitLevel, $"level_{PlayerDataManager.Instance.Data.level + 1}", $"wave_{LevelManager.Instance.CurrentWaveIndex + 1}");
            });
            
            btnResume.onClick.RemoveAllListeners();
            btnResume.onClick.AddListener(() =>
            {
                PauseGame.Instance.Resume();
            });
            
            btnOptions.onClick.RemoveAllListeners();
            btnOptions.onClick.AddListener(() =>
            {
                PauseGame.Instance.BlockResume = true;
                panelOptions.gameObject.SetActive(true);
            });
        }
        
        public static bool IgnorePause { get; set; }
    }
}