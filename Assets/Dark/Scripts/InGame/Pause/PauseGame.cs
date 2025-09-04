using System;
using Core;
using UnityEngine;

namespace InGame.Pause
{
    public class PauseGame : Singleton<PauseGame>
    {
        public Action<bool> onPause;
        
        public bool IsPaused { get; private set; }
        
        public void Pause()
        {
            Time.timeScale = 0f;
            IsPaused = true;
            onPause?.Invoke(true);
        }

        public void Resume()
        {
            Time.timeScale = 1f;
            IsPaused = false;
            onPause?.Invoke(false);
        }
    }
}