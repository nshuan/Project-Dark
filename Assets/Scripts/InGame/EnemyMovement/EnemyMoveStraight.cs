using UnityEngine;

namespace InGame.EnemyMovement
{
    public class EnemyMoveStraight : EnemyMovementBehaviour
    {
        public override void Init()
        {
            
        }

        public override void Move(Vector2 target, float speed)
        {
            if (Vector3.Distance(transform.position, target) > MinDelta)
            {
                transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * speed);
                transform.rotation = Quaternion.Euler(Vector3.Lerp(transform.eulerAngles,
                    transform.eulerAngles + new Vector3(0f, 0f, 180f), Time.deltaTime * speed));
            }
        }
    }
}