using System;
using System.Collections;
using System.Collections.Generic;
using InGame.ProjectileCustomPath;
using UnityEngine;

namespace InGame
{
    [RequireComponent(typeof(TargetedProjectile))]
    public class HomingProjectileV2Entity : ProjectileEntity
    {
        [Space]
        [SerializeField] private float activateTime = 1f;
        [SerializeField] private float activateSpeed = 2f;
        [SerializeField] private float rotateSpeed; // Degree per seconds
        
        private TargetedProjectile homingController;

        private Vector2 activateDirection;
        private Transform targetToChase;
        private Vector3 lastTargetPosition;
        private bool canRotate = false;
        private bool blockHit;

        private EnemyEntity targetEnemy;
        public EnemyEntity TargetEnemy => targetEnemy;
        
        public override void Init(Vector2 startPos, Vector2 direction, float range, float size, float speedScale, int damage,
            int criticalDamage, float criticalRate, float stagger, bool isCharge, int maxHit, List<IProjectileActivate> activateActions, List<IProjectileHit> hitActions, ProjectileType damageType)
        {
            base.Init(startPos, direction, range, size, speedScale, damage, criticalDamage, criticalRate, stagger, isCharge, maxHit, activateActions, hitActions, damageType);

            transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            canRotate = false;
            blockHit = true;
            
            if (WeaponSupporter.EnemyTargetingIndex < WeaponSupporter.EnemiesCountInRange)
            {
                targetToChase = WeaponSupporter.EnemiesInRange[WeaponSupporter.EnemyTargetingIndex].transform;
                // Nếu là boss thì ko tăng target index
                if (!targetToChase.TryGetComponent<EnemyEntity>(out targetEnemy) || !targetEnemy.IsBoss)
                    WeaponSupporter.EnemyTargetingIndex += 1;
            }
            
            homingController = GetComponent<TargetedProjectile>();
            if (targetToChase)
                homingController.InitializeProjectile(targetToChase.transform.position, Speed, 0.15f);
            else
                homingController.InitializeProjectile(RangeCenter + BoundPosition, Speed, 0.15f);
            homingController.InitializeAnimationCurve(ProjectileCurveManifest.GetRandomTrajectoryCurve(),
                ProjectileCurveManifest.GetAxisCorrectionCurve(0), ProjectileCurveManifest.GetProjectileSpeedCurve(0));
        }

        protected override IEnumerator IEActivate(float delay)
        {
            yield return new WaitForSeconds(delay);

            var activateVfx = ProjectileVfxActivatePool.Instance.Get(transform, true);
            activateVfx.transform.localPosition = Vector3.zero;
            activateVfx.transform.localRotation = Quaternion.identity;
            activateVfx.Activate((vfx) => ProjectileVfxActivatePool.Instance.Release(vfx));

            canRotate = true;
            activated = true;
            blockHit = false;
            collider.CanTrigger = true;
            
            if (ActivateActions != null)
            {
                foreach (var action in ActivateActions)
                {
                    action.DoAction(this, direction);
                }
            }
        }
        
        protected override void FixedUpdate()
        {
            if (!activated && !canRotate) return;

            if (!activated)
            {
                // Change projectile's direction slowly to target
                if (canRotate && targetToChase)
                {
                    if (targetToChase.gameObject.activeInHierarchy)
                    {
                        direction = Vector3.RotateTowards(direction, targetToChase.position - transform.position,
                            Mathf.Deg2Rad * rotateSpeed * Time.deltaTime, 0f);
                        lastTargetPosition = targetToChase.position;
                    }
                }
                
                transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            }
            else
            {
                if (targetToChase)
                {
                    if (targetToChase.gameObject.activeInHierarchy)
                        lastTargetPosition = targetToChase.position;
                    moveDirection = homingController.GetProjectileNextPosition(lastTargetPosition) - transform.position;
                    transform.rotation = homingController.GetProjectileNextRotation();
                    hitStatus = collider.CheckCollision(ref moveDirection, ref hitEnemyInfo);
                    if (hitStatus == ProjectileCollider.ProjectileHitStatus.Enemy && ReferenceEquals(hitEnemyInfo.hitEnemy.transform, targetToChase))
                    {
                        DebugUtility.Log($"Homing Hit enemy {hitEnemyInfo.hitEnemy}");
                        transform.position = hitEnemyInfo.hitEnemy.transform.position;

                        if (!hitEnemyInfo.hitEnemy.IsDestroyed && !forceHideDeadObject && currentHit + 1 >= MaxHit)
                        {
                            var deadProjectile = ProjectileDeadOnEnemyPool.Instance.Get(moveDirection);
                            deadProjectile.position = hitEnemyInfo.hit.point;
                            hitEnemyInfo.hitEnemy.body.SetupProjectileHit(deadProjectile.transform, moveDirection);
                            hitEnemyInfo.hitEnemy.OnStartDead += () =>
                            {
                                deadProjectile.gameObject.SetActive(false);
                            };
                        }
                        ProjectileHit(hitEnemyInfo.hitEnemy);
                    }
                    else
                    {
                        if (Vector2.Distance(transform.position + moveDirection, RangeCenter) > LevelUtility.GetRelativeRange(Range, transform.position - RangeCenter))
                        {
                            if (!BlockSpawnDeadBody)
                                ProjectileHomingDeadPool.Instance.Get(transform.position, direction);
                            ProjectileHit(null);
                        }
                        else
                        {
                            transform.position += moveDirection;
                        }
                    }
                }
                else
                {
                    moveDirection = homingController.GetProjectileNextPosition(BoundPosition) - transform.position;
                    transform.rotation = homingController.GetProjectileNextRotation();
                    
                    if (Vector2.Distance(transform.position + moveDirection, RangeCenter) > LevelUtility.GetRelativeRange(Range, transform.position - RangeCenter))
                    {
                        if (!BlockSpawnDeadBody)
                            ProjectileHomingDeadPool.Instance.Get(transform.position, direction);
                        ProjectileHit(null);
                    }
                    else
                    {
                        transform.position += moveDirection;
                    }
                }
            }
            
            lifeTime += Time.deltaTime;
            if (lifeTime > MaxLifeTime)
            {
                ProjectileHit(null);
            }
        }

        public override void ProjectileHit(EnemyEntity hit)
        {
            if (blockHit) return;
            targetToChase = null;
            currentHit = MaxHit;
            base.ProjectileHit(hit);
        }
        
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (targetToChase != null)
                Gizmos.DrawLine(transform.position, targetToChase.position);
        }
    }
}