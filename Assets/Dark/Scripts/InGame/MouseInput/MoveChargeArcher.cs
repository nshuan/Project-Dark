using System;
using UnityEngine;

namespace InGame
{
    public class MoveChargeArcher : MoveChargeController
    {
        public override void AddBullet()
        {
            // var p = ProjectilePool.Instance.Get(projectilePrefab, null, false);
            // p.transform.position = spawnPos;
        }

        public override void Attack(Action<ProjectileEntity> actionSetupProjectile)
        {
 
        }
    }
}