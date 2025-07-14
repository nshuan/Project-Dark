using UnityEngine;

namespace InGame
{
    public class EffectBurn : MonoEffectEntity
    {
        [SerializeField] private float delayEachBurn = 1f;
        
        public override void TriggerEffect(int effectId, IEffectTarget target, float size, float value, float stagger, ActionEffectPool pool)
        {
            target.Burn(size, delayEachBurn, (int)value);
        }
    }
}