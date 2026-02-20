using System.Collections;
using System.Collections.Generic;
using InGame.AttackNormalConfig;
using UnityEngine;

namespace InGame
{
    public class KnightSlashEntity : AutoAimProjectileEntity
    {
        [Space] [Header("Slash")] 
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private float vfxDuration = 1.5f;
        [SerializeField] private GameObject vfxSlash;
        
        private RaycastHit2D[] hits = new RaycastHit2D[20];
        private EnemyEntity cacheEnemy;

        public override void Init(Vector2 rangeCenter, Vector2 direction, float range, float size, float speedScale, int damage,
            int criticalDamage, float criticalRate, float stagger, bool isCharge, int maxHit, List<IProjectileActivate> activateActions,
            List<IProjectileHit> hitActions, ProjectileType damageType)
        {
            base.Init(rangeCenter, direction, range, size, speedScale, damage, criticalDamage, criticalRate, stagger, isCharge, maxHit, activateActions, hitActions, damageType);
            
            transform.position = RangeCenter;
            Quaternion rotation;
            if (TargetToChase)
                rotation = Quaternion.Euler(-45f, 0f, Mathf.Atan2(TargetToChase.transform.position.y - RangeCenter.y, TargetToChase.transform.position.x - RangeCenter.x) * Mathf.Rad2Deg);
            else 
                rotation = Quaternion.Euler(-45f, 0f, Mathf.Atan2(BoundPosition.y - RangeCenter.y, BoundPosition.x - RangeCenter.x) * Mathf.Rad2Deg);
            transform.rotation = Quaternion.identity;
            vfxSlash.transform.rotation = rotation;
            
            // Attack piercing dùng script này nên nhân bonus vào đây
            if (LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockNormalAttackPiercing &&
                LevelUtilityV2.StatsNormalPiercing is KnightSkillNormalConfig piercingSkillConfig)
            {
                Size *= piercingSkillConfig.sizeScale;
                Range *= piercingSkillConfig.rangeScale;
            }
            
            // Attack bullet dùng script này nên nhân bonus vào đây
            if (LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockNormalAttackBullet &&
                LevelUtilityV2.StatsNormalBullet is KnightSkillNormalConfig bulletSkillConfig)
            {
                Size *= bulletSkillConfig.sizeScale;
                Range *= bulletSkillConfig.rangeScale;
            }
        }

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
            vfxSlash.SetActive(false);
            transform.localScale = Range * Vector3.one;
            yield return new WaitForSeconds(delay);
            vfxSlash.SetActive(true);
            
            activated = true;
            collider.CanTrigger = false;
            
            var hitCount = Physics2D.CircleCastNonAlloc(RangeCenter, Range, direction, hits, 0f, enemyLayer);
            if (hitCount > 0)
            {
                var halfAngle = Size / 2;
                for (var i = 0; i < hitCount; i++)
                {
                    var dirTo = hits[i].point - (Vector2)transform.position;
                    var dirToCenter = hits[i].point - (Vector2)RangeCenter;
                    
                    // Check những enemy va chạm, nếu nằm trong góc damageAngle thì mới gây dame
                    if (Vector2.Angle(dirToCenter, dirTo.normalized) > halfAngle) continue;
                    
                    // Check relative range, tăng range lên 1 tí
                    var bonusRangeForInRangeEnemy = (TargetToChase && hits[i].transform == TargetToChase.transform) ? 0.2f : 0.1f;
                    if (dirToCenter.magnitude > (LevelUtilityV2.GetRelativeRangeMove(Range, dirToCenter) + bonusRangeForInRangeEnemy)) continue;
                    
                    if (hits[i].transform.TryGetComponent<EnemyEntity>(out cacheEnemy))
                    {
                        ProjectileHit(cacheEnemy);
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