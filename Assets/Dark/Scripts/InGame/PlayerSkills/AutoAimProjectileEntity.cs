using System.Collections.Generic;
using UnityEngine;

namespace InGame
{
    public class AutoAimProjectileEntity : ProjectileEntity
    {
        public EnemyEntity TargetToChase { get; set; }
        private Vector2 targetDirection = new Vector2();

        public override void Init(Vector2 rangeCenter, Vector2 direction, float range, float size, float speedScale, int damage,
            int criticalDamage, float criticalRate, float stagger, bool isCharge, int maxHit, List<IProjectileActivate> activateActions,
            List<IProjectileHit> hitActions, ProjectileType damageType)
        {
            base.Init(rangeCenter, direction, range, size, speedScale, damage, criticalDamage, criticalRate, stagger, isCharge, maxHit, activateActions, hitActions, damageType);
            
            targetDirection.x = direction.x;
            targetDirection.y = direction.y;
            targetDirection.Normalize();
        }

        protected override void FixedUpdate()
        {
            if (!activated) return;

            if (!BlockAutoDestroyOutRange)
            {
                if (Vector2.Distance(transform.position, SpawnPosition) > maxDistanceFromSpawnPosition)
                {
                    if (!BlockSpawnDeadBody)
                    {
                        // ProjectileDeadPool.Instance.Get(direction).position = transform.position;
                        ProjectileDeadPool.Instance.Get(BoundPosition, direction);
                    }
                    ProjectileHit(null);
                    return;
                }
            }

            if (TargetToChase && TargetToChase.Activated && !TargetToChase.IsDestroyed)
            {
                // targetDirection.x = TargetToChase.transform.position.x - transform.position.x;
                // targetDirection.y = TargetToChase.transform.position.y - transform.position.y;
                // targetDirection.Normalize();

                if ((TargetToChase.transform.position.x - transform.position.x) *
                    (TargetToChase.transform.position.x - (transform.position.x + moveDirection.x)) <= 0)
                {
                    if (collider.TryHit(TargetToChase.transform))
                    {
                        DebugUtility.Log($"Hit enemy {TargetToChase}");
                        var deadProjectile = ProjectileDeadOnEnemyPool.Instance.Get(targetDirection);
                        deadProjectile.position = TargetToChase.transform.position;
                        TargetToChase.body.SetupProjectileHit(deadProjectile.transform, targetDirection);
                        deadProjectile.SetParent(TargetToChase.transform);
                        TargetToChase.OnStartDead += () =>
                        {
                            deadProjectile.gameObject.SetActive(false);
                        };
                        ProjectileHit(TargetToChase);
                        TargetToChase = null;
                        return;
                    }
                }
            }
 
            moveDirection.x = Speed * Time.deltaTime * targetDirection.x;
            moveDirection.y = Speed * Time.deltaTime * targetDirection.y;
            moveDirection.z = 0f;
            
            if (!BlockAutoDestroyOutRange && Vector2.Distance(transform.position + moveDirection, SpawnPosition) > maxDistanceFromSpawnPosition)
            {
                if (!BlockSpawnDeadBody)
                {
                    // ProjectileDeadPool.Instance.Get(direction).position = transform.position;
                    ProjectileDeadPool.Instance.Get(BoundPosition, targetDirection);
                }
                ProjectileHit(null);
            }
            else
            {
                hitStatus = collider.CheckCollision(ref moveDirection, ref hitEnemyInfo);
                if (hitStatus == ProjectileCollider.ProjectileHitStatus.Enemy)
                {
                    DebugUtility.Log($"Hit enemy {hitEnemyInfo.hitEnemy}");
                    // Nếu trúng con quái này xong là destroy đạn thì ko di chuyển viên đạn nữa, set vị trí vào chỗ con quái luôn
                    if (!hitEnemyInfo.hitEnemy.IsDestroyed && !forceHideDeadObject && currentHit + 1 >= MaxHit)
                    {
                        transform.position = hitEnemyInfo.hitEnemy.transform.position;
                        
                        var deadProjectile = ProjectileDeadOnEnemyPool.Instance.Get(moveDirection);
                        deadProjectile.position = hitEnemyInfo.hit.point;
                        hitEnemyInfo.hitEnemy.body.SetupProjectileHit(deadProjectile.transform, moveDirection);
                        deadProjectile.SetParent(hitEnemyInfo.hitEnemy.transform);
                        hitEnemyInfo.hitEnemy.OnStartDead += () =>
                        {
                            deadProjectile.gameObject.SetActive(false);
                        };
                    }
                    ProjectileHit(hitEnemyInfo.hitEnemy);
                }
                
                transform.position += moveDirection;
            }
                
            lifeTime += Time.deltaTime;
            if (lifeTime > MaxLifeTime)
            {
                BlockDestroy = false;
                ProjectileHit(null);
            }
        }
    }
}