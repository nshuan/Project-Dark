using System;
using System.Collections;
using System.Collections.Generic;
using InGame.Pool;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame
{
    public class ProjectileEntity : MonoBehaviour
    {
        protected const float MaxLifeTime = 10f;

        [SerializeField] private Transform visual;
        [SerializeField] protected LayerMask enemyLayer;
        [SerializeField] private float baseDamageRange = 0.1f;
        
        [Space] [Header("Bullet config")]
        [SerializeField] private float baseSpeed = 5f;
        protected Vector2 direction;
        protected Vector2 startPos;
        protected float maxDistance;
        private int Damage { get; set; }
        private bool IsCharge { get; set; }
        protected float DamageHitBoundRadius { get; set; } = 1f;
        private int CriticalDamage { get; set; }
        private float CriticalRate { get; set; }
        protected float Speed { get; set; }
        private float Stagger { get; set; }
        private List<IProjectileHit> HitActions { get; set; }
        public bool BlockDestroy { get; set; } // Block destroy so that the projectile can go through enemies but still deal damage

        public Transform TargetTransform => transform;
        
        protected bool blockHit = false;
        protected bool activated = false;
        protected float lifeTime = 0f;
        protected EnemyEntity hitEnemy;

        protected RaycastHit2D[] hits = new RaycastHit2D[1];

        #region Actions

        public Action OnHit;

        #endregion
        
        private void OnDisable()
        {
            activated = false;
            StopAllCoroutines();
        }

        public virtual void Init(
            Vector2 startPos, 
            Vector2 direction, 
            float maxDistance, 
            float size,
            float speedScale, 
            int damage, 
            int criticalDamage, 
            float criticalRate,
            float stagger,
            bool isCharge,
            List<IProjectileHit> hitActions)
        {
            visual.localScale = size * Vector3.one;
            Speed = baseSpeed * speedScale;
            this.startPos = startPos;
            this.direction = direction;
            this.maxDistance = maxDistance;
            lifeTime = 0f;
            Damage = damage;
            DamageHitBoundRadius = baseDamageRange * size;
            CriticalDamage = criticalDamage;
            CriticalRate = criticalRate;
            Stagger = stagger;
            IsCharge = isCharge;
            HitActions = hitActions;
        }

        public void Activate(float delay)
        {
            gameObject.SetActive(true);
            StartCoroutine(IEActivate(delay));
        }

        protected virtual IEnumerator IEActivate(float delay)
        {
            yield return new WaitForSeconds(delay);
            activated = true;
        }

        protected virtual void Update()
        {
            if (!activated) return;
            if (Vector2.Distance(transform.position, startPos) > maxDistance) ProjectileHit(null);
            transform.position += (Vector3)(Speed * Time.deltaTime * direction);
            lifeTime += Time.deltaTime;
            if (lifeTime > MaxLifeTime)
            {
                blockHit = false;
                BlockDestroy = false;
                ProjectileHit(null);
            }
            
            // Check hit enemy
            var count = Physics2D.CircleCastNonAlloc(transform.position, DamageHitBoundRadius, Vector2.zero, hits, 0f,
                enemyLayer);
            if (count > 0)
                ProjectileHit(hits[0].transform);
        }
        
        protected virtual void ProjectileHit(Transform hitTransform)
        {
            if (blockHit) return;
            if (hitTransform)
            {
                if (hitTransform.TryGetComponent<EnemyEntity>(out hitEnemy))
                {
                    // Check critical hit
                    var critical = Random.Range(0f, 1f) <= CriticalRate;
                    hitEnemy.Damage(critical ? CriticalDamage : Damage, transform.position, Stagger);
                    PassiveEffectManager.Instance.TriggerEffect(IsCharge ? PassiveTriggerType.DameByChargeAttack : PassiveTriggerType.DameByNormalAttack, hitEnemy);
                    
                    if (critical)
                        DebugUtility.LogWarning($"Projectile {name} deals critical damage {CriticalDamage} to {hitEnemy.name}!!");

                    if (HitActions != null)
                    {
                        foreach (var action in HitActions)
                        {
                            action.DoAction(transform.position);
                        }
                    }
                    
                    OnHit?.Invoke();
                    if (!BlockDestroy)
                        OnHit = null;
                }
            }
            
            if (!BlockDestroy)
                ProjectilePool.Instance.Release(this);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, baseDamageRange);
        }
    }
}