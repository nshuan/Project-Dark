using System;
using UnityEngine;

namespace InGame
{
    public interface IDamageable
    {
        float HitDirectionX { get; set; }
        float HitDirectionY { get; set; }
        void Damage(int damage, Vector2 dealerPosition, float stagger);
        bool IsDestroyed { get; set; }
        Action<int> OnHit { get; set; }
    }
}