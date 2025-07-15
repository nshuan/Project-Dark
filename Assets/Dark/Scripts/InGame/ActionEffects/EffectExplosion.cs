using System;
using System.Collections;
using UnityEngine;

namespace InGame
{
    public class EffectExplosion : MonoEffectEntity
    {
        private Vector2 Position { get; set; }
        private float Stagger { get; set; }
        private RaycastHit2D[] hits = new RaycastHit2D[50];
        private IDamageable hitTarget;
        
        public override void TriggerEffect(int effectId, IEffectTarget target, float size, float value, float stagger, ActionEffectPool pool)
        {
            transform.position = target.Position;
            this.Position = target.Position;
            this.Stagger = stagger;

            StartCoroutine(IEExplode(() =>
            {
                // Check hit target
                var count = Physics2D.CircleCastNonAlloc(transform.position, size, Vector2.zero, hits, 0f,
                    targetLayer);
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        ExplosionHit(hits[i].transform, value);
                    }
                }
            }, () =>
            {
                pool.Release(this, effectId);
            }));
        }

        private IEnumerator IEExplode(Action actionDamage, Action actionComplete)
        {
            yield return new WaitForSeconds(0.5f);
            actionDamage?.Invoke();

            yield return new WaitForSeconds(1f);
            actionComplete?.Invoke();
        }

        private void ExplosionHit(Transform hitTransform, float value)
        {
            if (hitTransform)
            {
                if (hitTransform.TryGetComponent(out hitTarget))
                {
                    hitTarget.Damage((int)value, Position, Stagger);
                }
            }
        }
    }
}