using DG.Tweening;
using UnityEngine;

namespace InGame.SpawnBehaviour
{
    [CreateAssetMenu(menuName = "InGame/Enemy/Spawn/Enemy Spawn Anim", fileName = "EnemySpawnAnim")]
    public class EnemyAnimSpritesSpawnBehaviour : EnemySpawnBehaviour
    {
        public override void Init(EnemyEntity enemy)
        {
            
        }

        public override Tween DoSpawn(EnemyEntity enemy)
        {
            var spawnTime = enemy.animController.PlaySpawn();
            return DOTween.Sequence().AppendInterval(spawnTime);
        }
    }
}