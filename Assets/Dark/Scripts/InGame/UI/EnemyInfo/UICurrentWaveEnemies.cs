using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI.EnemyInfo
{
    public class UICurrentWaveEnemies : MonoBehaviour
    {
        [SerializeField] private Button btnOpenPanel;
        [SerializeField] private GameObject panelEnemyInfo;
        [SerializeField] private Transform parentEnemies;
        [SerializeField] private UIEnemyInfo prefabEnemyInfo;

        private List<UIEnemyInfo> enemyInfos;
        private int activeEnemyInfo;

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
            CombatActions.OnOneEnemySpawn += OnEnemySpawn;
        }

        private void OnWaveStarted(int waveIndex, float timeToEnd)
        {
  
        }
        
        private void OnEnemySpawn(EnemyEntity enemy)
        {
            enemyInfos ??= new List<UIEnemyInfo>();

            for (var i = 0; i <= activeEnemyInfo; i++)
            {
                if (i >= enemyInfos.Count) break;
                if (enemyInfos[i].EnemyId == enemy.config.enemyId) return;
            }
            
            if (activeEnemyInfo >= enemyInfos.Count)
            {
                var newEnemyInfo = Instantiate(prefabEnemyInfo, parentEnemies);
                newEnemyInfo.transform.SetAsLastSibling();
                newEnemyInfo.UpdateUI(enemy);
                enemyInfos.Add(newEnemyInfo);
                newEnemyInfo.gameObject.SetActive(true);
            }
            if (activeEnemyInfo < enemyInfos.Count)
            {
                enemyInfos[activeEnemyInfo].UpdateUI(enemy);
                enemyInfos[activeEnemyInfo].gameObject.SetActive(true);
            }

            activeEnemyInfo += 1;
        }
    }
}