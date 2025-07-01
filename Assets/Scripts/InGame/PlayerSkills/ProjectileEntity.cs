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
        private const float MaxLifeTime = 10f;
        
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private float damageRange = 0.1f;
        
        [Space] [Header("Bullet config")]
        [SerializeField] private float baseSpeed = 5f;
        private Vector2 direction;
        private Vector2 startPos;
        private float maxDistance;
        private int Damage { get; set; }
        private int CriticalDamage { get; set; }
        private float CriticalRate { get; set; }
        private float Speed { get; set; }
        private float Stagger { get; set; }

        private bool activated = false;
        private float lifeTime = 0f;

        private RaycastHit2D[] hits = new RaycastHit2D[1];
        
        private void OnDisable()
        {
            activated = false;
            StopAllCoroutines();
        }

        public void Init(
            Vector2 startPos, 
            Vector2 direction, 
            float maxDistance, 
            float speedScale, 
            int damage, 
            int criticalDamage, 
            float criticalRate,
            float stagger)
        {
            Speed = baseSpeed * speedScale;
            this.startPos = startPos;
            this.direction = direction;
            this.maxDistance = maxDistance;
            lifeTime = 0f;
            Damage = damage;
            CriticalDamage = criticalDamage;
            CriticalRate = criticalRate;
            Stagger = stagger;
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
            if (Vector2.Distance(transform.position, startPos) > maxDistance) ProjectileHit(null);
            transform.position += (Vector3)(Speed * Time.deltaTime * direction);
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
                    enemy.Damage(critical ? CriticalDamage : Damage, direction, Stagger);
                   
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