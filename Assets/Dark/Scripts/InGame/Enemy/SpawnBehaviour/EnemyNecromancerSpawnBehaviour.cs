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
            var spawnTime = enemy.animController.PlaySpawn();
            var seq = DOTween.Sequence().AppendInterval(spawnTime);

            if (enemy.config.listSummonIdsOnSpawned == null || enemy.config.listSummonAmountOnSpawned == null ||
                enemy.config.listSummonIdsOnSpawned.Count == 0 || enemy.config.listSummonAmountOnSpawned.Count == 0) return seq;
            
            var summonIndex = RandomUtil.Range(0, enemy.config.summonIds.Count);
            if (summonIndex >= 0 && summonIndex < enemy.config.listSummonAmountOnSpawned.Count && enemy.config.listSummonAmountOnSpawned[summonIndex] > 0)
            {
                seq.AppendCallback(() =>
                    {
                        enemy.animController.PlayAttack();
                        
                    })
                    .AppendInterval(0.5f)
                    .AppendCallback(() =>
                    {
                        summonBehaviour.Summon(enemy, enemy.TargetTower, enemy.config.listSummonIdsOnSpawned[summonIndex],
                            enemy.config.listSummonAmountOnSpawned[summonIndex]);
                    })
                    .AppendInterval(firstCreepSpawnDuration - 0.5f);
            }
            
            return seq;
        }
    }
}