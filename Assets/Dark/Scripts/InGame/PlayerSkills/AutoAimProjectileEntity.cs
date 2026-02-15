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
            moveDirection.z = 0;
        }

        protected override void FixedUpdate()
        {
            if (!activated) return;

            if (!BlockAutoDestroyOutRange)
            {
                if (Vector2.Distance(transform.position, SpawnPosition) > maxDistanceFromSpawnPosition)
                {
                    if (!BlockSpawnDeadBody && !forceHideDeadObject)
                    {
                        // ProjectileDeadPool.Instance.Get(direction).position = transform.position;
                        ProjectileDeadPool.Instance.Get(BoundPosition, direction);
                    }
                    ProjectileHit(null);
                    return;
                }
            }

            var flagOutOfRange = false;

            moveDirection.x = Speed * Time.deltaTime * targetDirection.x;
            moveDirection.y = Speed * Time.deltaTime * targetDirection.y;
            
            if (Vector2.Distance(transform.position + moveDirection, SpawnPosition) > maxDistanceFromSpawnPosition)
            {
                moveDirection.x = BoundPosition.x - transform.position.x;
                moveDirection.y = BoundPosition.y - transform.position.y;
                flagOutOfRange = true;
            }
            
            if (TargetToChase && TargetToChase.Activated && !TargetToChase.IsDestroyed)
            {
                if ((TargetToChase.transform.position.x - transform.position.x) *
                    (TargetToChase.transform.position.x - (transform.position.x + moveDirection.x)) <= 0)
                {
                    if (collider.TryHit(TargetToChase.transform))
                    {
                        DebugUtility.Log($"Hit enemy {TargetToChase}");
                        if (!TargetToChase.IsDestroyed && !BlockSpawnDeadBody && !forceHideDeadObject && currentHit + 1 >= MaxHit)
                        {
                            var deadProjectile = ProjectileDeadOnEnemyPool.Instance.Get(targetDirection);
                            deadProjectile.position = TargetToChase.transform.position;
                            TargetToChase.body.SetupProjectileHit(deadProjectile.transform, targetDirection);
                            deadProjectile.SetParent(TargetToChase.transform);
                            TargetToChase.OnStartDead += (dead) =>
                            {
                                deadProjectile.gameObject.SetActive(false);
                            };
                        }
                        ProjectileHit(TargetToChase);
                        TargetToChase = null;
                    }
                }
            }
            
            // CheckHitEnemyNotTargeted
            hitStatus = collider.CheckCollision(ref moveDirection, ref hitEnemyInfo);
            if (hitStatus == ProjectileCollider.ProjectileHitStatus.Enemy)
            {
                DebugUtility.Log($"Hit enemy {hitEnemyInfo.hitEnemy}");
                // Nếu trúng con quái này xong là destroy đạn thì ko di chuyển viên đạn nữa, set vị trí vào chỗ con quái luôn
                if (!hitEnemyInfo.hitEnemy.IsDestroyed && !BlockSpawnDeadBody && !forceHideDeadObject && currentHit + 1 >= MaxHit)
                {
                    transform.position = hitEnemyInfo.hitEnemy.transform.position;
                        
                    var deadProjectile = ProjectileDeadOnEnemyPool.Instance.Get(moveDirection);
                    deadProjectile.position = hitEnemyInfo.hit.point;
                    hitEnemyInfo.hitEnemy.body.SetupProjectileHit(deadProjectile.transform, moveDirection);
                    deadProjectile.SetParent(hitEnemyInfo.hitEnemy.transform);
                    hitEnemyInfo.hitEnemy.OnStartDead += (dead) =>
                    {
                        deadProjectile.gameObject.SetActive(false);
                    };
                }
                ProjectileHit(hitEnemyInfo.hitEnemy);
            }
                
            transform.position += moveDirection;
                
            if (flagOutOfRange && !BlockAutoDestroyOutRange)
            {
                if (!BlockSpawnDeadBody && !forceHideDeadObject)
                {
                    // ProjectileDeadPool.Instance.Get(direction).position = transform.position;
                    ProjectileDeadPool.Instance.Get(BoundPosition, targetDirection);
                }
                ProjectileHit(null);
                return;
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