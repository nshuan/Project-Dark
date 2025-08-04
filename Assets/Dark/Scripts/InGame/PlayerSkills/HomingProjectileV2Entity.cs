using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame
{
    public class HomingProjectileV2Entity : ProjectileEntity
    {
        [Space]
        [SerializeField] private float activateTime = 1f;
        [SerializeField] private float activateSpeed = 2f;
        [SerializeField] private float rotateSpeed; // Degree per seconds
        
        [Space]
        [SerializeField] private AnimationCurve speedCurve;
        [SerializeField] private float timeToReachMaxSpeed = 2f;

        private Vector2 activateDirection;
        private float activateTimeCounter;
        private Transform targetToChase;
        private bool canRotate = false;
        private bool blockHit;

        public override void Init(Vector2 startPos, Vector2 direction, float maxDistance, float size, float speedScale, int damage,
            int criticalDamage, float criticalRate, float stagger, bool isCharge, int maxHit, List<IProjectileActivate> activateActions, List<IProjectileHit> hitActions)
        {
            base.Init(startPos, direction, maxDistance, size, speedScale, damage, criticalDamage, criticalRate, stagger, isCharge, maxHit, activateActions, hitActions);

            canRotate = false;
            blockHit = true;
            activateDirection = Quaternion.Euler(0f, 0f, Random.Range(-45f, 45f)) * direction * Random.Range(0.8f, 1f);
            transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(activateDirection.y, activateDirection.x) * Mathf.Rad2Deg);
            activateTimeCounter = activateTime;
            
            if (WeaponSupporter.EnemyTargetingIndex < WeaponSupporter.EnemiesCountInRange)
            {
                targetToChase = WeaponSupporter.EnemiesInRange[WeaponSupporter.EnemyTargetingIndex].transform;
                WeaponSupporter.EnemyTargetingIndex += 1;
            }
        }

        protected override IEnumerator IEActivate(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            while (activateTimeCounter > 0)
            {
                activateTimeCounter -= Time.deltaTime;
                // if (targetToChase && targetToChase.gameObject.activeInHierarchy)
                // {
                //     activateDirection = Vector3.RotateTowards(activateDirection, targetToChase.position - transform.position,
                //         Mathf.Deg2Rad * 90 * Time.deltaTime, 0f);
                // }
                // transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(activateDirection.y, activateDirection.x) * Mathf.Rad2Deg);
                transform.position += (Vector3)(activateSpeed * Time.deltaTime * activateDirection);
                yield return null;
            }

            direction.x = activateDirection.x;
            direction.y = activateDirection.y;
            canRotate = true;

            yield return new WaitForSeconds(0.2f);
            
            activated = true;
            blockHit = false;
        }
        
        protected override void Update()
        {
            if (!activated && !canRotate) return;
            if (Vector2.Distance(transform.position, startPos) > maxDistance)
            {
                ProjectileDeadPool.Instance.Get(direction).position = transform.position;
                ProjectileHit(null);
            }
            
            // Change direction slowly to target
            if (canRotate && targetToChase)
            {
                if (targetToChase.gameObject.activeInHierarchy)
                {
                    direction = Vector3.RotateTowards(direction, targetToChase.position - transform.position,
                        Mathf.Deg2Rad * rotateSpeed * Time.deltaTime, 0f);
                }
            }
            
            transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

            if (activated)
            {
                transform.position += (Vector3)(Speed * (lifeTime < timeToReachMaxSpeed ? speedCurve.Evaluate(lifeTime / timeToReachMaxSpeed) : 1f) 
                                                      * Time.deltaTime * direction);
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