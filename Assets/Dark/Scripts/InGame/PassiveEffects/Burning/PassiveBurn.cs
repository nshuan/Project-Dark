using UnityEngine;

namespace InGame
{
    public class PassiveBurn : MonoPassiveEntity
    {
        [SerializeField] private float delayEachBurn = 1f;
        
        public override void TriggerEffect(int effectId, IEffectTarget target, float size, float value, float stagger, PassiveEffectPool pool)
        {
            transform.position = target.TargetTransform.position;
            transform.SetParent(target.TargetTransform);
            target.Burn(size, delayEachBurn, (int)value, () => pool.Release(this, effectId));
        }
    }
}