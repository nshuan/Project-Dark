using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame.EditorCheating
{
    public class ForceActivateEnemies : MonoBehaviour
    {
        [SerializeField] private EnemyBehaviour[] enemyConfigs;
        [SerializeField] private TowerEntity[] towers;
        private EnemyEntity[] enemies;

        private void Awake()
        {
            enemies = GetComponentsInChildren<EnemyEntity>();
        }

        private void Start()
        {
            foreach (var enemy in enemies)
            {
                foreach (var config in enemyConfigs)
                {
                    if (enemy.name.Contains(config.name))
                    {
                        enemy.Init(config, towers[Random.Range(0, towers.Length)], 1f, 1f, 1f,1f);
                        enemy.Activate();
                    }
                }
            }
        }
    }
}