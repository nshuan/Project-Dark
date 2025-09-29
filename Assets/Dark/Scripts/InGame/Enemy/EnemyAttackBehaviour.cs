using UnityEngine;

namespace InGame
{
    public abstract class EnemyAttackBehaviour : ScriptableObject
    {
        public abstract void Attack(TowerEntity target, Vector2 enemyPosition, int damage);
    }
}