using System;
using Redzen.Structures;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Player/Player Skill Config", fileName = "PlayerSKillConfig")]   
    public class PlayerSkillConfig : SerializedScriptableObject
    {
        public PlayerSkillType skillType;
        public ProjectileEntity projectilePrefab;
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

        public void Shoot(
            Vector2 spawnPos, 
            Vector2 target, 
            int damagePerBullet, 
            int bulletNumber,
            int criticalDamagePerBullet,
            float criticalRatePerBullet)
        {
            shootLogic.Shoot(
                projectilePrefab,
                spawnPos, 
                target,
                damagePerBullet,
                criticalDamagePerBullet,
                criticalRatePerBullet,
                bulletNumber,
                speedScale);
        }
    }

    public enum PlayerSkillType
    {
        
    }
}