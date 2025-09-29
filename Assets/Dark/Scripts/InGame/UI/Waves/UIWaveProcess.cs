using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace InGame.UI.Waves
{
    public class UIWaveProcess : MonoBehaviour
    {
        [SerializeField] private UIWaveProcessItem[] waveItems;
        [SerializeField] private Vector2 anchor;
        [SerializeField] private float spacing;

        private int totalWave = 10;
        
        private void Awake()
        {
            LevelManager.Instance.OnLevelLoaded += OnLevelLoaded;
            LevelManager.Instance.OnWaveStart += OnWaveStart;
        }
        
        private void OnLevelLoaded(LevelConfig level)
        {
            totalWave = level.waveInfo.Length;
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
        [Button]
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

            var currentPos = anchor;
            for (var i = 0; i < Math.Min(waveItems.Length, totalWave); i++)
            {
                waveItems[i].transform.localPosition = currentPos;
                if (i < waveItems.Length - 1)
                {
                    currentPos.y -= (waveItems[i].rectTransform.sizeDelta.y / 2 + spacing + waveItems[i + 1].rectTransform.sizeDelta.y / 2);
                }
            }
        }

        [Button]
        public void Validate()
        {
            var currentPos = anchor;
            for (var i = 0; i < waveItems.Length; i++)
            {
                waveItems[i].transform.localPosition = currentPos;
                if (i < waveItems.Length - 1)
                {
                    currentPos.y -= (waveItems[i].rectTransform.sizeDelta.y / 2 + spacing + waveItems[i + 1].rectTransform.sizeDelta.y / 2);
                }
            }
        }
    }
}