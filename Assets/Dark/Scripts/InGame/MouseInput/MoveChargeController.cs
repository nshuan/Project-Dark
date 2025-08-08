using System;
using UnityEngine;

namespace InGame
{
    public abstract class MoveChargeController : MonoBehaviour
    {
        protected ProjectileEntity projectilePrefab;
        public Camera Cam { get; set; }
        
        public void SetProjectile(ProjectileEntity projectileEntity)
        {
            projectilePrefab = projectileEntity;    
        }
        
        public abstract void AddBullet(Vector2 spawnPos, Vector2 aimDirection);
        public abstract void AddSize(float size);
        public abstract void Attack(Action<ProjectileEntity, Vector2> actionSetupProjectile);
    }
}