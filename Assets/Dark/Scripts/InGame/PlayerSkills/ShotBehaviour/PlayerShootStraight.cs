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
            ShootWithTarget(projectilePrefab, null, spawnPos, rangeCenter, target, damagePerBullet,
                criticalDamagePerBullet, criticalRatePerBullet, numberOfBullets,
                skillSize, skillRange, bulletSpeedScale, stagger, maxHit, isCharge, activateActions,
                projectileHitActions);
        }

        public override void ShootWithTarget(ProjectileEntity projectilePrefab, EnemyEntity targetEnemy, Vector2 spawnPos, Vector2 rangeCenter, Vector2 target,
            int damagePerBullet, int criticalDamagePerBullet, float criticalRatePerBullet, int numberOfBullets, float skillSize,
            float skillRange, float bulletSpeedScale, float stagger, int maxHit, bool isCharge, List<IProjectileActivate> activateActions,
            List<IProjectileHit> hitEffects)
        {
            const float delayEachBullet = 0.1f;
            for (var i = 0; i < numberOfBullets; i++)
            {
                var p = ProjectilePool.Instance.Get(projectilePrefab, null, false);
                var direction = (target - spawnPos).normalized;
                p.transform.position = spawnPos;
                p.transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
                p.Init(rangeCenter, (target - spawnPos).normalized, skillRange, skillSize, bulletSpeedScale, damagePerBullet, criticalDamagePerBullet, criticalRatePerBullet, stagger, isCharge, maxHit, activateActions, hitEffects, ProjectileType.PlayerProjectile);
                if (p is AutoAimProjectileEntity autoAimP) autoAimP.TargetToChase = targetEnemy;
                
                // Có activate action thì ko bắn đạn nữa
                if (activateActions == null || activateActions.Count == 0)
                {
                    p.Activate(delayEachBullet * i);
                    if (LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockNormalAttackPiercing)
                        p.OnHit += () =>
                        {
                            p.Damage = Mathf.RoundToInt(p.Damage *
                                                        LevelUtilityV2.GetNormalPiercingDamageScale());
                        };
                }
                else
                {
                    var activateDirection = (target - spawnPos).normalized;
                    foreach (var activateAction in activateActions)
                    {
                        activateAction.DoAction(p, activateDirection);
                    }
                    
                    ProjectilePool.Instance.Release(p, 0f);
                }
            }
        }
    }
}