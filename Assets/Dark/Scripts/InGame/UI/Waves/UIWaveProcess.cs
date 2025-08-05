using System;
using UnityEngine;

namespace InGame.UI.Waves
{
    public class UIWaveProcess : MonoBehaviour
    {
        [SerializeField] private UIWaveProcessItem[] waveItems;

        private int totalWave;
        
        private void Awake()
        {
            LevelManager.Instance.OnLevelLoaded += OnLevelLoaded;
            LevelManager.Instance.OnWaveStart += OnWaveStart;
        }
        
        private void OnLevelLoaded(LevelConfig level)
        {
            totalWave = level.waveInfos.Length;
            foreach (var waveItem in waveItems)
            {
                waveItem.UpdateUI(0);
            }
        }

        private void OnWaveStart(int waveIndex)
        {
            UpdateUI(waveIndex);
        }

        // 0 - 1 - 2 - 3 - 4
        // Current state = 3
        public void UpdateUI(int currentWave)
        {
            for (var i = 0; i < waveItems.Length; i++)
            {
                if (i < totalWave)
                {
                    waveItems[i].UpdateUI(i - currentWave + 3);
                    waveItems[i].gameObject.SetActive(true);
                }
                else
                {
                    waveItems[i].gameObject.SetActive(false);
                }
            }
        }
    }
}