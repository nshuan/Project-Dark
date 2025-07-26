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

        public override void Init(Vector2 startPos, Vector2 direction, float maxDistance, float size, float speedScale, int damage,
            int criticalDamage, float criticalRate, float stagger, bool isCharge, List<IProjectileHit> hitActions)
        {
            base.Init(startPos, direction, maxDistance, size, speedScale, damage, criticalDamage, criticalRate, stagger, isCharge, hitActions);

            canRotate = false;
            activateDirection = Quaternion.Euler(0f, 0f, Random.Range(-45f, 45f)) * direction * Random.Range(0.8f, 1f);
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
                transform.position += (Vector3)(activateSpeed * Time.deltaTime * activateDirection);
                yield return null;
            }
            
            canRotate = true;

            yield return new WaitForSeconds(0.4f);
            
            activated = true;
        }
        
        protected override void Update()
        {
            if (!activated && !canRotate) return;
            if (Vector2.Distance(transform.position, startPos) > maxDistance)
            {
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
            
            // // Check hit enemy
            // var count = Physics2D.CircleCastNonAlloc(transform.position, DamageHitBoundRadius, Vector2.zero, hits, 0f,
            //     enemyLayer);
            // if (count > 0)
            // {
            //     collider.BlockHit = false;
            //     ProjectileHit(hits[0].transform);
            // }
        }

        public override void ProjectileHit(EnemyEntity hit)
        {
            targetToChase = null;
            base.ProjectileHit(hit);
        }
    }
}