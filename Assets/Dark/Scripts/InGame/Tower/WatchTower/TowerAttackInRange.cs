using System;
using System.Collections;
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
        
        protected bool counterCooldown;

        [Space] [Header("Config")] 
        public bool canCounter;

        protected int Damage => GetCounterDamage();
        protected float Cooldown => GetCounterCooldown();

        protected Vector2 counterDirection = Vector2.zero;
        
        private void Awake()
        {
            UpgradeManager.Instance.OnActivated += OnUpgradeBonusActivated;
            LevelManager.Instance.OnLose += OnLose;
            tower.OnDestroyed += OnTowerDestroyed;
        }

        private void OnDestroy()
        {
            UpgradeManager.Instance.OnActivated -= OnUpgradeBonusActivated;
        }

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
        
        private void OnUpgradeBonusActivated(UpgradeBonusInfoV2 bonusInfo)
        {
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
            // if (!canCounter) return;
            if (counterCooldown) return;
            if (!other.CompareTag("Enemy")) return;
            if (!other.transform.TryGetComponent<EnemyEntity>(out var enemy)) return;
            
            counterDirection.x = enemy.transform.position.x - transform.position.x;
            counterDirection.y = enemy.transform.position.y - transform.position.y;
            StartCoroutine(IECounter(Cooldown));
        }

        protected virtual IEnumerator IECounter(float cooldown)
        {
            counterCooldown = true;
            vfxActivateCounter.transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(counterDirection.y, counterDirection.x) * Mathf.Rad2Deg);
            vfxActivateCounter.SetActive(true);
            Counter(transform.position, counterDirection, Damage, bulletSpeedScale);
            yield return new WaitForSeconds(vfxActivateCounterDuration);
            vfxActivateCounter.SetActive(false);
            yield return new WaitForSeconds(cooldown - vfxActivateCounterDuration);
            counterCooldown = false;
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
            var maxHit = LevelUtilityV2.GetCounterPiercingAmount();
            projectile.transform.position = towerAttackPos;
            projectile.transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            projectile.Init(towerAttackPos, direction.normalized, 8, 5, speedScale, damage, damage, 0f, stagger, false, maxHit, null, null, ProjectileType.TowerProjectile);
            projectile.BlockDestroy = true;
            projectile.Activate(0f);
        }
    }
}