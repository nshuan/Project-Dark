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
        
        public override void Init(Vector2 startPos, Vector2 direction, float maxDistance, float size, float speedScale, int damage,
            int criticalDamage, float criticalRate, float stagger, bool isCharge, int maxHit, List<IProjectileActivate> activateActions, List<IProjectileHit> hitActions, ProjectileType damageType)
        {
            base.Init(startPos, direction, maxDistance, size, speedScale, damage, criticalDamage, criticalRate, stagger, isCharge, maxHit, activateActions, hitActions, damageType);

            canRotate = false;
            blockHit = true;
            transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            
            if (WeaponSupporter.EnemyTargetingIndex < WeaponSupporter.EnemiesCountInRange)
            {
                targetToChase = WeaponSupporter.EnemiesInRange[WeaponSupporter.EnemyTargetingIndex].transform;
                WeaponSupporter.EnemyTargetingIndex += 1;
            }
            
            homingController = GetComponent<TargetedProjectile>();
            if (targetToChase)
                homingController.InitializeProjectile(targetToChase.transform.position, Speed, 0.1f);
            else
                homingController.InitializeProjectile(startPos + direction.normalized * maxDistance, Speed, 0.1f);
            homingController.InitializeAnimationCurve(ProjectileCurveManifest.GetRandomTrajectoryCurve(),
                ProjectileCurveManifest.GetAxisCorrectionCurve(0), ProjectileCurveManifest.GetProjectileSpeedCurve(0));
        }

        protected override IEnumerator IEActivate(float delay)
        {
            yield return new WaitForSeconds(delay);

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
        
        protected override void Update()
        {
            if (!activated && !canRotate) return;
            if (Vector2.Distance(transform.position, RangeCenter) > maxDistance)
            {
                if (!BlockSpawnDeadBody)
                    ProjectileDeadPool.Instance.Get(direction).position = transform.position;
                ProjectileHit(null);
            }

            if (!activated)
            {
                // Change direction slowly to target
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
                        // Nếu trúng con quái này xong là destroy đạn thì ko di chuyển viên đạn nữa, set vị trí vào chỗ con quái luôn
                        if (currentHit + 1 >= MaxHit)
                            transform.position = hitEnemyInfo.hitEnemy.transform.position;
                        else
                            transform.position += moveDirection;
                    
                        ProjectileHit(hitEnemyInfo.hitEnemy);
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
            base.ProjectileHit(hit);
        }
    }
}