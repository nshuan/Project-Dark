using System;
using UnityEngine;

namespace InGame
{
    public class EffectExplosion : MonoEffectEntity
    {
        [SerializeField] private LayerMask targetLayer;

        private Vector2 Position { get; set; }
        private float Stagger { get; set; }
        private RaycastHit2D[] hits = new RaycastHit2D[50];
        private IDamageable hitTarget;
        
        public override void TriggerEffect(int effectId, Vector2 position, float size, float value, float stagger, ActionEffectPool pool)
        {
            hits = new RaycastHit2D[50];
            this.Position = position;
            this.Stagger = stagger;
            
            // Check hit target
            var count = Physics2D.CircleCastNonAlloc(position, size, Vector2.zero, hits, 0f,
                targetLayer);
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    ExplosionHit(hits[i].transform, value);
                }
            }
            
            pool.Release(this, effectId);
        }

        private void ExplosionHit(Transform hitTransform, float value)
        {
            if (hitTransform)
            {
                if (hitTransform.TryGetComponent(out hitTarget))
                {
                    hitTarget.Damage((int)value, ((Vector2)hitTransform.position - Position).normalized, Stagger);
                }
            }
        }
    }
}