using System;
using System.Collections;
using InGame.Upgrade;
using UnityEngine;

namespace InGame
{
    public class TowerCounter : MonoBehaviour
    {
        [SerializeField] private TowerEntity tower;
        [SerializeField] private ProjectileEntity projectilePrefab;
        
        [SerializeField] private bool canCounter;
        private bool counterCooldown;

        [Space] [Header("Config")] 
        public int baseDamage;
        public float baseCooldown;

        private int Damage => LevelUtility.GetTowerCounterDamage(baseDamage);
        private float Cooldown => LevelUtility.GetTowerCounterCooldown(baseCooldown);
        
        private static event Action<Vector2> OnOneTowerHit;
        
        private Vector2 counterDirection = Vector2.zero;
        
        private void Awake()
        {
            UpgradeManager.Instance.OnActivated += OnUpgradeBonusActivated;
            LevelManager.Instance.OnLose += OnLose;
            tower.OnHitAttackerPos += OnTowerHit;
            OnOneTowerHit += OnCounter;
            tower.OnDestroyed += OnTowerDestroyed;
        }

        private void OnDestroy()
        {
            UpgradeManager.Instance.OnActivated -= OnUpgradeBonusActivated;
            OnOneTowerHit -= OnCounter;
        }

        private void OnUpgradeBonusActivated(UpgradeBonusInfo bonusInfo)
        {
            canCounter = bonusInfo.unlockedTowerCounter;
        }

        private void OnTowerHit(Vector2 attackerPos)
        {
            if (!canCounter) return;
            if (counterCooldown) return;
            OnOneTowerHit?.Invoke(attackerPos);
            CombatActions.OnTowerCounter?.Invoke(Cooldown);
        }

        private void OnTowerDestroyed(TowerEntity destroyedTower)
        {
            tower.OnHitAttackerPos -= OnTowerHit;
            OnOneTowerHit -= OnCounter;
        }

        private void OnCounter(Vector2 attackerPos)
        {
            counterDirection.x = attackerPos.x - transform.position.x;
            counterDirection.y = attackerPos.y - transform.position.y;
            
            Counter(transform.position, counterDirection, Damage, 1f);
            StartCoroutine(IECounterCooldown(Cooldown));
        }

        private void OnLose()
        {
            LevelManager.Instance.OnLose -= OnLose;
            canCounter = false;
        }
        
        private IEnumerator IECounterCooldown(float cooldown)
        {
            counterCooldown = true;
            yield return new WaitForSeconds(cooldown);
            counterCooldown = false;
        }
        
        public void Counter(Vector2 towerAttackPos, Vector2 direction, int damage, float speedScale)
        {
            var projectile = ProjectilePool.Instance.Get(projectilePrefab, null, false);
            projectile.transform.position = towerAttackPos;
            projectile.transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            projectile.Init(towerAttackPos, direction.normalized, 20, 5, speedScale, damage, damage, 0f, 0f, false, 10, null, null, ProjectileType.TowerProjectile);
            projectile.BlockDestroy = true;
            projectile.Activate(0f);
        }
    }
}