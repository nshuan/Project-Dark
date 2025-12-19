using UnityEngine;
using Dark.Scripts.AudioV2;

namespace InGame
{
    public class PassiveBurn : MonoPassiveEntity
    {
        [SerializeField] private float delayEachBurn = 1f;
        [SerializeField] private AudioPlayComponentV2 sfx;

        public override void TriggerEffect(int effectId, IEffectTarget target, float size, float val, float stagger, PassiveEffectPool pool)
        {
            transform.SetParent(target.BurnVfxParent);
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            gameObject.SetActive(true);
            target.Burn(size, delayEachBurn, (int)(val / size * delayEachBurn), () => pool.Release(this, effectId));
            sfx.Play();
        }
    }
}