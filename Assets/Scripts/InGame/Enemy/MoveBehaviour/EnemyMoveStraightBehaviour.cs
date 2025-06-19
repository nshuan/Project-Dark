using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Enemy/Enemy Move Straight", fileName = "EnemyMoveStraight")]
    public class EnemyMoveStraightBehaviour : EnemyMoveBehaviour
    {
        public override void Init()
        {
            
        }

        public override void Move(Transform enemy, Vector2 target, float speed)
        {
            if (Vector3.Distance(enemy.position, target) > MinDelta)
            {
                enemy.position = Vector3.Lerp(enemy.position, target, Time.deltaTime * speed);
                enemy.rotation = Quaternion.Euler(Vector3.Lerp(enemy.eulerAngles,
                    enemy.eulerAngles + new Vector3(0f, 0f, 180f), Time.deltaTime * speed));
            }
        }
    }
}