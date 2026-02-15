using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Enemy/Attack/Enemy Attack Summon", fileName = "EnemyAttackSummon")]
    public class EnemyAttackSummonBehaviour : EnemyAttackBehaviour
    {
        [SerializeField] private List<EnemyBehaviour> spawnableEnemies;
        [SerializeField] private float summonSpanAngle = 80f;
        [SerializeField] private float creepDelayAttack = 0.5f;

        public List<EnemyBehaviour> SpawnableEnemies => spawnableEnemies;
        
        // [Tooltip("True if you want to randomize the enemy type spawned each turn")] 
        // [SerializeField] private bool randomize = true;
        
        public override void Attack(EnemyEntity enemy, TowerEntity target, Vector2 enemyPosition, int damage)
        {
            if (enemy.config is EnemySummonBehaviour enemyConfig)
            {
                if (enemyConfig.summonIds == null || enemyConfig.summonAmount == null || enemyConfig.summonIds.Count <= 0 || enemyConfig.summonAmount.Count <= 0) return;
                var summonIndex = RandomUtil.Range(0, enemyConfig.summonIds.Count);
                if (spawnableEnemies == null) return;

                Summon(enemy, target, enemyConfig.summonIds[summonIndex], enemyConfig.summonAmount[summonIndex]);
            }
        }

        public void Summon(EnemyEntity enemy, TowerEntity target, int enemyId, int amount)
        {
            var spawnEnemyConfig =
                spawnableEnemies.FirstOrDefault(e => e.enemyId == enemyId);
            if (!spawnEnemyConfig) return;
            
            var summonInfo = new SummonInfo()
            {
                enemyConfig = spawnEnemyConfig,
                amount = amount
            };
            
            for (var i = 0; i < summonInfo.amount; i++)
            {
                var direction = (target.transform.position - enemy.transform.position).normalized;
                var creep = EnemyPool.Instance.Get(summonInfo.enemyConfig.enemyPrefab, summonInfo.enemyConfig.enemyId, null, false);
                creep.transform.position = enemy.transform.position + direction * 0.2f +
                                           RandomUtil.InsideUnitSpan(
                                               direction, summonSpanAngle);
                creep.Init(
                    summonInfo.enemyConfig, 
                    target, 
                    enemy.StatsScale,
                    enemy.LevelExpRatio, 
                    enemy.LevelDarkRatio, 
                    enemy.LevelDarkUnitValue);
                
                creep.Activate(creepDelayAttack);
                enemy.UniqueId = EnemyManager.Instance.CurrentEnemyIndex;
                EnemyManager.Instance.OnEnemySpawn(creep);
                creep.OnDead += (dead, reason) =>
                {
                    EnemyManager.Instance.OnEnemyDead(creep, reason);
                };
            }
        }
        
        [Serializable]
        public class SummonInfo
        {
            public EnemyBehaviour enemyConfig;
            public int amount;
        }
    }
}