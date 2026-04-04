using System;
using Dark.Tools.Language.Runtime;
using TMPro;
using UnityEngine;

namespace InGame.EndlessLevel
{
    public class UIEndlessWaveProcess : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtWave;
        [SerializeField] private GameObject content;

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
            content.SetActive(LevelManager.Instance.IsPlayingEndless);
        }
        
        private void OnStartNewWave(int wave)
        {
            txtWave.SetTextLanguage("key_wave", ("%{value}", wave.ToString()));    
        }
    }
}