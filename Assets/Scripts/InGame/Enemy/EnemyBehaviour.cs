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
        public float dmg; // Base damage
        public float moveSpeed;
    }
}