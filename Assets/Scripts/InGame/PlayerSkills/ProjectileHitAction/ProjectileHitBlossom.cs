using System;
using InGame.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame
{
    /// <summary>
    /// Prefab của projectile sẽ được set ở trong upgrade node
    /// </summary>
    [Serializable]
    public class ProjectileHitBlossom : IProjectileHit
    {
        [SerializeField] private ProjectileEntity projectile;

        public int bulletAmount = 8;
        public float blossomSize = 3f;
        public int damage;
        public float stagger;
        
        public void DoAction(Vector2 position)
        {
            var dir = Random.insideUnitCircle.normalized;
            var angle = 360f / bulletAmount;
            
            for (var i = 0; i < bulletAmount; i++)
            {
                var pDir = (Vector2)(Quaternion.Euler(0f, 0f, angle * i) * dir);
                var p = ProjectilePool.Instance.Get(projectile, null, false);
                p.transform.position = position;
                p.Init(position, pDir.normalized, blossomSize, 1f, 1f, damage, damage, 0f, stagger, false,null);
                p.Activate(0f);
            }
        }
    }
}