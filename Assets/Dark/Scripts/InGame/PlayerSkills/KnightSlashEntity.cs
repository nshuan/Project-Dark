using System.Collections;
using UnityEngine;

namespace InGame
{
    public class KnightSlashEntity : ProjectileEntity
    {
        [Space] [Header("Slash")] 
        [SerializeField] private float slashSpan = 45f;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private float vfxDuration = 1.5f;
        
        private RaycastHit2D[] hits = new RaycastHit2D[20];
        private EnemyEntity cacheEnemy;
        
        protected override void FixedUpdate()
        {
            
        }

        protected override void PlayHitActions(EnemyEntity hit)
        {
            
        }

        public override void ProjectileHit(EnemyEntity hit)
        {
            if (!hit)
            {
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
                    
            currentHit += 1;
            OnHit?.Invoke();
        }

        protected override IEnumerator IEActivate(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            activated = true;
            collider.CanTrigger = false;
            
            var hitCount = Physics2D.CircleCastNonAlloc(transform.position, Range, direction, hits, 0f, enemyLayer);
            if (hitCount > 0)
            {
                var halfAngle = slashSpan / 2;
                for (var i = 0; i < hitCount; i++)
                {
                    var dirTo = (hits[i].point - (Vector2)transform.position).normalized;
                    // Check những enemy va chạm, nếu nằm trong góc damageAngle thì mới gây dame
                    if (Vector2.Angle(direction, dirTo) <= halfAngle)
                    {
                        if (hits[i].transform.TryGetComponent<EnemyEntity>(out cacheEnemy))
                        {
                            ProjectileHit(cacheEnemy);
                        }
                    }
                }
            }
            
            collider.CanTrigger = false;
            BlockDestroy = false;
            BlockSpawnDeadBody = false;
            OnHit = null;
            lifeTime = 0f;
            activated = false; ;

            yield return new WaitForSeconds(vfxDuration);
            ProjectilePool.Instance.Release(this, hasVfxHit ? 1f : 0f);
        }
    }
}