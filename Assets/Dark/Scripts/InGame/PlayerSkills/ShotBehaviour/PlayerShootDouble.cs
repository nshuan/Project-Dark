using System;
using System.Collections.Generic;
using InGame.Pool;
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
            Vector2 target, 
            int damagePerBullet,
            int criticalDamagePerBullet,
            float criticalRatePerBullet,
            int numberOfBullets,
            float skillSize, 
            float skillRange, 
            float bulletSpeedScale, 
            float stagger, 
            bool isCharge,
            List<IProjectileHit> projectileHitActions)
        {
            var dir = target - spawnPos;
            var pDir = (Vector2)(Quaternion.Euler(0f, 0f, angle / 2) * dir);
            var p = ProjectilePool.Instance.Get(projectilePrefab, null, false);
            p.transform.position = spawnPos;
            p.Init(spawnPos, pDir.normalized, skillRange, skillSize, bulletSpeedScale, damagePerBullet, criticalDamagePerBullet, criticalRatePerBullet, stagger, isCharge, projectileHitActions);
            p.Activate(0);
            
            pDir = Quaternion.Euler(0f, 0f, - angle / 2) * dir;
            p = ProjectilePool.Instance.Get(projectilePrefab, null, false);
            p.transform.position = spawnPos;
            p.Init(spawnPos, pDir.normalized, skillRange, skillSize, bulletSpeedScale, damagePerBullet, criticalDamagePerBullet, criticalRatePerBullet, stagger, isCharge, projectileHitActions);
            p.Activate(0);
        }
    }
}