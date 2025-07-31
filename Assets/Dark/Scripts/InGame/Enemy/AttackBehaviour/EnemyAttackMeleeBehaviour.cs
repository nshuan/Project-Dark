using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Enemy/Attack/Enemy Attack Melee", fileName = "EnemyAttackMelee")]
    public class EnemyAttackMeleeBehaviour : EnemyAttackBehaviour
    {
        public override void Attack(TowerEntity target, Vector2 enemyPosition, int damage)
        {
            target.HitDirectionX = target.transform.position.x - enemyPosition.x;
            target.HitDirectionY = target.transform.position.y - enemyPosition.y;
            target.Damage(damage, enemyPosition, 0f);
        }
    }
}