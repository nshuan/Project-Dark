using UnityEngine;

namespace InGame
{
    public interface ITowerCounterLogic
    {
        void Counter(Vector2 towerAttackPos, Vector2 direction, int damage, float speedScale);
    }
}