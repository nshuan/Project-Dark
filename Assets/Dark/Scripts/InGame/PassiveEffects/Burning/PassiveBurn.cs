using UnityEngine;
using Dark.Scripts.AudioV2;

namespace InGame
{
    public class PassiveBurn : MonoPassiveEntity
    {
        [SerializeField] private AudioPlayComponentV2 sfx;

        private float delayEachBurn = 1f;
        
        public override void TriggerEffect(int effectId, IEffectTarget target, float size, float val, float stagger, PassiveEffectPool pool, params float[] additionalParams)
        {
            // Interval
            if (additionalParams is { Length: > 0 } && additionalParams[0] > 0)
                delayEachBurn = additionalParams[0];
            
            transform.SetParent(target.BurnVfxParent);
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            gameObject.SetActive(true);
            target.Burn(size, delayEachBurn, (int)(val / size * delayEachBurn), () => pool.Release(this, effectId));
            sfx.Play();
        }
    }
}