using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Enemy/Enemy Behaviour", fileName = "EnemyBehaviour")]
    public class EnemyBehaviour : ScriptableObject
    {
        public int enemyId;
        public EnemyEntity enemyPrefab;
        public EnemySpawnBehaviour spawnBehaviour;
        public EnemyMoveBehaviour moveBehaviour;
        public EnemyAttackBehaviour attackBehaviour;
        public float attackRange; // Distance to start attacking
        public float attackSpeed; // Attack speed
        public int hp;
        public int dmg; // Base damage
        public float moveSpeed;
        public List<ActionEffectConfig> effects; 
        
        public void Init(Transform enemy)
        {
            spawnBehaviour.Init(enemy);
        }
        
        public void Spawn(Transform enemy, Action completeCallback)
        {
            DOTween.Kill(enemy);
            if (spawnBehaviour)
            {
                enemy.gameObject.SetActive(true);
                spawnBehaviour.DoSpawn(enemy).OnComplete(() => completeCallback?.Invoke()).SetTarget(enemy);
            }
        }
    }

    public enum EnemyState
    {
        Spawn,
        Move
    }
}