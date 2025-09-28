using System;
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
        
        public void DoAction(ProjectileEntity parentProjectile, Vector2 position)
        {
            var dir = Random.insideUnitCircle.normalized;
            var angle = 360f / bulletAmount;
            
            for (var i = 0; i < bulletAmount; i++)
            {
                var pDir = (Vector2)(Quaternion.Euler(0f, 0f, angle * i) * dir);
                var p = ProjectilePool.Instance.Get(projectile, null, false);
                p.transform.position = position;
                p.Init(
                    position, 
                    pDir.normalized, 
                    blossomSize, 
                    parentProjectile.Size, 
                    1.3f, 
                    parentProjectile.Damage, 
                    parentProjectile.CriticalDamage, 
                    parentProjectile.CriticalRate,
                    parentProjectile.Stagger, 
                    false,
                    1, 
                    null, 
                    null, 
                    ProjectileType.PlayerProjectile);
                p.Activate(0f);
            }
        }
    }
}