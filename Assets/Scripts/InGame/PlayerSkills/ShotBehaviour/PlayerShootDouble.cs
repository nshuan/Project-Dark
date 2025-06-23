using System;
using InGame.Pool;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InGame
{
    [Serializable]
    public class PlayerShootDouble : PlayerSkillBehaviour
    {
        [Range(0f, 180f)] public float angle;
        
        public override void Shoot(Vector2 spawnPos, Vector2 target, int damagePerBullet, int numberOfBullets)
        {
            var dir = target - spawnPos;
            var pDir = (Vector2)(Quaternion.Euler(0f, 0f, angle / 2) * dir);
            var p = ProjectilePool.Instance.Get(projectilePrefab, null, false);
            p.transform.position = spawnPos;
            p.Init(5f, spawnPos + pDir, damagePerBullet);
            p.Activate(0);
            
            pDir = Quaternion.Euler(0f, 0f, - angle / 2) * dir;
            p = ProjectilePool.Instance.Get(projectilePrefab, null, false);
            p.transform.position = spawnPos;
            p.Init(5f, spawnPos + pDir, damagePerBullet);
            p.Activate(0);
        }
    }
}