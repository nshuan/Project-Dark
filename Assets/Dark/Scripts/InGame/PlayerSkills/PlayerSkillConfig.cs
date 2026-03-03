using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Player/Player Skill Config", fileName = "PlayerSKillConfig")]   
    public class PlayerSkillConfig : SerializedScriptableObject
    {
        public int skillId;
        public Dictionary<PlayerProjectileType, ProjectileEntity> projectiles;
        [NonSerialized, OdinSerialize] public PlayerSkillBehaviour shootLogic;
        public int damePerBullet; // bullet base damage
        public int numberOfBullets = 1; // number of bullets in each shot
        public float cooldown = 0.5f;  // time between shots
        public float range = 5f; // Max damage range, also max distance from player to mouse aimìng position
        public float size; // size of the aiming field
        public float chargeCooldown = 0.5f;
        public float chargeDameStep = 0;
        public int chargeDameMaxStep = 0;
        public float chargeSizeStep = 0;
        public int chargeSizeMaxStep = 0;
        public float chargeRangeStep = 0;
        public int chargeRangeMaxStep = 0;
        public float chargeBulletStep = 0;
        public int chargeBulletMaxStep = 0;
        public float chargeStepTime = 1f;
        public float speedScale = 1f; // Scale speed of bullets
        public float stagger; // Push enemy away when projectile hit

        public void Shoot(
            ProjectileEntity projectile,
            Vector2 spawnPos, 
            Vector2 rangeCenter,
            Vector2 target, 
            int damagePerBullet, 
            int bulletNumber,
            float skillSize,
            float skillRange,
            int criticalDamagePerBullet,
            float criticalRatePerBullet,
            float stagger,
            int maxHit,
            bool isCharge,
            List<IProjectileActivate> activateActions,
            List<IProjectileHit> hitActions)
        {
            shootLogic.Shoot(
                projectile,
                spawnPos, 
                rangeCenter,
                target,
                damagePerBullet,
                criticalDamagePerBullet,
                criticalRatePerBullet,
                bulletNumber,
                skillSize,
                skillRange,
                speedScale,
                stagger,
                maxHit,
                isCharge,
                activateActions,
                hitActions);
        }
        
        public void ShootToTarget(
            ProjectileEntity projectile,
            EnemyEntity targetEnemy,
            bool isTargetEnemyForceSelect,
            Vector2 spawnPos, 
            Vector2 rangeCenter,
            Vector2 target, 
            int damagePerBullet, 
            int bulletNumber,
            float skillSize,
            float skillRange,
            int criticalDamagePerBullet,
            float criticalRatePerBullet,
            float stagger,
            int maxHit,
            bool isCharge,
            List<IProjectileActivate> activateActions,
            List<IProjectileHit> hitActions)
        {
            shootLogic.ShootWithTarget(
                projectile,
                targetEnemy,
                isTargetEnemyForceSelect,
                spawnPos, 
                rangeCenter,
                target,
                damagePerBullet,
                criticalDamagePerBullet,
                criticalRatePerBullet,
                bulletNumber,
                skillSize,
                skillRange,
                speedScale,
                stagger,
                maxHit,
                isCharge,
                activateActions,
                hitActions);
        }
    }

    public enum PlayerProjectileType
    {
        Normal,
        ChargeBullet,
        ChargeSize,
        ChargeBulletSize,
        ChargeSizeSubBullet
    }
}