using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace InGame.SpawnBehaviour
{
    [CreateAssetMenu(menuName = "InGame/Enemy/Spawn/Enemy Spawn Necromancer", fileName = "EnemySpawnNecromancer")]
    public class EnemyNecromancerSpawnBehaviour : EnemySpawnBehaviour
    {
        [SerializeField] private EnemyAttackSummonBehaviour summonBehaviour;
        [SerializeField] private float firstCreepSpawnDuration = 0.5f;
        
        public override void Init(EnemyEntity enemy)
        {
            
        }

        public override Tween DoSpawn(EnemyEntity enemy)
        {
            if (enemy.config is EnemySummonBehaviour enemyConfig)
            {
                var spawnTime = enemy.animController.PlaySpawn();
                var seq = DOTween.Sequence().AppendInterval(spawnTime);

                if (enemyConfig.listSummonIdsOnSpawned == null || enemyConfig.listSummonAmountOnSpawned == null ||
                    enemyConfig.listSummonIdsOnSpawned.Count == 0 || enemyConfig.listSummonAmountOnSpawned.Count == 0) return seq;
                
                var summonIndex = RandomUtil.Range(0, enemyConfig.listSummonAmountOnSpawned.Count);
                if (summonIndex >= 0 && summonIndex < enemyConfig.listSummonAmountOnSpawned.Count && enemyConfig.listSummonAmountOnSpawned[summonIndex] > 0)
                {
                    seq.AppendCallback(() =>
                        {
                            enemy.animController.PlayAttack();
                            
                        })
                        .AppendInterval(0.5f)
                        .AppendCallback(() =>
                        {
                            summonBehaviour.Summon(enemy, enemy.TargetTower, enemyConfig.listSummonIdsOnSpawned[summonIndex],
                                enemyConfig.listSummonAmountOnSpawned[summonIndex]);
                        })
                        .AppendInterval(firstCreepSpawnDuration - 0.5f);
                }
                
                return seq;
            }

            // Spawn thường
            enemy.transform.localScale = 0.3f * Vector3.one;
            return enemy.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }
    }
}