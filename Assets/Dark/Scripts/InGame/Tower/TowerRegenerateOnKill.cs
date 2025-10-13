using System.Collections;
using Dark.Scripts.Common.ParticleSystemUtil;
using UnityEngine;

namespace InGame
{
    public class TowerRegenerateOnKill : MonoBehaviour
    {
        private TowerEntity tower;
        private int regenAmount;
        
        private Coroutine coroutineRegenerate;
        
        [Header("Visual")]
        [SerializeField] private ParticleSystemGroup vfxTowerRegenerate;
        [SerializeField] private float vfxDuration;
        
        public void Initialize(TowerEntity targetTower, int amount)
        {
            tower = targetTower;
            regenAmount = amount;

            EnemyManager.Instance.OnOneEnemyDead += OnOneEnemyKilled;
        }

        private void OnOneEnemyKilled(EnemyEntity enemy)
        {
            if (regenAmount <= 0) return;
            
            if (tower.CurrentHp < tower.MaxHp && tower.CurrentHp > 0)
            {
                if (coroutineRegenerate != null)
                    StopCoroutine(coroutineRegenerate);
                coroutineRegenerate = StartCoroutine(IERegenerateVfx());
                tower.Regenerate(regenAmount);
            }
        }
        
        private IEnumerator IERegenerateVfx()
        {
            vfxTowerRegenerate.gameObject.SetActive(true);
            
            yield return new WaitForSeconds(vfxDuration);
            vfxTowerRegenerate.gameObject.SetActive(false);
        }
    }
}