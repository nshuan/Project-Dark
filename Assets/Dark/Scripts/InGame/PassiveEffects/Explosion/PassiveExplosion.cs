using System;
using System.Collections;
using Dark.Scripts.Audio;
using InGame.Effects;
using UnityEngine;

namespace InGame
{
    public class PassiveExplosion : MonoPassiveEntity
    {
        [SerializeField] private float vfxDuration = 1f;
        
        private Vector2 Position { get; set; }
        private float Stagger { get; set; }
        private RaycastHit2D[] hits = new RaycastHit2D[50];
        private IDamageable hitTarget;
        private CameraShake cameraShakeEffect;
        [SerializeField] private AudioComponent sfx;

        public override void Initialize()
        {
            cameraShakeEffect = new CameraShake() { Cam = VisualEffectHelper.Instance.DefaultCamera };
        }

        public override void TriggerEffect(int effectId, IEffectTarget target, float size, float value, float stagger, PassiveEffectPool pool)
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
                    for (var i = 0; i < count; i++)
                    {
                        ExplosionHit(hits[i].transform, value);
                        sfx.Play();
                    }
                }
                
                VisualEffectHelper.Instance.PlayEffect(cameraShakeEffect);
            }, () =>
            {
                pool.Release(this, effectId);
            }));
        }

        private IEnumerator IEExplode(Action actionDamage, Action actionComplete)
        {
            // yield return new WaitForSeconds(0.5f);
            actionDamage?.Invoke();

            yield return new WaitForSeconds(vfxDuration);
            actionComplete?.Invoke();
        }

        private void ExplosionHit(Transform hitTransform, float value)
        {
            if (hitTransform)
            {
                if (hitTransform.TryGetComponent(out hitTarget))
                {
                    hitTarget.HitDirectionX = hitTransform.position.x - Position.x;
                    hitTarget.HitDirectionY = hitTransform.position.y - Position.y;
                    hitTarget.Damage((int)value, Position, Stagger);
                }
            }
        }
    }
}