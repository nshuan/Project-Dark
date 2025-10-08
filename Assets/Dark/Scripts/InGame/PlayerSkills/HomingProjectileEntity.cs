using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InGame
{
    public class HomingProjectileEntity : ProjectileEntity
    {
        [SerializeField] private AnimationCurve rotateSpeedCurve;
        [SerializeField] private float rotateSpeed; // Degree per seconds
        
        [Space]
        [SerializeField] private AnimationCurve speedCurve;
        [SerializeField] private float timeToReachMaxSpeed = 2f;

        private Transform targetToChase;
        private bool canChase = false;

        public override void Init(Vector2 startPos, Vector2 direction, float maxDistance, float size, float speedScale, int damage,
            int criticalDamage, float criticalRate, float stagger, bool isCharge, int maxHit, List<IProjectileActivate> activateActions, List<IProjectileHit> hitActions, ProjectileType damageType)
        {
            base.Init(startPos, direction, maxDistance, size, speedScale, damage, criticalDamage, criticalRate, stagger, isCharge, maxHit, activateActions, hitActions, damageType);

            if (WeaponSupporter.EnemyTargetingIndex < WeaponSupporter.EnemiesCountInRange)
            {
                targetToChase = WeaponSupporter.EnemiesInRange[WeaponSupporter.EnemyTargetingIndex].transform;
                WeaponSupporter.EnemyTargetingIndex += 1;
            }
        }

        protected override IEnumerator IEActivate(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            activated = true;

            yield return new WaitForSeconds(0.05f);
            canChase = true;
        }
        
        protected override void Update()
        {
            if (!activated) return;
            if (Vector2.Distance(transform.position, RangeCenter) > maxDistance)
            {
                ProjectileDeadPool.Instance.Get(direction).position = transform.position;
                ProjectileHit(null);
            }
            
            // Change direction slowly to target
            if (canChase && targetToChase)
            {
                if (targetToChase.gameObject.activeInHierarchy)
                {
                    direction = Vector3.RotateTowards(direction, targetToChase.position - transform.position,
                        Mathf.Deg2Rad * rotateSpeed * Time.deltaTime, 0f);
                }
            }
            
            transform.position += (Vector3)(Speed * (lifeTime < timeToReachMaxSpeed ? speedCurve.Evaluate(lifeTime / timeToReachMaxSpeed) : 1f) 
                                                  * Time.deltaTime * direction);
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
            //     ProjectileHit(hits[0]);
            // }
        }

        public override void ProjectileHit(EnemyEntity hit)
        {
            targetToChase = null;
            base.ProjectileHit(hit);
        }
    }
}