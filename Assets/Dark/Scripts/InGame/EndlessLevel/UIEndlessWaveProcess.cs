using System;
using Dark.Tools.Language.Runtime;
using TMPro;
using UnityEngine;

namespace InGame.EndlessLevel
{
    public class UIEndlessWaveProcess : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtWave;

        private void Awake()
        {
            LevelManager.Instance.OnLevelLoaded += OnLevelLoaded;
        }

        private void Start()
        {
            LevelEndlessManager.OnStartWave += OnStartNewWave;
        }

        private void OnDestroy()
        {
            LevelEndlessManager.OnStartWave -= OnStartNewWave;
        }

        private void OnLevelLoaded(LevelConfig level)
        {
            if (!LevelManager.Instance.IsPlayingEndless)
                gameObject.SetActive(false);
        }
        
        private void OnStartNewWave(int wave)
        {
            txtWave.SetTextLanguage("key_wave", ("%{value}", wave.ToString()));    
        }
    }
}