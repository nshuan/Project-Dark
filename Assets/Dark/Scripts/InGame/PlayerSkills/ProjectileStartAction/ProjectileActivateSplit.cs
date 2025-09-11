using System;
using UnityEngine;

namespace InGame
{
    [Serializable]
    public class ProjectileActivateSplit : IProjectileActivate
    {
        [SerializeField] private ProjectileEntity projectile;
        [SerializeField] private int amount;
        [Range(0f, 180f)] 
        [SerializeField] private float angle;
        
        public void DoAction(ProjectileEntity parentProjectile, Vector2 direction)
        {
            direction.Normalize();

            var spawnPos = new Vector2();
            var calculatedAmount = amount;
            
            ProjectileEntity p = null;
            if (calculatedAmount % 2 == 1)
            {
                p = ProjectilePool.Instance.Get(projectile, null, false);
                spawnPos.x = parentProjectile.transform.position.x;
                spawnPos.y = parentProjectile.transform.position.y;
                p.transform.position = spawnPos;
                p.Init(
                    spawnPos, 
                    direction, 
                    parentProjectile.maxDistance, 
                    parentProjectile.Size, 
                    parentProjectile.SpeedScale, 
                    parentProjectile.Damage, 
                    parentProjectile.CriticalDamage, 
                    parentProjectile.CriticalRate, 
                    parentProjectile.Stagger, 
                    parentProjectile.IsCharge, 
                    parentProjectile.MaxHit, 
                    null,
                    parentProjectile.HitActions,
                    ProjectileType.PlayerProjectile
                    );
                p.Activate(0.2f);

                calculatedAmount -= 1;
            }
            
            for (var i = 1; i <= calculatedAmount / 2; i++)
            {
                var pDir = (Vector2)(Quaternion.Euler(0f, 0f, i * angle / calculatedAmount) * direction);
                p = ProjectilePool.Instance.Get(projectile, null, false);
                spawnPos.x = parentProjectile.transform.position.x;
                spawnPos.y = parentProjectile.transform.position.y;
                p.transform.position = spawnPos;
                p.Init(
                    spawnPos, 
                    pDir, 
                    parentProjectile.maxDistance, 
                    parentProjectile.Size, 
                    parentProjectile.SpeedScale, 
                    parentProjectile.Damage, 
                    parentProjectile.CriticalDamage, 
                    parentProjectile.CriticalRate, 
                    parentProjectile.Stagger, 
                    parentProjectile.IsCharge, 
                    parentProjectile.MaxHit, 
                    null,
                    parentProjectile.HitActions,
                    ProjectileType.PlayerProjectile
                );
                p.Activate(0f);
            
                pDir = Quaternion.Euler(0f, 0f, - i * angle / calculatedAmount) * direction;
                p = ProjectilePool.Instance.Get(projectile, null, false);
                spawnPos.x = parentProjectile.transform.position.x;
                spawnPos.y = parentProjectile.transform.position.y;
                p.transform.position = spawnPos;
                p.Init(
                    spawnPos, 
                    pDir, 
                    parentProjectile.maxDistance, 
                    parentProjectile.Size, 
                    parentProjectile.SpeedScale, 
                    parentProjectile.Damage, 
                    parentProjectile.CriticalDamage, 
                    parentProjectile.CriticalRate, 
                    parentProjectile.Stagger, 
                    parentProjectile.IsCharge, 
                    parentProjectile.MaxHit, 
                    null,
                    parentProjectile.HitActions,
                    ProjectileType.PlayerProjectile
                );
                p.Activate(0f);
            }
        }
    }
}