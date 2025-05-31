using System;
using UnityEngine;

namespace InGame.Trap
{
    public class MonoTrap : MonoBehaviour
    {
        public float RefreshRate { get; set; } = 10f; // Check hits per seconds
        
        public virtual void Setup(Camera cam, EnemyEntity target, Vector2 size, float damage, float duration, Action onComplete)
        {
            
        }
    }
}