using UnityEngine;

namespace InGame
{
    public abstract class EnemyAttackBehaviour : ScriptableObject
    {
        public abstract void Attack(IDamageable target, int damage);
    }
}