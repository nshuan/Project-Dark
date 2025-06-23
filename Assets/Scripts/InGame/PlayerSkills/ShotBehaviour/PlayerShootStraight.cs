using System;
using InGame.Pool;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InGame
{
    [Serializable]
    public class PlayerShootStraight : PlayerSkillBehaviour
    {
        public override void Shoot(Vector2 spawnPos, Vector2 target)
        {
            var p = ProjectilePool.Instance.Get(projectilePrefab, null, false);
            p.transform.position = spawnPos;
            p.Init(5f, target);
            p.Activate();
        }
    }
}