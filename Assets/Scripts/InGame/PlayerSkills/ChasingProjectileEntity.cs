using System.Collections;
using System.Linq;
using UnityEngine;

namespace InGame
{
    public class ChasingProjectileEntity : ProjectileEntity
    {
        [SerializeField] private float rotateSpeed; // Degree per seconds
        private Transform targetToChase;
        
        protected override IEnumerator IEActivate(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            blockHit = true;
            activated = true;

            yield return new WaitForSeconds(0.2f);
            
            FindNextTarget();
        }

        private void FindNextTarget()
        {
            // Find target to chase
            var count = Physics2D.CircleCastNonAlloc(startPos, maxDistance, Vector2.zero, hits, 0f,
                enemyLayer);
            targetToChase = hits[Random.Range(0, count)].transform;
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
            if (targetToChase)
            {
                // direction = Quaternion.Euler(0f, 0f, angle / 2) * dir;
                direction = Vector3.RotateTowards(direction, targetToChase.position - transform.position,
                    Mathf.Deg2Rad * rotateSpeed * Time.deltaTime, 0f);
                
                if (targetToChase.gameObject.activeInHierarchy == false)
                    FindNextTarget();
            }
            
            transform.position += (Vector3)(Speed * Time.deltaTime * direction);
            lifeTime += Time.deltaTime;
            if (lifeTime > MaxLifeTime)
            {
                blockHit = false;
                ProjectileHit(null);
            }
            
            // Check hit enemy
            var count = Physics2D.CircleCastNonAlloc(transform.position, damageRange, Vector2.zero, hits, 0f,
                enemyLayer);
            if (count > 0 && targetToChase && hits.Select((hit => hit.transform)).Contains(targetToChase))
            {
                blockHit = false;
                ProjectileHit(targetToChase);
            }
        }

        protected override void ProjectileHit(Transform hitTransform)
        {
            targetToChase = null;
            base.ProjectileHit(hitTransform);
        }
    }
}