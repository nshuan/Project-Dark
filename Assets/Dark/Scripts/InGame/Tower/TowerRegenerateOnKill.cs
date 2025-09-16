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

        private void OnOneEnemyKilled()
        {
            if (coroutineRegenerate != null)
                StopCoroutine(coroutineRegenerate);
            coroutineRegenerate = StartCoroutine(IERegenerate());
        }
        
        private IEnumerator IERegenerate()
        {
            vfxTowerRegenerate.gameObject.SetActive(true);
            
            tower.Regenerate(regenAmount);
            
            yield return new WaitForSeconds(vfxDuration);
            vfxTowerRegenerate.gameObject.SetActive(false);
        }
    }
}