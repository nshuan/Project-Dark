using UnityEngine;

namespace InGame
{
    public interface IEffectTarget
    {
        Transform TargetTransform { get; }
        Vector2 Position => TargetTransform.position;
        void Burn(float duration, float delayEachBurn, int damage);
    }
}