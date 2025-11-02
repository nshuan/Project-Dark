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
        [SerializeField] private float baseDamageRange = 0.1f;
        
        [Space] [Header("Bullet config")]
        [SerializeField] private float baseSpeed = 5f;
        protected Vector2 direction;
        public Vector2 RangeCenter { get; set; }
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
        public ProjectileType DamageType { get; set; }
        public bool BlockDestroy { get; set; } // Block destroy so that the projectile can go through enemies but still deal damage
        public bool BlockSpawnDeadBody { get; set; } // Do not spawn dead projectile on hit
        
        public Transform TargetTransform => transform;

        protected int currentHit;
        protected bool activated = false;
        protected float lifeTime = 0f;
        protected Vector3 moveDirection = Vector3.zero;
        protected ProjectileCollider.ProjectileHitStatus hitStatus;

        protected RaycastHit2D[] hits = new RaycastHit2D[1];
        protected ProjectileCollider.HitEnemyInfo hitEnemyInfo;
        
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
            Vector2 rangeCenter,
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
            List<IProjectileHit> hitActions,
            ProjectileType damageType)
        {
            Size = size;
            transform.localScale = size * Vector3.one;
            SpeedScale = speedScale;
            Speed = baseSpeed * speedScale;
            this.RangeCenter = rangeCenter;
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
            DamageType = damageType;

            hitStatus = ProjectileCollider.ProjectileHitStatus.None;
            hitEnemyInfo = new ProjectileCollider.HitEnemyInfo();
            collider.Init();
            collider.UpdateLastPosition(transform.position);
        }

        public void Activate(float delay)
        {
            gameObject.SetActive(true);
            StartCoroutine(IEActivate(delay));
        }

        protected virtual IEnumerator IEActivate(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            // Khi vừa activate đạn thì check luôn tại vị trí spawn, bán kính [x] để xử lý những enemy ở quá gân
            collider.CheckHitEnemiesOnInit();
            
            activated = true;
            collider.CanTrigger = true;
            
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
            if (Vector2.Distance(transform.position, RangeCenter) > maxDistance)
            {
                if (!BlockSpawnDeadBody)
                    ProjectileDeadPool.Instance.Get(direction).position = transform.position;
                ProjectileHit(null);
            }

            moveDirection.x = Speed * Time.deltaTime * direction.x;
            moveDirection.y = Speed * Time.deltaTime * direction.y;
            moveDirection.z = 0f;
            hitStatus = collider.CheckCollision(ref moveDirection, ref hitEnemyInfo);
            if (hitStatus == ProjectileCollider.ProjectileHitStatus.Enemy)
            {
                DebugUtility.Log($"Hit enemy {hitEnemyInfo.hitEnemy}");
                // Nếu trúng con quái này xong là destroy đạn thì ko di chuyển viên đạn nữa, set vị trí vào chỗ con quái luôn
                if (currentHit + 1 >= MaxHit)
                {
                    transform.position = hitEnemyInfo.hitEnemy.transform.position;
                    
                    var deadProjectile = ProjectileDeadPool.Instance.Get(direction);
                    deadProjectile.position = hitEnemyInfo.hitEnemy.transform.position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(0f, 0.2f), 0f);
                    deadProjectile.SetParent(hitEnemyInfo.hitEnemy.transform);
                }
                else
                    transform.position += moveDirection;
                
                ProjectileHit(hitEnemyInfo.hitEnemy);
            }
            else
            {
                transform.position += moveDirection;
            }
                
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
                collider.CanTrigger = false;
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
            
            // Set lại vị trí viên đạn vào vị trí enemy (tránh việc đạn bay nhanh quá nhìn giống như không chạm vào enemy)
            
            // Check critical hit
            var critical = Random.Range(0f, 1f) <= CriticalRate;
            hit.HitDirectionX = direction.x;
            hit.HitDirectionY = direction.y;
            hit.Damage(critical ? CriticalDamage : Damage, transform.position, Stagger, critical ? InGame.DamageType.NormalCritical : InGame.DamageType.Normal);
            if (!hit.IsDestroyed)
            {
                if (DamageType == ProjectileType.PlayerProjectile)
                    PassiveEffectManager.Instance.TriggerEffect(IsCharge ? PassiveTriggerType.DameByChargeAttack : PassiveTriggerType.DameByNormalAttack, hit);
                else if (DamageType == ProjectileType.TowerProjectile)
                    PassiveEffectManager.Instance.TriggerEffect(PassiveTriggerType.TowerTakeDame, hit);
            }
                    
            DebugUtility.Log("hit");
            if (critical)
                DebugUtility.LogWarning($"Projectile {name} deals critical damage {CriticalDamage} to {hit.name}!!");

            if (HitActions != null)
            {
                foreach (var action in HitActions)
                {
                    action.DoAction(this, transform.position);
                }
            }
                    
            OnHit?.Invoke();
            currentHit += 1;
            
            if (!BlockDestroy && currentHit >= MaxHit)
            {
                collider.CanTrigger = false;
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

    public enum ProjectileType
    {
        PlayerProjectile,
        TowerProjectile
    }
}