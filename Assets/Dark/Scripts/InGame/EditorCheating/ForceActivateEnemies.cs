using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame.EditorCheating
{
    public class ForceActivateEnemies : MonoBehaviour
    {
        [SerializeField] private EnemyBehaviour[] enemyConfigs;
        [SerializeField] private TowerEntity[] towers;
        private EnemyEntity[] enemies;

        [Space] 
        [SerializeField] private bool enableRandomTower;

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
                        int targetTowerIndex = Random.Range(0, towers.Length);
                        if (!enableRandomTower)
                        {
                            var nearestDistance = Vector2.Distance(towers[targetTowerIndex].transform.position,
                                enemy.transform.position);
                            for (var i = 0; i < towers.Length; i++)
                            {
                                if (Vector2.Distance(towers[i].transform.position, enemy.transform.position) <
                                    nearestDistance)
                                {
                                    targetTowerIndex = i;
                                    nearestDistance = Vector2.Distance(towers[i].transform.position,
                                        enemy.transform.position);
                                }
                            }
                        }
                        
                        enemy.Init(config, towers[targetTowerIndex], 1f, 1f, 1f,1f);
                        enemy.Activate();
                    }
                }
            }
        }
    }
}