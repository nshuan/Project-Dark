using System;
using UnityEngine;

namespace InGame
{
    public interface IDamageable
    {
        void Damage(int damage, Vector2 damageDirection, float stagger);
        bool IsDestroyed { get; set; }
        Action<int> OnHit { get; set; }
    }
}