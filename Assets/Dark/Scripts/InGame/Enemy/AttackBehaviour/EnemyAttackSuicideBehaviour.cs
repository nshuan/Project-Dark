using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Enemy/Attack/Enemy Attack Suicide", fileName = "EnemyAttackSuicide")]
    public class EnemyAttackSuicideBehaviour : EnemyAttackBehaviour
    {
        public override void Attack(EnemyEntity enemy, TowerEntity target, Vector2 enemyPosition, int damage)
        {
            target.HitDirectionX = target.transform.position.x - enemyPosition.x;
            target.HitDirectionY = target.transform.position.y - enemyPosition.y;
            target.Damage(damage, enemyPosition, 0f, DamageType.Enemy);
            enemy.Kill(DamageType.SelfDestruct);
        }
    }
}