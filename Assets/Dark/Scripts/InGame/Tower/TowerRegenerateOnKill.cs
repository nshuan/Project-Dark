using System.Collections;
using Dark.Scripts.Common.ParticleSystemUtil;
using UnityEngine;

namespace InGame
{
    public class TowerRegenerateOnKill : MonoBehaviour
    {
        private TowerEntity tower;
        private float lifeLeechRate;
        private float currenLifeToConvert;
        
        private Coroutine coroutineRegenerate;
        
        [Header("Visual")]
        [SerializeField] private ParticleSystemGroup vfxTowerRegenerate;
        [SerializeField] private float vfxDuration;
        
        public void Initialize(TowerEntity targetTower, float amount)
        {
            tower = targetTower;
            lifeLeechRate = amount;
            currenLifeToConvert = 0;

            CombatActions.OnDamageDealt += OnDamageDealt;
        }

        private void OnDamageDealt(int value)
        {
            if (lifeLeechRate <= 0) return;
            if (value <= 0) return;
            if (tower.Id != LevelManager.Instance.CurrentTower.Id) return;

            currenLifeToConvert += lifeLeechRate * value;
            var valueToAdd = Mathf.FloorToInt(currenLifeToConvert);
            currenLifeToConvert -= valueToAdd;
            
            if (tower.CurrentHp < tower.MaxHp && tower.CurrentHp > 0)
            {
                if (coroutineRegenerate != null)
                    StopCoroutine(coroutineRegenerate);
                coroutineRegenerate = StartCoroutine(IERegenerateVfx());
                tower.Regenerate(valueToAdd);
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