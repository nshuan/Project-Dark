using System;
using System.Collections;
using DG.Tweening;
using InGame.Upgrade;
using Unity.VisualScripting;
using UnityEngine;

namespace InGame.WatchTower
{
    public class TowerAttackInRange : MonoBehaviour
    {
        [SerializeField] protected TowerEntity tower;
        [SerializeField] protected NodeTowerCounter.CounterType counterType;
        [SerializeField] protected ProjectileEntity projectilePrefab;
        [SerializeField] protected GameObject vfxActivateCounter;
        [SerializeField] protected float vfxActivateCounterDuration;
        [SerializeField] protected float bulletSpeedScale = 2f;

        [Space] [Header("Range")] 
        [SerializeField] protected Transform detectRange;
        [SerializeField] protected CircleCollider2D detectCollider;
        
        protected bool counterCooldown;

        [Space] [Header("Config")] 
        public bool canCounter;

        protected int Damage => GetCounterDamage();
        protected float Cooldown => GetCounterCooldown();
        protected float DetectRange => GetRangeRadius();
        protected float DelayOnDetected => GetDelayOnDetectedEnemy();

        protected Vector2 counterDirection = Vector2.zero;

        protected Coroutine coroutineCounter;
        protected Coroutine coroutineCooldown;
        protected int totalInRangeTarget;
        
        private void Awake()
        {
            LevelManager.Instance.OnInitTowers += OnInitTowers;
            LevelManager.Instance.OnLose += OnLose;
            tower.OnDestroyed += OnTowerDestroyed;
            detectRange.localScale = DetectRange * Vector3.one;
            detectCollider.radius = DetectRange;
            detectRange.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            LevelManager.Instance.OnInitTowers -= OnInitTowers;
        }

        #region Config
        
        private int GetCounterDamage()
        {
            return counterType switch
            {
                NodeTowerCounter.CounterType.Pierce => LevelUtilityV2.GetCounterPiercingDamage(),
                NodeTowerCounter.CounterType.Slash => LevelUtilityV2.GetCounterSlashDamage(),
                _ => 1
            };
        }

        private float GetCounterCooldown()
        {
            return counterType switch
            {
                NodeTowerCounter.CounterType.Pierce => LevelUtilityV2.GetCounterPiercingCooldown(),
                NodeTowerCounter.CounterType.Slash => LevelUtilityV2.GetCounterSlashCooldown(),
                _ => 1
            };
        }

        private float GetRangeRadius()
        {
            return counterType switch
            {
                NodeTowerCounter.CounterType.Pierce => LevelUtilityV2.GetCounterPiercingDetectRange(),
                NodeTowerCounter.CounterType.Slash => LevelUtilityV2.GetCounterSlashDetectRange(),
                _ => 1
            };
        }
        
        private float GetDelayOnDetectedEnemy()
        {
            return counterType switch
            {
                NodeTowerCounter.CounterType.Pierce => LevelUtilityV2.GetCounterPiercingDelayAfterDetected(),
                NodeTowerCounter.CounterType.Slash => LevelUtilityV2.GetCounterSlashDelayAfterDetected(),
                _ => 1
            };
        }
        
        #endregion
        private void OnInitTowers()
        {
            var bonusInfo = LevelUtilityV2.BonusInfo;
            if (counterType == NodeTowerCounter.CounterType.Pierce)
                canCounter = bonusInfo.bonusUnlockSkill.unlockCounterPiercing;
            else if (counterType == NodeTowerCounter.CounterType.Slash)
                canCounter = bonusInfo.bonusUnlockSkill.unlockCounterSlash;
        }

        private void OnTowerDestroyed(TowerEntity destroyedTower)
        {
            canCounter = false;
        }
        
        private void OnLose()
        {
            LevelManager.Instance.OnLose -= OnLose;
            canCounter = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!canCounter) return;
            if (counterCooldown) return;
            if (!other.CompareTag("Enemy")) return;
            if (!other.transform.TryGetComponent<EnemyEntity>(out var enemy)) return;
            var triggerDirection = other.transform.position - transform.position;
            if (LevelUtilityV2.GetRelativeRange(DetectRange, triggerDirection) < triggerDirection.magnitude) return;

            enemy.OnDead += OnEnemyDead;
            totalInRangeTarget += 1;
            counterDirection.x = triggerDirection.x;
            counterDirection.y = triggerDirection.y;
            if (coroutineCounter == null) coroutineCounter = StartCoroutine(IECounter(Cooldown));
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            totalInRangeTarget -= 1;
            if (totalInRangeTarget < 0) totalInRangeTarget = 0;
            if (totalInRangeTarget == 0 && coroutineCounter != null)
            {
                detectRange.gameObject.SetActive(false);
                StopCoroutine(coroutineCounter);
                coroutineCounter = null;
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            OnTriggerEnter2D(other);
        }

        private void OnEnemyDead(EnemyDieReason dieReason)
        {
            totalInRangeTarget -= 1;
            if (totalInRangeTarget < 0) totalInRangeTarget = 0;
            if (totalInRangeTarget == 0 && coroutineCounter != null)
            {
                detectRange.gameObject.SetActive(false);
                StopCoroutine(coroutineCounter);
                coroutineCounter = null;
            }
        }

        protected virtual IEnumerator IECounter(float cooldown)
        {
            DOTween.Kill(vfxActivateCounter);
            detectRange.gameObject.SetActive(true);
            yield return new WaitForSeconds(DelayOnDetected); 
            if (totalInRangeTarget <= 0)
                yield break;
            
            if (coroutineCooldown != null) StopCoroutine(coroutineCooldown);
            coroutineCooldown = StartCoroutine(IECooldown(cooldown));
                
            vfxActivateCounter.transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(counterDirection.y, counterDirection.x) * Mathf.Rad2Deg);
            vfxActivateCounter.SetActive(true);
            Counter(transform.position, counterDirection, Damage, bulletSpeedScale);
            detectRange.gameObject.SetActive(false);
            DOVirtual.DelayedCall(vfxActivateCounterDuration, () =>
            {
                vfxActivateCounter.SetActive(false);
            }).SetTarget(vfxActivateCounter);
            
            coroutineCounter = null;
        }
        
        public virtual void Counter(Vector2 towerAttackPos, Vector2 direction, int damage, float speedScale)
        {
            var projectile = ProjectilePool.Instance.Get(projectilePrefab, null, false);
            var stagger = counterType switch
            {
                NodeTowerCounter.CounterType.Pierce => LevelUtilityV2.StatsCounterPiercing.stagger,
                NodeTowerCounter.CounterType.Slash => LevelUtilityV2.StatsCounterSlash.stagger,
                _ => 1
            };
            var maxHit = 20;
            var size = LevelUtilityV2.GetCounterPiercingSize();
            projectile.transform.position = towerAttackPos;
            projectile.transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            projectile.Init(towerAttackPos, direction.normalized, 8, size, speedScale, damage, damage, 0f, stagger, false, maxHit, null, null, ProjectileType.TowerProjectile);
            projectile.BlockDestroy = true;
            projectile.Activate(0f);
        }

        private IEnumerator IECooldown(float cooldown)
        {
            counterCooldown = true;
            yield return new WaitForSeconds(cooldown);
            counterCooldown = false;
        }
    }
}