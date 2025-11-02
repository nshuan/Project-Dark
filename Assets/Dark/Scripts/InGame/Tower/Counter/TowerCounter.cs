using System;
using System.Collections;
using InGame.CounterConfig;
using InGame.Upgrade;
using UnityEngine;

namespace InGame
{
    public class TowerCounter : MonoBehaviour
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
        protected TowerCounterConfig config;

        protected int Damage => LevelUtility.GetTowerCounterDamage(config.damage);
        protected float Cooldown => LevelUtility.GetTowerCounterCooldown(counterType, config.cooldown);
        
        protected static event Action<NodeTowerCounter.CounterType, Vector2> OnOneTowerHit;
        
        protected Vector2 counterDirection = Vector2.zero;
        
        private void Awake()
        {
            UpgradeManager.Instance.OnActivated += OnUpgradeBonusActivated;
            LevelManager.Instance.OnLose += OnLose;
            tower.OnHitAttackerPos += OnTowerHit;
            OnOneTowerHit += OnCounter;
            tower.OnDestroyed += OnTowerDestroyed;

            config = TowerCounterManifest.Get(counterType);
        }

        private void OnDestroy()
        {
            UpgradeManager.Instance.OnActivated -= OnUpgradeBonusActivated;
            OnOneTowerHit -= OnCounter;
        }

        private void OnUpgradeBonusActivated(UpgradeBonusInfo bonusInfo)
        {
            canCounter = bonusInfo.unlockedTowerCounter != null && bonusInfo.unlockedTowerCounter.ContainsKey(counterType) && bonusInfo.unlockedTowerCounter[counterType];
            // canCounter = true;
        }

        private void OnTowerHit(Vector2 attackerPos)
        {
            if (!canCounter) return;
            if (counterCooldown) return;
            OnOneTowerHit?.Invoke(counterType, attackerPos);
            CombatActions.OnTowerCounter?.Invoke(counterType, Cooldown);
        }

        private void OnTowerDestroyed(TowerEntity destroyedTower)
        {
            tower.OnHitAttackerPos -= OnTowerHit;
            OnOneTowerHit -= OnCounter;
        }

        private void OnCounter(NodeTowerCounter.CounterType counterType, Vector2 attackerPos)
        {
            if (counterType != this.counterType) return;
            counterDirection.x = attackerPos.x - transform.position.x;
            counterDirection.y = attackerPos.y - transform.position.y;
            
            StartCoroutine(IECounter(Cooldown));
        }

        private void OnLose()
        {
            LevelManager.Instance.OnLose -= OnLose;
            canCounter = false;
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
            projectile.transform.position = towerAttackPos;
            projectile.transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            projectile.Init(towerAttackPos, direction.normalized, 20, 5, speedScale, damage, damage, 0f, config.stagger, false, 10, null, null, ProjectileType.TowerProjectile);
            projectile.BlockDestroy = true;
            projectile.Activate(0f);
        }
    }
}