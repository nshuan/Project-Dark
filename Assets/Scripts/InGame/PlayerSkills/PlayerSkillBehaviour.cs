using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame
{
    public abstract class PlayerSkillBehaviour
    {
        public ShotCursorType cursorType;

        public abstract void Shoot(
            ProjectileEntity projectilePrefab,
            Vector2 spawnPos, 
            float bulletRange,
            Vector2 target,
            int damagePerBullet, 
            int criticalDamagePerBullet,
            float criticalRatePerBullet,
            int numberOfBullets,
            float skillSize,
            float skillRange,
            float bulletSpeedScale,
            float stagger,
            List<ActionEffectConfig> hitEffects);
    }
}