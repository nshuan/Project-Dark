using UnityEngine;

namespace InGame
{
    public abstract class EnemyMoveBehaviour : ScriptableObject
    {
        protected const float MinDelta = 0.001f;
        public abstract void Init();
        public abstract void Move(Transform enemy, Vector2 target, float stopRange, float speed);
    }
}