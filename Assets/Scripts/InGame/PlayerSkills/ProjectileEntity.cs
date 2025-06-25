using System;
using System.Collections;
using InGame.Pool;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame
{
    public class ProjectileEntity : MonoBehaviour
    {
        [SerializeField] private LayerMask enemyLayer;
        private const float MaxLifeTime = 5f;
        private float speed = 5f;
        [SerializeField] private float damageRange = 0.1f;
        private Vector2 direction;
        private Vector2 target;
        private int Damage { get; set; }
        private int CriticalDamage { get; set; }
        private float CriticalRate { get; set; }

        private bool activated = false;
        private float lifeTime = 0f;

        private RaycastHit2D[] hits = new RaycastHit2D[1];
        
        private void OnDisable()
        {
            activated = false;
            StopAllCoroutines();
        }

        public void Init(float spe, Vector2 targetPos, int damage, int criticalDamage, float criticalRate)
        {
            speed = spe;
            target = targetPos;
            direction = (target - (Vector2)transform.position).normalized;
            lifeTime = 0f;
            Damage = damage;
            CriticalDamage = criticalDamage;
            CriticalRate = criticalRate;
        }

        public void Activate(float delay)
        {
            gameObject.SetActive(true);
            StartCoroutine(IEActivate(delay));
        }

        private IEnumerator IEActivate(float delay)
        {
            yield return new WaitForSeconds(delay);
            activated = true;
        }

        private void Update()
        {
            if (!activated) return;
            if (Vector2.Distance(transform.position, target) < 0.1f) ProjectileHit(null);
            transform.position += (Vector3)(speed * Time.deltaTime * direction);
            lifeTime += Time.deltaTime;
            if (lifeTime > MaxLifeTime) Destroy(gameObject);
            
            // Check hit enemy
            var count = Physics2D.CircleCastNonAlloc(transform.position, damageRange, Vector2.zero, hits, 0f,
                enemyLayer);
            if (count > 0)
                ProjectileHit(hits[0].transform);
        }

        protected virtual void ProjectileHit(Transform hitTransform)
        {
            if (hitTransform)
            {
                if (hitTransform.TryGetComponent<EnemyEntity>(out var enemy))
                {
                    // Check critical hit
                    var critical = Random.Range(0f, 1f) <= CriticalRate;
                    enemy.OnHit(critical ? CriticalDamage : Damage);
                   
                    if (critical)
                        DebugUtility.LogWarning($"Projectile {name} deals critical damage {CriticalDamage} to {enemy.name}!!");
                }
            }
            
            ProjectilePool.Instance.Release(this);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, damageRange);
        }
    }
}