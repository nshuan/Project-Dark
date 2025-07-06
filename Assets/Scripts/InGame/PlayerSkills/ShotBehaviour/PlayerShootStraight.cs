using System;
using InGame.Pool;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InGame
{
    [Serializable]
    public class PlayerShootStraight : PlayerSkillBehaviour
    {
        public override void Shoot(ProjectileEntity projectilePrefab, Vector2 spawnPos, float bulletRange, Vector2 target, int damagePerBullet, int criticalDamagePerBullet, float criticalRatePerBullet, int numberOfBullets, float bulletSize, float bulletSpeedScale, float stagger)
        {
            const float delayEachBullet = 0.1f;
            for (var i = 0; i < numberOfBullets; i++)
            {
                var p = ProjectilePool.Instance.Get(projectilePrefab, null, false);
                p.transform.position = spawnPos;
                p.Init(spawnPos, (target - spawnPos).normalized, bulletRange, bulletSpeedScale, damagePerBullet, criticalDamagePerBullet, criticalRatePerBullet, stagger);
                p.Activate(delayEachBullet * i);
            }
        }
    }
}