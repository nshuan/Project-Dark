using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame
{
    public class ProjectileEntity : MonoBehaviour
    {
        protected const float MaxLifeTime = 10f;

        [SerializeField] protected ProjectileCollider collider;
        [SerializeField] protected LayerMask enemyLayer;
        [SerializeField] private float baseDamageRange = 0.1f;
        
        [Space] [Header("Bullet config")]
        [SerializeField] private float baseSpeed = 5f;
        protected Vector2 direction;
        protected Vector2 startPos;
        public float maxDistance;
        public int Damage { get; set; }
        public float Size { get; set; }
        public float SpeedScale { get; set; }
        public bool IsCharge { get; set; }
        public float DamageHitBoundRadius { get; set; } = 1f;
        public int CriticalDamage { get; set; }
        public float CriticalRate { get; set; }
        public float Speed { get; set; }
        public float Stagger { get; set; }
        public int MaxHit { get; set; } = 1;
        public List<IProjectileActivate> ActivateActions { get; set; }
        public List<IProjectileHit> HitActions { get; set; }
        public bool BlockDestroy { get; set; } // Block destroy so that the projectile can go through enemies but still deal damage
        public bool BlockSpawnDeadBody { get; set; } // Do not spawn dead projectile on hit
        
        public Transform TargetTransform => transform;

        private int currentHit;
        protected bool activated = false;
        protected float lifeTime = 0f;

        protected RaycastHit2D[] hits = new RaycastHit2D[1];

        #region Actions

        public Action OnHit;

        #endregion

        private void Awake()
        {
            collider.Projectile = this;
        }

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
            int maxHit,
            List<IProjectileActivate> activateActions,
            List<IProjectileHit> hitActions)
        {
            Size = size;
            transform.localScale = size * Vector3.one;
            SpeedScale = speedScale;
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
            ActivateActions = activateActions;
            HitActions = hitActions;
            MaxHit = maxHit;
            currentHit = 0;
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
            if (ActivateActions != null)
            {
                foreach (var action in ActivateActions)
                {
                    action.DoAction(this, direction);
                }
            }
        }

        protected virtual void Update()
        {
            if (!activated) return;
            if (Vector2.Distance(transform.position, startPos) > maxDistance)
            {
                if (!BlockSpawnDeadBody)
                    ProjectileDeadPool.Instance.Get(direction).position = transform.position;
                ProjectileHit(null);
            }
            transform.position += (Vector3)(Speed * Time.deltaTime * direction);
            lifeTime += Time.deltaTime;
            if (lifeTime > MaxLifeTime)
            {
                BlockDestroy = false;
                ProjectileHit(null);
            }
            
            // Check hit enemy
            // var count = Physics2D.CircleCastNonAlloc(transform.position, DamageHitBoundRadius, Vector2.zero, hits, 0f,
            //     enemyLayer);
            // if (count > 0)
            //     ProjectileHit(hits[0].transform);
        }
        
        public virtual void ProjectileHit(EnemyEntity hit)
        {
            if (!hit)
            {
                DebugUtility.Log("null");
                BlockDestroy = false;
                BlockSpawnDeadBody = false;
                OnHit = null;
                lifeTime = 0f;
                activated = false;
                ProjectilePool.Instance.Release(this);
                return;
            }

            if (hit.State == EnemyState.Invisible)
            {
                DebugUtility.Log("Invisible");
                return;
            }
            
            // Check critical hit
            var critical = Random.Range(0f, 1f) <= CriticalRate;
            hit.HitDirectionX = direction.x;
            hit.HitDirectionY = direction.y;
            hit.Damage(critical ? CriticalDamage : Damage, transform.position, Stagger);
            PassiveEffectManager.Instance.TriggerEffect(IsCharge ? PassiveTriggerType.DameByChargeAttack : PassiveTriggerType.DameByNormalAttack, hit);
                    
            DebugUtility.Log("hit");
            if (critical)
                DebugUtility.LogWarning($"Projectile {name} deals critical damage {CriticalDamage} to {hit.name}!!");

            if (HitActions != null)
            {
                foreach (var action in HitActions)
                {
                    action.DoAction(transform.position);
                }
            }
                    
            OnHit?.Invoke();
            currentHit += 1;
            
            if (!BlockDestroy && currentHit >= MaxHit)
            {
                BlockDestroy = false;
                BlockSpawnDeadBody = false;
                OnHit = null;
                lifeTime = 0f;
                activated = false;
                ProjectilePool.Instance.Release(this);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, baseDamageRange);
        }
    }
}