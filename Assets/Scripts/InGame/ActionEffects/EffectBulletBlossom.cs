using System;
using InGame.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame
{
    [Serializable]
    public class EffectBulletBlossom : IActionEffectLogic
    {
        [SerializeField] private ProjectileEntity projectilePrefab;
        [SerializeField] private int bulletAmount;
        
        public void Initialize()
        {
            
        }

        public void TriggerEffect(int effectId, Vector2 center, float size, float value, float stagger)
        {
            var dir = Random.insideUnitCircle.normalized;
            var angle = 360f / bulletAmount;
            
            for (var i = 0; i < bulletAmount; i++)
            {
                var pDir = (Vector2)(Quaternion.Euler(0f, 0f, angle * i) * dir);
                var p = ProjectilePool.Instance.Get(projectilePrefab, null, false);
                p.transform.position = center;
                p.Init(center, pDir.normalized, size, 1f, (int)value, (int)value, 0f, stagger, null);
                p.Activate(0f);
            }
        }
    }
}