using System.Collections;
using Dark.Scripts.Common.ParticleSystemUtil;
using Dark.Scripts.Utils;
using UnityEngine;

namespace InGame
{
    public class TowerAutoRegenerate : MonoBehaviour
    {
        private TowerEntity tower;
        private bool canRegen = false;
        private int hpPerSecond;
        private float secondPerHp;
        private Coroutine coroutineRegenerate;
        private Coroutine coroutineRegenerateVfx;
        
        [Header("Visual")]
        [SerializeField] private ParticleSystemGroup vfxTowerRegenerate;
        [SerializeField] private float vfxInterval;
        [SerializeField] private float vfxDuration;
        [SerializeField] private float vfxDurationScale;
        
        public void Initialize(TowerEntity targetTower, int hpPerSec)
        {
            tower = targetTower;
            hpPerSecond = hpPerSec;
            if (hpPerSecond > 0)
                secondPerHp = 1f / hpPerSec;
            else
                secondPerHp = 9999999;
            
            canRegen = hpPerSecond > 0;
            vfxTowerRegenerate.ScaleStartLifetime(vfxDurationScale);
        }

        public void Activate()
        {
            if (!canRegen) return;
            if (coroutineRegenerate != null) return;

            coroutineRegenerate = StartCoroutine(IERegenerate());
            coroutineRegenerateVfx ??= StartCoroutine(IERegenerateVfx());
        }

        public void Deactivate()
        {
            if (coroutineRegenerate != null)
            {
                StopCoroutine(coroutineRegenerate);
                coroutineRegenerate = null;
            }

            if (coroutineRegenerateVfx != null)
            {
                StopCoroutine(coroutineRegenerateVfx);
                coroutineRegenerateVfx = null;
            }
        }

        private IEnumerator IERegenerate()
        {
            while (tower.CurrentHp < tower.MaxHp && tower.CurrentHp > 0)
            {
                yield return new WaitForSeconds(secondPerHp);
                tower.Regenerate(1);
            }
            
            Deactivate();
        }

        private IEnumerator IERegenerateVfx()
        {
            vfxTowerRegenerate.gameObject.SetActive(false);
            yield return new WaitForSeconds(vfxInterval);
            
            while (tower.CurrentHp < tower.MaxHp && tower.CurrentHp > 0)
            {
                vfxTowerRegenerate.gameObject.SetActive(true);
                yield return new WaitForSeconds(vfxDuration);
                vfxTowerRegenerate.gameObject.SetActive(false);
                yield return new WaitForSeconds(vfxInterval - vfxDuration);
            }
        }
    }
}