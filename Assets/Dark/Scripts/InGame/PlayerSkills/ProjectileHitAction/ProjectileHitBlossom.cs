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
        public ProjectileEntity projectile;

        public int bulletAmount = 5;
        public float blossomSize = 3f;
        
        public void DoAction(ProjectileEntity parentProjectile, Vector2 position)
        {
            var amount = LevelUtility.GetChargeSizeExplodeBullet(bulletAmount);
            
            var dir = Random.insideUnitCircle.normalized;
            var angle = 360f / amount;
            
            for (var i = 0; i < amount; i++)
            {
                var pDir = (Vector2)(Quaternion.Euler(0f, 0f, angle * i) * dir);
                var p = ProjectilePool.Instance.Get(projectile, null, false);
                p.transform.position = position;
                p.transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(pDir.y, pDir.x) * Mathf.Rad2Deg);
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