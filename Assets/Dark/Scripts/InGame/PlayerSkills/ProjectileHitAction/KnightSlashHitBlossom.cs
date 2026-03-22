using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame
{
 	[Serializable]
    public class KnightSlashHitBlossom : ProjectileHitBlossom
    {
        public override void DoAction(ProjectileEntity parentProjectile, Vector2 position, Action<ProjectileEntity> callbackLateInit)
        {
            if (bulletAmount == 0) return;
            if (blossomSize == 0) return;
            if (!projectile) return;
            
            var dir = Random.insideUnitCircle.normalized;
            var angle = 360f / bulletAmount;
            
            // Chỉ gây dame 1 lần
            var hasSetupDamage = false;
            
            for (var i = 0; i < bulletAmount; i++)
            {
                var damage = parentProjectile.Damage;
                var criticalDamage = parentProjectile.CriticalDamage;
                var size = 360f;
                if (hasSetupDamage)
                {
                    damage = 0;
                    criticalDamage = 0;
                    size = 0;
                }

                hasSetupDamage = true;
                var pDir = (Vector2)(Quaternion.Euler(0f, 0f, angle * i) * dir);
                var p = ProjectilePool.Instance.Get(projectile, null, false);
                p.transform.position = position;
                p.Init(
                    position, 
                    pDir.normalized, 
                    blossomSize, 
                    size, 
                    LevelUtilityV2.StatsNormalAttack.speedScale, 
                    damage, 
                    criticalDamage, 
                    parentProjectile.CriticalRate,
                    parentProjectile.Stagger, 
                    false,
                    1, 
                    null, 
                    null, 
                    ProjectileType.PlayerProjectile);
                
                callbackLateInit?.Invoke(p);
                
                p.Activate(0f);
            }
        }
    }
}