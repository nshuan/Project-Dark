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
        
        private EnemyEntity Enemy { get; set; }
        public EnemyState State { get; set; }

        public void Init(EnemyEntity enemy)
        {
            Enemy = enemy;
            moveBehaviour.Init();
            State = EnemyState.Spawn;
        }
        
        public void Spawn(Action completeCallback)
        {
            DOTween.Kill(this);
            spawnBehaviour?.DoSpawn(Enemy.transform).OnComplete(() => completeCallback?.Invoke()).SetTarget(this);
        }
    }

    public enum EnemyState
    {
        Spawn,
        Move
    }
}