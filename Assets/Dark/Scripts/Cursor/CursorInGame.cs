using System;
using InGame.Pause;
using InGame.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dark.Scripts.Cursor
{
    public class CursorInGame : MonoBehaviour
    {
        private void Start()
        {
            if (SceneManager.GetActiveScene().name != "BaseLevel") return;

            PauseGame.Instance.onPause += OnPause;
            PopupWin.onShowPopup += OnShowPopupEndGame;
            PopupLose.onShowPopup += OnShowPopupEndGame;
        }

        private void OnPause(bool pause)
        {
            if (pause)
                CursorManager.Instance.SetCursorOutGame();
            else
                CursorManager.Instance.SetCursorInGame();
        }

        private void OnShowPopupEndGame()
        {
            CursorManager.Instance.SetCursorOutGame();
        }
    }
}