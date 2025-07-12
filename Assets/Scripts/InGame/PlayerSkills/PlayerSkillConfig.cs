using System;
using System.Collections.Generic;
using Redzen.Structures;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Player/Player Skill Config", fileName = "PlayerSKillConfig")]   
    public class PlayerSkillConfig : SerializedScriptableObject
    {
        public int skillId;
        public PlayerSkillType skillType;
        public ProjectileEntity projectilePrefab;
        public ProjectileEntity chargeProjectilePrefab;
        [NonSerialized, OdinSerialize] public PlayerSkillBehaviour shootLogic;
        public int damePerBullet; // bullet base damage
        public int numberOfBullets = 1; // number of bullets in each shot
        public float cooldown = 0.5f;  // time between shots
        public float range = 5f; // Max damage range, also max distance from player to mouse aimìng position
        public float size; // size of the aiming field
        public float chargeDameMax = -1;
        public float chargeDameTime = -1;
        public float chargeSizeMax = -1;
        public float chargeSizeTime = -1;
        public float chargeRangeMax = -1;
        public float chargeRangeTime = -1;
        public float chargeBulletInterval = -1;
        public int chargeBulletMaxAdd = -1;
        public float speedScale = 1f; // Scale speed of bullets
        public float stagger; // Push enemy away when projectile hit

        public void Shoot(
            ProjectileEntity projectile,
            Vector2 spawnPos, 
            Vector2 target, 
            int damagePerBullet, 
            int bulletNumber,
            float skillSize,
            float skillRange,
            int criticalDamagePerBullet,
            float criticalRatePerBullet,
            bool isCharge,
            List<IProjectileHit> hitActions)
        {
            shootLogic.Shoot(
                projectile,
                spawnPos, 
                target,
                damagePerBullet,
                criticalDamagePerBullet,
                criticalRatePerBullet,
                bulletNumber,
                skillSize,
                skillRange,
                speedScale,
                stagger,
                isCharge,
                hitActions);
        }
    }

    public enum PlayerSkillType
    {
        
    }
}