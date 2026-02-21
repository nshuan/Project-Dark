using DG.Tweening;
using InGame.Boss;
using UnityEngine;

namespace InGame.SpawnBehaviour
{
    [CreateAssetMenu(menuName = "InGame/Enemy/Spawn/Enemy Spawn Summoner King", fileName = "EnemySpawnSummonerKing")]
    public class EnemySummonerKingSpawnBehaviour : EnemyNecromancerSpawnBehaviour
    {
        public override Tween DoSpawn(EnemyEntity enemy)
        {
            if (enemy.config is EnemySummonBehaviour enemyConfig)
            {
                var spawnTime = enemy.animController.PlaySpawn();
                var seq = DOTween.Sequence().AppendInterval(spawnTime).SetTarget(enemy);

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
                            summonBehaviour.Summon(enemy, enemy.TargetTower,
                                enemyConfig.listSummonIdsOnSpawned[summonIndex],
                                enemyConfig.listSummonAmountOnSpawned[summonIndex]);
                        });
                }

                if (enemy is BossSummonerKingEntity boss)
                {
                    var attackDuration = enemy.animController.GetAttackDuration();
                    var buffDuration = enemy.animController.GetCustomAnimDuration(boss.buffAnim);
                    seq.AppendInterval(attackDuration - 0.5f)
                        .AppendCallback(() => boss.animController.PlayCustomAnim(boss.buffAnim))
                        .AppendInterval(boss.delayTriggerBuff)
                        .AppendCallback(() => Buff(enemy))
                        .AppendInterval(buffDuration - boss.delayTriggerBuff);
                }
                
                return seq;
            }

            // Spawn thường
            enemy.transform.localScale = 0.3f * Vector3.one;
            return enemy.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }

        private void Buff(EnemyEntity thisEnemy)
        {
            foreach (var enemy in EnemyManager.Instance.Enemies)
            {
                if (enemy.Value == thisEnemy) continue;
                
                if (enemy.Value.gameObject.activeInHierarchy &&
                    enemy.Value.IsDestroyed == false)
                {
                    enemy.Value.TempDmgScale = 1.5f;
                    enemy.Value.TempSpeedScale = 1.5f;
                }
            }
        }
    }
}