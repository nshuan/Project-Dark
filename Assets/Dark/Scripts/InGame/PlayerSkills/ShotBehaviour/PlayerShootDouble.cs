using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InGame
{
    [Serializable]
    public class PlayerShootDouble : PlayerSkillBehaviour
    {
        [Range(0f, 180f)] public float angle;
        
        public override void Shoot(
            ProjectileEntity projectilePrefab, 
            Vector2 spawnPos, 
            Vector2 rangeCenter,
            Vector2 target, 
            int damagePerBullet,
            int criticalDamagePerBullet,
            float criticalRatePerBullet,
            int numberOfBullets,
            float skillSize, 
            float skillRange, 
            float bulletSpeedScale, 
            float stagger, 
            int maxHit,
            bool isCharge,
            List<IProjectileActivate> activateActions,
            List<IProjectileHit> projectileHitActions)
        {
            ShootWithTarget(projectilePrefab, null, true, spawnPos, rangeCenter, target, damagePerBullet,
                criticalDamagePerBullet, criticalRatePerBullet, numberOfBullets,
                skillSize, skillRange, bulletSpeedScale, stagger, maxHit, isCharge, activateActions,
                projectileHitActions);
        }

        public override void ShootWithTarget(ProjectileEntity projectilePrefab, EnemyEntity targetEnemy, bool isTargetEnemyForceSelect, Vector2 spawnPos, Vector2 rangeCenter,
            Vector2 target, int damagePerBullet, int criticalDamagePerBullet, float criticalRatePerBullet, int numberOfBullets,
            float skillSize, float skillRange, float bulletSpeedScale, float stagger, int maxHit, bool isCharge,
            List<IProjectileActivate> activateActions, List<IProjectileHit> hitEffects)
        {
            var dir = target - spawnPos;
            var pDir = (Vector2)(Quaternion.Euler(0f, 0f, angle / 2) * dir);
            var p = ProjectilePool.Instance.Get(projectilePrefab, null, false);
            p.transform.position = spawnPos;
            p.Init(rangeCenter, pDir.normalized, skillRange, skillSize, bulletSpeedScale, damagePerBullet, criticalDamagePerBullet, criticalRatePerBullet, stagger, isCharge, maxHit, activateActions, hitEffects, ProjectileType.PlayerProjectile);
            p.Activate(0);
            if (p is AutoAimProjectileEntity autoAimP)
            {
                autoAimP.TargetToChase = targetEnemy;
                autoAimP.IsTargetToChaseSelectByMouse = isTargetEnemyForceSelect;
            }
            
            pDir = Quaternion.Euler(0f, 0f, - angle / 2) * dir;
            p = ProjectilePool.Instance.Get(projectilePrefab, null, false);
            p.transform.position = spawnPos;
            p.Init(rangeCenter, pDir.normalized, skillRange, skillSize, bulletSpeedScale, damagePerBullet, criticalDamagePerBullet, criticalRatePerBullet, stagger, isCharge, maxHit, activateActions, hitEffects, ProjectileType.PlayerProjectile);
            p.Activate(0);
            if (p is AutoAimProjectileEntity autoAimP1)
            {
                autoAimP1.TargetToChase = targetEnemy;
                autoAimP1.IsTargetToChaseSelectByMouse = isTargetEnemyForceSelect;
            }
        }
    }
}