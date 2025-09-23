using System;
using System.Collections;
using InGame.Upgrade;
using UnityEngine;

namespace InGame
{
    public class TowerCounter : MonoBehaviour
    {
        [SerializeField] private TowerEntity tower;
        
        [SerializeField] private bool canCounter;
        private bool counterCooldown;
        [SerializeField] private TowerCounterConfig config;
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
            canCounter = bonusInfo.unlockedTowerCounter != null;
            config = bonusInfo.unlockedTowerCounter;
        }

        private void OnTowerHit(Vector2 attackerPos)
        {
            if (!canCounter) return;
            if (counterCooldown) return;
            OnOneTowerHit?.Invoke(attackerPos);
            CombatActions.OnTowerCounter?.Invoke(config.cooldown);
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

            var damage = config.damage + LevelUtility.BonusInfo.towerCounterDamagePlus;
            var cooldown = Mathf.Max(config.cooldown - LevelUtility.BonusInfo.towerCounterCooldownPlus, 0f);
            config.logic.Counter(transform.position, counterDirection, damage, 1f);
            StartCoroutine(IECounterCooldown(cooldown));
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
    }
}