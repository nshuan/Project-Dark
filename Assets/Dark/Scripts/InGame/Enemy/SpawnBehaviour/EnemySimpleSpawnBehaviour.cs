using System;
using DG.Tweening;
using UnityEngine;

namespace InGame.SpawnBehaviour
{
    [CreateAssetMenu(menuName = "InGame/Enemy/Spawn/Enemy Simple Spawn", fileName = "EnemySimpleSpawn")]
    public class EnemySimpleSpawnBehaviour : EnemySpawnBehaviour
    {
        public override void Init(EnemyEntity enemy)
        {
            enemy.transform.localScale = Vector3.zero;
        }

        public override Tween DoSpawn(EnemyEntity enemy)
        {
            enemy.transform.localScale = 0.3f * Vector3.one;
            return enemy.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }
    }
}