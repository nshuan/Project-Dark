using System;
using UnityEngine;

namespace InGame
{
    public interface IEffectTarget
    {
        Transform TargetTransform { get; }
        float PercentageHpLeft { get; }
        Vector2 Position => TargetTransform.position;
        void Burn(float duration, float delayEachBurn, int damage, Action callbackComplete);
        void Kill();
    }
}