using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InGame
{
    [Serializable]
    public class PlayerShootStraight : PlayerSkillBehaviour
    {
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
            int maxHit,
            bool isCharge,
            List<IProjectileActivate> activateActions,
            List<IProjectileHit> projectileHitActions)
        {
            const float delayEachBullet = 0.1f;
            for (var i = 0; i < numberOfBullets; i++)
            {
                var p = ProjectilePool.Instance.Get(projectilePrefab, null, false);
                p.transform.position = spawnPos;
                p.Init(spawnPos, (target - spawnPos).normalized, skillRange, skillSize, bulletSpeedScale, damagePerBullet, criticalDamagePerBullet, criticalRatePerBullet, stagger, isCharge, maxHit, activateActions, projectileHitActions);
                p.Activate(delayEachBullet * i);
            }
        }
    }
}