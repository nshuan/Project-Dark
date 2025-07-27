using System;
using UnityEngine;

namespace InGame
{
    [Serializable]
    public class TowerCounterPiercing : ITowerCounterLogic
    {
        [SerializeField] private ProjectileEntity projectilePrefab;
        
        public void Counter(Vector2 towerAttackPos, Vector2 direction, int damage, float speedScale)
        {
            var projectile = ProjectilePool.Instance.Get(projectilePrefab, null, false);
            projectile.transform.position = towerAttackPos;
            projectile.Init(towerAttackPos, direction.normalized, 20, 5, speedScale, damage, damage, 0f, 0f, false, 10, null, null);
            projectile.BlockDestroy = true;
            projectile.Activate(0f);
        }
    }
}