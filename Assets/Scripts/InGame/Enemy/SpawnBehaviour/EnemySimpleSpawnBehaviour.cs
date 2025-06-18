using System;
using DG.Tweening;
using UnityEngine;

namespace InGame.SpawnBehaviour
{
    [CreateAssetMenu(menuName = "InGame/Enemy/Enemy Simple Spawn", fileName = "EnemySimpleSpawn")]
    public class EnemySimpleSpawnBehaviour : EnemySpawnBehaviour
    {
        protected override Tween DoSpawn()
        {
            throw new Exception("Not implemented!");
        }
    }
}