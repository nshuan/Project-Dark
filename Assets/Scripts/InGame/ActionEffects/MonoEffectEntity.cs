using UnityEngine;

namespace InGame
{
    public abstract class MonoEffectEntity : MonoBehaviour
    {
        [SerializeField] protected LayerMask targetLayer;
        
        public abstract void TriggerEffect(int effectId, Vector2 center, float size, float value, float stagger, ActionEffectPool pool);
    }
}