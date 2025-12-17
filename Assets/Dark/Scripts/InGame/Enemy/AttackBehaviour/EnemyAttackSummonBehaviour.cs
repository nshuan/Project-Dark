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

        // [Tooltip("True if you want to randomize the enemy type spawned each turn")] 
        // [SerializeField] private bool randomize = true;
        
        public override void Attack(EnemyEntity enemy, TowerEntity target, Vector2 enemyPosition, int damage)
        {
            var summonIndex = RandomUtil.Range(0, enemy.config.summonIds.Count);
            if (spawnableEnemies == null) return;
            var spawnEnemyConfig =
                spawnableEnemies.FirstOrDefault(e => e.enemyId == enemy.config.summonIds[summonIndex]);
            if (!spawnEnemyConfig) return;

            var summonInfo = new SummonInfo()
            {
                enemyConfig = spawnEnemyConfig,
                amount = enemy.config.summonAmount[summonIndex]
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
                    enemy.WaveHpMultiplier, 
                    enemy.WaveDmgMultiplier, 
                    enemy.LevelExpRatio, 
                    enemy.LevelDarkRatio, 
                    enemy.LevelDarkUnitValue);
                
                creep.Activate();
                enemy.UniqueId = EnemyManager.Instance.CurrentEnemyIndex;
                EnemyManager.Instance.OnEnemySpawn(creep);
                creep.OnDead += (reason) =>
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