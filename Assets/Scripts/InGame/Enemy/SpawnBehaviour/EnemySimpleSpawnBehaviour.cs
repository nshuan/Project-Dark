using System;
using DG.Tweening;
using UnityEngine;

namespace InGame.SpawnBehaviour
{
    [CreateAssetMenu(menuName = "InGame/Enemy/Enemy Simple Spawn", fileName = "EnemySimpleSpawn")]
    public class EnemySimpleSpawnBehaviour : EnemySpawnBehaviour
    {
        public override void Init(Transform enemy)
        {
            enemy.localScale = Vector3.zero;
        }

        public override Tween DoSpawn(Transform enemy)
        {
            enemy.localScale = 0.3f * Vector3.one;
            return enemy.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }
    }
}