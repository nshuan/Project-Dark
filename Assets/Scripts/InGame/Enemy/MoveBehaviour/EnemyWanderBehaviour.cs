using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Enemy/Move/Enemy Wander", fileName = "EnemyWander")]
    public class EnemyWanderBehaviour : EnemyMoveBehaviour
    {
        public override void Init()
        {
            
        }
        
        public override void Move(Transform enemy, Vector2 target, float stopRange, float speed)
        {
            if (Vector2.Distance(enemy.position, target) > MinDelta)
            {
                Vector2 direction = (target - (Vector2)enemy.position).normalized;
                Vector2 currentDirection = enemy.right;
                Vector2 curvedDirection = Vector2.Lerp(currentDirection, direction, Time.deltaTime * speed).normalized;

                enemy.position += (Vector3)(speed * Time.deltaTime * curvedDirection);

                float angle = Mathf.Atan2(curvedDirection.y, curvedDirection.x) * Mathf.Rad2Deg;
                enemy.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
}