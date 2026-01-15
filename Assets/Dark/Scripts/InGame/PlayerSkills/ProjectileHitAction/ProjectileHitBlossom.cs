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
        
        public void DoAction(ProjectileEntity parentProjectile, Vector2 position, Action<ProjectileEntity> callbackLateInit)
        {
            if (bulletAmount == 0) return;
            if (blossomSize == 0) return;
            if (!projectile) return;
            
            var dir = Random.insideUnitCircle.normalized;
            var angle = 360f / bulletAmount;
            
            for (var i = 0; i < bulletAmount; i++)
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
                    LevelUtilityV2.StatsNormalAttack.speedScale, 
                    parentProjectile.Damage, 
                    parentProjectile.CriticalDamage, 
                    parentProjectile.CriticalRate,
                    parentProjectile.Stagger, 
                    false,
                    1, 
                    null, 
                    null, 
                    ProjectileType.PlayerProjectile);
                
                callbackLateInit?.Invoke(p);
                
                p.Activate(0);
            }
        }
    }
}