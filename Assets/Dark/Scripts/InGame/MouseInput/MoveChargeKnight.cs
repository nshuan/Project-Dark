using System;
using System.Collections.Generic;
using UnityEngine;

namespace InGame
{
    public class MoveChargeKnight : MoveChargeController
    {
        [SerializeField] private int bulletAddedPerUnit = 5;
        
        private float spanAngle = 45f;
        private List<ProjectileEntity> projectiles;
        private Vector2[] directions;
        
        private void Awake()
        {
            projectiles  = new List<ProjectileEntity>();
            directions = new Vector2[50];
        }
        
        public override void AddBullet(Vector2 spawnPos, Vector2 aimDirection)
        {
            for (var i = 0; i < bulletAddedPerUnit; i++)
            {
                var p = ProjectilePool.Instance.Get(projectilePrefab, null, false);
                p.transform.position = spawnPos;
                p.transform.localScale = Vector3.one;
                // Do đây setactive rồi nhng mà chưa set RangeCenter cho projectile nên check trong update bị sai
                // Nên ở đây phải set RangeCenter luoon
                p.RangeCenter = spawnPos;
                p.gameObject.SetActive(false);
                projectiles.Add(p);
            }
            TotalBulletAdded += 1;
        }

        public override void AddSize(float size)
        {
            
        }

        public override void Attack(Action<ProjectileEntity, Vector2, float> actionSetupProjectile)
        {
            const float damageReducePercentageOnBoss = 0.4f;
            
            if (projectiles.Count == 0) return;

            spanAngle = LevelUtilityV2.StatsChargeBullet.range;
            
            var totalTargetBoss = 0;
            if (UseForceDirection)
            {
                RandomUtil.InsideUnitSpanSpacedNonAlloc(ForceDirection, spanAngle, projectiles.Count, ref directions);    
            }
            else
            {
                var camPos = Cam.ScreenToWorldPoint(Input.mousePosition);
                RandomUtil.InsideUnitSpanSpacedNonAlloc(camPos - projectiles[0].transform.position, spanAngle, projectiles.Count, ref directions);
            }
            for (var i = 0; i < projectiles.Count; i++)
            {
                projectiles[i].transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(directions[i].y, directions[i].x) * Mathf.Rad2Deg);
                actionSetupProjectile?.Invoke(projectiles[i], directions[i], i * 0.1f);
                
                // Giảm dame dần nếu target vào boss trong cùng 1 lượt bắn
                if (projectiles[i] is HomingProjectileV2Entity castedProjectile)
                {
                    if (castedProjectile.TargetEnemy && castedProjectile.TargetEnemy.IsBoss)
                    {
                        for (var reduceTime = 0; reduceTime <= totalTargetBoss; reduceTime++)
                        {
                            projectiles[i].Damage -= (int)(damageReducePercentageOnBoss * projectiles[i].Damage);
                        }

                        totalTargetBoss += 1;
                    }
                }
            }
            
            projectiles = new List<ProjectileEntity>();
            TotalBulletAdded = 0;
        }
    }
}