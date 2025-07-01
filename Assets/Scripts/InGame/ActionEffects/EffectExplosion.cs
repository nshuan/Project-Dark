using System;
using UnityEngine;

namespace InGame
{
    [Serializable]
    public class EffectExplosion : IActionEffectLogic
    {
        [SerializeField] private ActionEffectEntity effectEntity;
        [SerializeField] private LayerMask targetLayer;

        private Vector2 Position { get; set; }
        private float Stagger { get; set; }
        private RaycastHit2D[] hits = new RaycastHit2D[50];
        private IDamageable hitTarget;

        public void Initialize()
        {
            hits = new RaycastHit2D[50];
        }

        public void TriggerEffect(int effectId, Vector2 position, float size, float value, float stagger)
        {
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