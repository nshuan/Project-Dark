using UnityEngine;

namespace InGame.EnemyMovement
{
    public class EnemyMoveStraight : EnemyMovementBehaviour
    {
        public override void Move(Transform target, float speed, float minDistance)
        {
            if (Vector3.Distance(transform.position, target.position) > minDistance)
            {
                transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * speed);
                transform.rotation = Quaternion.Euler(Vector3.Lerp(transform.eulerAngles,
                    transform.eulerAngles + new Vector3(0f, 0f, 180f), Time.deltaTime * speed));
            }
        }
    }
}