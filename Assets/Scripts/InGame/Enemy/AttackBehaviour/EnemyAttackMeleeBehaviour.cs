using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Enemy/Attack/Enemy Attack Melee", fileName = "EnemyAttackMelee")]
    public class EnemyAttackMeleeBehaviour : EnemyAttackBehaviour
    {
        public override void Attack(IDamageable target, int damage)
        {
            target.Damage(damage, Vector2.zero, 0f);
        }
    }
}