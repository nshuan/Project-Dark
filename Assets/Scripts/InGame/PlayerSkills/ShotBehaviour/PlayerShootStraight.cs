using System;
using InGame.Pool;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InGame
{
    [Serializable]
    public class PlayerShootStraight : PlayerSkillBehaviour
    {
        public override void Shoot(ProjectileEntity projectilePrefab, Vector2 spawnPos, Vector2 target, int damagePerBullet, int criticalDamagePerBullet, float criticalRatePerBullet, int numberOfBullets, float bulletSpeedScale)
        {
            const float delayEachBullet = 0.1f;
            for (var i = 0; i < numberOfBullets; i++)
            {
                var p = ProjectilePool.Instance.Get(projectilePrefab, null, false);
                p.transform.position = spawnPos;
                p.Init(bulletSpeedScale, target, damagePerBullet, criticalDamagePerBullet, criticalRatePerBullet);
                p.Activate(delayEachBullet * i);
            }
        }
    }
}