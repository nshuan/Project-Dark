using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI.EnemyInfo
{
    public class UILevelEnemies : MonoBehaviour
    {
        [SerializeField] private Button btnOpenPanel;
        [SerializeField] private GameObject panelEnemyInfo;
        [SerializeField] private UIWaveInfo currentWave;
        [SerializeField] private UIWaveInfo nextWave;

        private void Awake()
        {
            btnOpenPanel.onClick.RemoveAllListeners();
            btnOpenPanel.onClick.AddListener(() =>
            {
                if (panelEnemyInfo.activeSelf) panelEnemyInfo.SetActive(false);
                else panelEnemyInfo.SetActive(true);
            });
        }

        private void Start()
        {
            LevelManager.Instance.OnWaveStart += OnWaveStarted;
        }

        private void OnWaveStarted(int waveIndex, float timeToEnd)
        {
            if (waveIndex < LevelManager.Instance.Level.waveInfo.Length)
                currentWave.UpdateUI(LevelManager.Instance.Level.waveInfo[waveIndex]);
            if (waveIndex < LevelManager.Instance.Level.waveInfo.Length - 1)
            {
                nextWave.UpdateUI(LevelManager.Instance.Level.waveInfo[waveIndex + 1]);
                nextWave.gameObject.SetActive(true);
            }
            else
            {
                nextWave.gameObject.SetActive(false);
            }
        }
    }
}