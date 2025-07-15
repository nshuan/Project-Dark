using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Enemy/Move/Enemy Move Straight", fileName = "EnemyMoveStraight")]
    public class EnemyMoveStraightBehaviour : EnemyMoveBehaviour
    {
        public override void Move(Transform enemy, Vector2 target, Vector2 directionAdder, float stopRange, float speed)
        {
            var enemyPos = enemy.position;
            var distance = Vector2.Distance(enemyPos, target);
            if (distance > MinDelta)
            {
                var dirX = target.x - enemyPos.x;
                var dirY = target.y - enemyPos.y;
                var magnitude = Mathf.Sqrt(dirX * dirX + dirY * dirY);
                var calculatedDir = new Vector3(
                    dirX + directionAdder.x * magnitude, 
                    dirY + directionAdder.y * magnitude,
                    0f);
                // Remove normalized to make it similar to Vector3.Lerp
                enemy.position = enemyPos + calculatedDir.normalized * (Time.deltaTime * speed);
            }
        }

        public override void MoveNonAlloc(Transform enemy, Vector2 target, Vector2 directionAdder, float stopRange, float speed,
            ref Vector3 direction)
        {
            var enemyPos = enemy.position;
            var distance = Vector2.Distance(enemyPos, target);
            if (distance > MinDelta)
            {
                var dirX = target.x - enemyPos.x;
                var dirY = target.y - enemyPos.y;
                var magnitude = Mathf.Sqrt(dirX * dirX + dirY * dirY);
                direction.x = dirX + directionAdder.x * magnitude;
                direction.y = dirY + directionAdder.y * magnitude;
                direction.z = 0;
                // Remove normalized to make it similar to Vector3.Lerp
                enemy.position = enemyPos + direction.normalized * (Time.deltaTime * speed);
            }
        }
    }
}