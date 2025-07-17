using UnityEngine;

namespace InGame
{
    public abstract class MonoPassiveEntity : MonoBehaviour
    {
        [SerializeField] protected LayerMask targetLayer;
        
        public abstract void TriggerEffect(int effectId, IEffectTarget target, float size, float value, float stagger, PassiveEffectPool pool);
    }
}