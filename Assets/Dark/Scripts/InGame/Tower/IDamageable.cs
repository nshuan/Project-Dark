using System;
using UnityEngine;

namespace InGame
{
    public interface IDamageable
    {
        float HitDirectionX { get; set; }
        float HitDirectionY { get; set; }
        void Damage(int damage, Vector2 dealerPosition, float stagger, DamageType dmgType);
        bool IsDestroyed { get; set; }
        Action<int, DamageType> OnHit { get; set; } // <damage, DamageType>
    }

    public enum DamageType
    {
        Normal,
        NormalCritical
    }
}