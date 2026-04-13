using System;
using System.Collections;
using UnityEngine;

namespace InGame.Shield
{
    public class TowerShield : MonoBehaviour
    {
        [SerializeField] private TowerShieldConfig config;

        private int currentShield;
        public int CurrentShield => currentShield;
        public int MaxShield { get; private set; }
        
        public Action<int> OnRegenerate { get; set; }
        
        private Coroutine coroutineHealing;
        
        public void Initialize(int maxShield, int currentShield)
        {
            MaxShield = maxShield;
            this.currentShield = currentShield;
        }
        
        // Return the amount of damage left after reducing shield
        public int Damage(int damage)
        {
            if (coroutineHealing != null) StopCoroutine(coroutineHealing);
            
            if (currentShield <= damage)
            {
                damage -= currentShield;
                currentShield = 0;
            }
            else
            {
                currentShield -= damage;
                damage = 0;
            }

            // Each time the tower is damaged, restart healing cooldown
            coroutineHealing = StartCoroutine(IEHealing());
            
            return damage;
        }

        private IEnumerator IEHealing()
        {
            yield return new WaitForSeconds(config.healingInterval);
            
            var amountHealingPerTime = Mathf.CeilToInt(MaxShield / (config.healingDuration / config.healingDelta));
            while (currentShield < MaxShield)
            {
                var lastShield = currentShield;
                currentShield = Mathf.Min(currentShield + amountHealingPerTime, MaxShield);
                OnRegenerate?.Invoke(currentShield - lastShield);
                yield return new WaitForSeconds(config.healingDelta);
            }
        }
    }
}