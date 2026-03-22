using System.Linq;
using DG.Tweening;
using InGame.SpawnBehaviour;
using UnityEngine;

namespace InGame.Boss.BossWizard
{
    [CreateAssetMenu(menuName = "InGame/Enemy/Spawn/Enemy Spawn Wizard", fileName = "EnemySpawnWizard")]
    public class BossWizardSpawnBehaviour : EnemyNecromancerSpawnBehaviour
    {
        public override Tween DoSpawn(EnemyEntity enemy)
        {
            var spawnTime = enemy.animController.PlaySpawn();
            var seq = DOTween.Sequence().AppendInterval(spawnTime).SetTarget(enemy);
            
            if (enemy is BossWizardEntity boss && boss.configCasted.wizardConfig.summonOnSpawn && enemy.config is EnemySummonBehaviour enemyConfig)
            {
                if (enemyConfig.listSummonIdsOnSpawned == null || enemyConfig.listSummonAmountOnSpawned == null ||
                    enemyConfig.listSummonIdsOnSpawned.Count == 0 || enemyConfig.listSummonAmountOnSpawned.Count == 0) return seq;
                
                var summonIndex = RandomUtil.Range(0, enemyConfig.listSummonAmountOnSpawned.Count);
                if (summonIndex >= 0 && summonIndex < enemyConfig.listSummonAmountOnSpawned.Count && enemyConfig.listSummonAmountOnSpawned[summonIndex] > 0)
                {
                    seq.AppendCallback(() =>
                        {
                            enemy.animController.PlayCustomAnim(boss.summonAnimInfo);
                        })
                        .AppendInterval(0.5f)
                        .AppendCallback(() =>
                        {
                            var randomTargetForCreeps =
                                RandomUtil.ShuffleIndex(0, LevelManager.Instance.Towers.Length - 1)
                                    .Select((towerIndex) => LevelManager.Instance.Towers[towerIndex]).ToArray();
                            summonBehaviour.Summon(enemy, randomTargetForCreeps,
                                enemyConfig.listSummonIdsOnSpawned[summonIndex],
                                enemyConfig.listSummonAmountOnSpawned[summonIndex]);
                        })
                        .AppendInterval(enemy.animController.GetCustomAnimDuration(boss.summonAnimInfo));
                }
                
            }
                
            return seq;
        }
    }
}