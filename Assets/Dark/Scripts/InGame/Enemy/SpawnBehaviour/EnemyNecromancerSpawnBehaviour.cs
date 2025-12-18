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

            if (enemy.config.summonAmountOnSpawned > 0)
            {
                seq.AppendCallback(() =>
                    {
                        enemy.animController.PlayAttack();
                        
                    })
                    .AppendInterval(0.5f)
                    .AppendCallback(() =>
                    {
                        summonBehaviour.Summon(enemy, enemy.TargetTower, enemy.config.summonIdOnSpawned,
                            enemy.config.summonAmountOnSpawned);
                    })
                    .AppendInterval(firstCreepSpawnDuration - 0.5f);
            }
            
            return seq;
        }
    }
}