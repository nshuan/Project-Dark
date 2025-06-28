using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Enemy/Move/Enemy Move Straight", fileName = "EnemyMoveStraight")]
    public class EnemyMoveStraightBehaviour : EnemyMoveBehaviour
    {
        public override void Move(Transform enemy, Vector2 target, Vector2 directionAdder, float stopRange, float speed)
        {
            if (Vector3.Distance(enemy.position, target) > MinDelta)
            {
                var enemyPos = enemy.position;
                var direction = target - (Vector2)enemyPos;
                var calculatedTarget = direction.normalized + directionAdder;
                enemyPos = Vector3.Lerp(enemyPos, enemyPos + (Vector3)(direction.magnitude * calculatedTarget), Time.deltaTime * speed);
                enemy.position = enemyPos;
                // enemy.rotation = Quaternion.Euler(Vector3.Lerp(enemy.eulerAngles,
                //     enemy.eulerAngles + new Vector3(0f, 0f, 180f), Time.deltaTime * speed));
            }
        }
    }
}