using System;
using Dark.Scripts.Utils;
using UnityEngine;

namespace InGame.Pause
{
    public class UIPauseGame : MonoBehaviour
    {
        private bool blockPause;
        [SerializeField] private float pauseCooldownDuration = 1f;

        private float pauseCooldown;

        private void Awake()
        {
            LevelManager.Instance.OnLevelLoaded += (level) =>
            {
                LevelManager.Instance.OnLose += OnLose;
                LevelManager.Instance.OnWin += OnWin;
            };

            pauseCooldown = pauseCooldownDuration;
        }

        private void Update()
        {
            if (!blockPause)
            {
                if (pauseCooldown > 0)
                {
                    pauseCooldown -= Time.unscaledDeltaTime;
                }
                else
                {
                    if (Input.GetKey(KeyCode.Escape))
                    {
                        if (PauseGame.Instance.IsPaused) PauseGame.Instance.Resume();
                        else PauseGame.Instance.Pause();
                        pauseCooldown = pauseCooldownDuration;
                    }
                }
            }
        }

        private void OnLose()
        {
            blockPause = true;
        }

        private void OnWin()
        {
            blockPause = true;
        }
    }
}