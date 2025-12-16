using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame
{
    public class ProjectileEntity : MonoBehaviour
    {
        protected const float MaxLifeTime = 10f;

        public ProjectileCollider collider;
        [SerializeField] private float baseDamageRange = 0.1f;
        public bool forceHideDeadObject;
        
        [Space] [Header("Bullet config")]
        [SerializeField] private float baseSpeed = 5f;
        
        protected Vector2 direction;
        public Vector3 SpawnPosition { get; set; }
        protected Vector3 BoundPosition { get; set; }
        public float Range { get; set; }
        public float TrueRange { get; set; }
        public Vector3 RangeCenter { get; set; }
        protected float maxDistanceFromSpawnPosition;
        protected float maxDistanceFromRangeCenter;
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
        public bool BlockAutoDestroyOutRange { get; set; } // If the projectile reach max range, destroy it automatically
        
        public Transform TargetTransform => transform;

        protected int currentHit;
        protected bool activated = false;
        protected float lifeTime = 0f;
        protected Vector3 moveDirection = Vector3.zero;
        protected ProjectileCollider.ProjectileHitStatus hitStatus;

        protected RaycastHit2D[] hits = new RaycastHit2D[1];
        protected ProjectileCollider.HitEnemyInfo hitEnemyInfo;

        private bool hasVfxHit;
        
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
            float range,
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
            SpawnPosition = transform.position;
            Range = range;
            
            Size = size;
            transform.localScale = size * Vector3.one;
            SpeedScale = speedScale;
            Speed = baseSpeed * speedScale;
            this.RangeCenter = rangeCenter;
            this.direction = direction;
            BoundPosition =
                LevelUtility.GetIntersectionInRangeBound(RangeCenter, range, SpawnPosition, direction);
            maxDistanceFromSpawnPosition = (BoundPosition - SpawnPosition).magnitude;
            maxDistanceFromRangeCenter = (BoundPosition - RangeCenter).magnitude;
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

            BlockAutoDestroyOutRange = false;
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
            
            // Cast 1 đường theo direction, nếu đi qua itemCollector thì BlockAutoDestroyOutRange
            if (collider.CheckCollectibleOnWay(direction))
                BlockAutoDestroyOutRange = true;
            
            activated = true;
            collider.CanTrigger = true;
            
            // if (ActivateActions != null)
            // {
            //     foreach (var action in ActivateActions)
            //     {
            //         action.DoAction(this, direction);
            //     }
            // }
        }

        protected virtual void Update()
        {
            if (!activated) return;
            if (!BlockAutoDestroyOutRange)
            {
                if (Vector2.Distance(transform.position, SpawnPosition) > maxDistanceFromSpawnPosition)
                {
                    if (!BlockSpawnDeadBody)
                    {
                        // ProjectileDeadPool.Instance.Get(direction).position = transform.position;
                        ProjectileDeadPool.Instance.Get(BoundPosition, direction);
                    }
                    ProjectileHit(null);
                    return;
                }
            }

            moveDirection.x = Speed * Time.deltaTime * direction.x;
            moveDirection.y = Speed * Time.deltaTime * direction.y;
            moveDirection.z = 0f;
            
            if (!BlockAutoDestroyOutRange && Vector2.Distance(transform.position + moveDirection, SpawnPosition) > maxDistanceFromSpawnPosition)
            {
                if (!BlockSpawnDeadBody)
                {
                    // ProjectileDeadPool.Instance.Get(direction).position = transform.position;
                    ProjectileDeadPool.Instance.Get(BoundPosition, direction);
                }
                ProjectileHit(null);
            }
            else
            {
                hitStatus = collider.CheckCollision(ref moveDirection, ref hitEnemyInfo);
                if (hitStatus == ProjectileCollider.ProjectileHitStatus.Enemy)
                {
                    DebugUtility.Log($"Hit enemy {hitEnemyInfo.hitEnemy}");
                    ProjectileHit(hitEnemyInfo.hitEnemy);
                    // Nếu trúng con quái này xong là destroy đạn thì ko di chuyển viên đạn nữa, set vị trí vào chỗ con quái luôn
                    if (!hitEnemyInfo.hitEnemy.IsDestroyed && !forceHideDeadObject && currentHit >= MaxHit)
                    {
                        transform.position = hitEnemyInfo.hitEnemy.transform.position;
                        
                        var deadProjectile = ProjectileDeadOnEnemyPool.Instance.Get(direction);
                        deadProjectile.position = hitEnemyInfo.hit.point;
                        hitEnemyInfo.hitEnemy.body.SetupProjectileHit(deadProjectile.transform, direction);
                        deadProjectile.SetParent(hitEnemyInfo.hitEnemy.transform);
                        hitEnemyInfo.hitEnemy.OnStartDead += () =>
                        {
                            deadProjectile.gameObject.SetActive(false);
                        };
                    }
                }
                
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
                
                PlayVfxHit();
                PlayHitActions(hit);

                ProjectilePool.Instance.Release(this, hasVfxHit ? 1f : 0f);
                
                return;
            }

            if (hit.State == EnemyState.Invisible)
            {
                DebugUtility.Log("Invisible");
                return;
            }
            
            // Set lại vị trí viên đạn vào vị trí enemy (tránh việc đạn bay nhanh quá nhìn giống như không chạm vào enemy)
            
            // Check critical hit
            var critical = RandomUtil.Range(0f, 1f) <= CriticalRate;
            hit.HitDirectionX = direction.x;
            hit.HitDirectionY = direction.y;
            InGame.DamageType dmgType = InGame.DamageType.Normal;
            switch (DamageType)
            {
                case ProjectileType.PlayerProjectile:
                    dmgType = critical ? InGame.DamageType.NormalCritical : InGame.DamageType.Normal;
                    break;
                case ProjectileType.TowerProjectile:
                    dmgType = critical ? InGame.DamageType.TowerCritical : InGame.DamageType.Tower;
                    break;
            }
            hit.Damage(critical ? CriticalDamage : Damage, transform.position, Stagger, dmgType);
            // if (!hit.IsDestroyed)
            {
                if (DamageType == ProjectileType.PlayerProjectile)
                    PassiveEffectManager.Instance.TriggerEffect(IsCharge ? PassiveTriggerType.DameByChargeAttack : PassiveTriggerType.DameByNormalAttack, hit);
                else if (DamageType == ProjectileType.TowerProjectile)
                    PassiveEffectManager.Instance.TriggerEffect(PassiveTriggerType.TowerTakeDame, hit);
            }
                    
            DebugUtility.Log("hit");
            if (critical)
                DebugUtility.LogWarning($"Projectile {name} deals critical damage {CriticalDamage} to {hit.name}!!");

            PlayVfxHit();
            PlayHitActions(hit);
                    
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
                
                ProjectilePool.Instance.Release(this, hasVfxHit ? 1f : 0f);
            }
        }

        protected virtual void PlayVfxHit()
        {
            
        }

        protected virtual void PlayHitActions(EnemyEntity hit)
        {
            if (HitActions != null)
            {
                foreach (var action in HitActions)
                {
                    action.DoAction(this, transform.position, null);
                }
            }
        }

        protected virtual void OnDrawGizmos()
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