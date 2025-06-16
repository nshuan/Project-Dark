using UnityEngine;

namespace InGame
{
    public abstract class EnemyMovementBehaviour : MonoBehaviour
    {
        protected const float MinDelta = 0.001f;
        public abstract void Init();
        public abstract void Move(Vector2 target, float speed);
    }
}