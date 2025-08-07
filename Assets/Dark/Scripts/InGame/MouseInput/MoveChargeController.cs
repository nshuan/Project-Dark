using System;

namespace InGame
{
    public abstract class MoveChargeController
    {
        protected ProjectileEntity projectilePrefab;
        
        public void SetProjectile(ProjectileEntity projectileEntity)
        {
            projectilePrefab = projectileEntity;    
        }
        
        public abstract void AddBullet();
        public abstract void Attack(Action<ProjectileEntity> actionSetupProjectile);
    }
}