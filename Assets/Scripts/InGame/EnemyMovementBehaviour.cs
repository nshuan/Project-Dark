using UnityEngine;

namespace InGame
{
    public abstract class EnemyMovementBehaviour : MonoBehaviour
    {
        public abstract void Move(Transform target, float speed, float minDistance);
    }
}