using UnityEngine;

namespace InGame
{
    public abstract class MonoEffectEntity : MonoBehaviour
    {
        public abstract void TriggerEffect(int effectId, Vector2 center, float size, float value, float stagger, ActionEffectPool pool);
    }
}