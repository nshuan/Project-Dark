using UnityEngine;

namespace InGame
{
    public abstract class EnemyAttackBehaviour : ScriptableObject
    {
        public abstract void Attack(EnemyEntity enemy, TowerEntity target, Vector2 enemyPosition, int damage);
    }
}