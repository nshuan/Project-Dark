using UnityEngine;

namespace InGame
{
    public abstract class EnemyMoveBehaviour : ScriptableObject
    {
        protected const float MinDelta = 0.001f;
        public abstract void Move(Transform enemy, Vector2 target, Vector2 directionAdder, float stopRange, float speed);
    }
}