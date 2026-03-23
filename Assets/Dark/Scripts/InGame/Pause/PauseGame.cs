using System;
using Core;
using UnityEngine;

namespace InGame.Pause
{
    public class PauseGame : Singleton<PauseGame>
    {
        public Action<bool> onPause;
        
        public bool IsPaused { get; private set; }
        public bool BlockResume { get; set; }
        
        public bool Pause()
        {
            Time.timeScale = 0f;
            IsPaused = true;
            onPause?.Invoke(true);
            DebugUtility.LogError("[InGame] Paused");
            return true;
        }

        public bool Resume()
        {
            if (BlockResume) return false;
            Time.timeScale = 1f;
            IsPaused = false;
            onPause?.Invoke(false);
            DebugUtility.LogError("[InGame] Resumed");
            return true;
        }
    }
}