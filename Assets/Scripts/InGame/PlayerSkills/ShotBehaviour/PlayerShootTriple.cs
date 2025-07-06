using System;
using InGame.Pool;
using UnityEngine;

namespace InGame
{
    [Serializable]
    public class PlayerShootTriple : PlayerSkillBehaviour
    {
        [Range(0f, 180f)] public float angle;
        
        public override void Shoot(ProjectileEntity projectilePrefab, Vector2 spawnPos, float bulletRange, Vector2 target, int damagePerBullet, int criticalDamagePerBullet, float criticalRatePerBullet, int numberOfBullets, float bulletSize, float bulletSpeedScale, float stagger)
        {
            const float delayEachBullet = 0.1f;
            
            for (var i = 0; i < numberOfBullets; i++)
            {
                var dir = target - spawnPos;
                var p = ProjectilePool.Instance.Get(projectilePrefab, null, false);
                p.transform.position = spawnPos;
                p.Init(spawnPos, dir.normalized, bulletRange, bulletSpeedScale, damagePerBullet, criticalDamagePerBullet, criticalRatePerBullet, stagger);
                p.Activate(delayEachBullet * i);
                
                var pDir = (Vector2)(Quaternion.Euler(0f, 0f, angle / 2) * dir);
                p = ProjectilePool.Instance.Get(projectilePrefab, null, false);
                p.transform.position = spawnPos;
                p.Init(spawnPos, pDir.normalized, bulletRange, bulletSpeedScale, damagePerBullet, criticalDamagePerBullet, criticalRatePerBullet, stagger);
                p.Activate(delayEachBullet * i);
            
                pDir = Quaternion.Euler(0f, 0f, - angle / 2) * dir;
                p = ProjectilePool.Instance.Get(projectilePrefab, null, false);
                p.transform.position = spawnPos;
                p.Init(spawnPos, pDir.normalized, bulletRange, bulletSpeedScale, damagePerBullet, criticalDamagePerBullet, criticalRatePerBullet, stagger);
                p.Activate(delayEachBullet * i);
            }
            
        }
    }
}