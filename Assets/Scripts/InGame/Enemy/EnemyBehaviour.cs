using System;
using DG.Tweening;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Enemy/Enemy Behaviour", fileName = "EnemyBehaviour")]
    public class EnemyBehaviour : ScriptableObject
    {
        public int enemyId;
        public EnemySpawnBehaviour spawnBehaviour;
        public EnemyMoveBehaviour moveBehaviour;
        public EnemyAttackBehaviour attackBehaviour;
        public float attackRange; // Distance to start attacking
        public float attackSpeed; // Attack speed
        public float hp;
        public float dmg; // Base damage
        public float moveSpeed;
        
        public void Init(Transform enemy)
        {
            spawnBehaviour.Init(enemy);
            moveBehaviour.Init();
        }
        
        public void Spawn(Transform enemy, Action completeCallback)
        {
            DOTween.Kill(enemy);
            spawnBehaviour?.DoSpawn(enemy).OnComplete(() => completeCallback?.Invoke()).SetTarget(enemy);
        }
    }

    public enum EnemyState
    {
        Spawn,
        Move
    }
}