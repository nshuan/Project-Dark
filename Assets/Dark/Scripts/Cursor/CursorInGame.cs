using System;
using InGame.Pause;
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
        }

        private void OnPause(bool pause)
        {
            if (pause)
                CursorManager.Instance.SetCursorOutGame();
            else
                CursorManager.Instance.SetCursorInGame();
        }
    }
}