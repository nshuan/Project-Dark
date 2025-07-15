using UnityEngine;

namespace InGame
{
    public abstract class MonoEffectEntity : MonoBehaviour
    {
        [SerializeField] protected LayerMask targetLayer;
        
        public abstract void TriggerEffect(int effectId, IEffectTarget target, float size, float value, float stagger, ActionEffectPool pool);
    }
}