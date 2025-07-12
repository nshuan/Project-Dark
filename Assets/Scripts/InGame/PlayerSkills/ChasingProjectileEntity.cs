using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InGame
{
    public class ChasingProjectileEntity : ProjectileEntity
    {
        [SerializeField] private float rotateSpeed; // Degree per seconds
        private Transform targetToChase;
        private bool canChase = false;

        public override void Init(Vector2 startPos, Vector2 direction, float maxDistance, float size, float speedScale, int damage,
            int criticalDamage, float criticalRate, float stagger, bool isCharge, List<IProjectileHit> hitActions)
        {
            base.Init(startPos, direction, maxDistance, size, speedScale, damage, criticalDamage, criticalRate, stagger, isCharge, hitActions);

            if (WeaponSupporter.EnemyTargetingIndex < WeaponSupporter.EnemiesCountInRange)
            {
                targetToChase = WeaponSupporter.EnemiesInRange[WeaponSupporter.EnemyTargetingIndex].transform;
                WeaponSupporter.EnemyTargetingIndex += 1;
            }
        }

        protected override IEnumerator IEActivate(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            blockHit = true;
            activated = true;

            yield return new WaitForSeconds(0.1f);
            canChase = true;
        }
        
        protected override void Update()
        {
            if (!activated) return;
            if (Vector2.Distance(transform.position, startPos) > maxDistance)
            {
                blockHit = false;
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
            
            transform.position += (Vector3)(Speed * Time.deltaTime * direction);
            lifeTime += Time.deltaTime;
            if (lifeTime > MaxLifeTime)
            {
                blockHit = false;
                ProjectileHit(null);
            }
            
            // Check hit enemy
            var count = Physics2D.CircleCastNonAlloc(transform.position, DamageHitBoundRadius, Vector2.zero, hits, 0f,
                enemyLayer);
            if (count > 0)
            {
                blockHit = false;
                ProjectileHit(hits[0].transform);
            }
        }

        protected override void ProjectileHit(Transform hitTransform)
        {
            targetToChase = null;
            base.ProjectileHit(hitTransform);
        }
    }
}