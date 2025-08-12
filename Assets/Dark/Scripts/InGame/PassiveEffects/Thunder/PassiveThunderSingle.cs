using System;
using System.Collections;
using Dark.Scripts.Audio;
using InGame.Effects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame
{
    public class PassiveThunderSingle : MonoPassiveEntity
    {
        [SerializeField] private bool randomEnemy;
        [SerializeField] private AudioComponent sfx;
        
        private EnemyEntity triggerredEnemy;
        private RaycastHit2D[] hits = new RaycastHit2D[50];
        private IEffectTarget tempTarget;
        private CameraShake cameraShakeEffect;
        
        public override void Initialize()
        {
            cameraShakeEffect = new CameraShake() { Cam = VisualEffectHelper.Instance.DefaultCamera, Duration = 0.5f, Magnitude = 0.2f};
            hits = new RaycastHit2D[10];
        }
        
        public override void TriggerEffect(int effectId, IEffectTarget target, float size, float value, float stagger, PassiveEffectPool pool)
        {
            if (randomEnemy)
            {
                StartCoroutine(IEThunder(() =>
                {
                    // Check hit target
                    var count = Physics2D.CircleCastNonAlloc(transform.position, size, Vector2.zero, hits, 0f,
                        targetLayer);
                    if (count > 0)
                    {
                        if (hits[Random.Range(0, count)].transform.TryGetComponent(out triggerredEnemy))
                        {
                            VfxThunderPool.Instance.GetAndRelease(null, triggerredEnemy.transform.position, 0f, 1f);
                            if (triggerredEnemy.PercentageHpLeft < value)
                            {
                                triggerredEnemy.Kill();
                            }
                            else
                            {
                                triggerredEnemy.Damage((int)(triggerredEnemy.MaxHealth * value), triggerredEnemy.transform.position, stagger);
                                sfx.Play();
                            }
                        }
                        
                        cameraShakeEffect.Duration = 0.5f;
                        VisualEffectHelper.Instance.PlayEffect(cameraShakeEffect);
                    }
                }, () =>
                {
                    pool.Release(this, effectId);
                }));
            }
            else
            {
                tempTarget = target;
                StartCoroutine(IEThunder(() =>
                {
                    VfxThunderPool.Instance.GetAndRelease(null, tempTarget.Position, 0f, 1f);
                    if (tempTarget.TargetTransform.TryGetComponent(out triggerredEnemy))
                    {
                        VfxThunderPool.Instance.GetAndRelease(null, triggerredEnemy.transform.position, 0f, 1f);
                        if (triggerredEnemy.PercentageHpLeft < value)
                        {
                            triggerredEnemy.Kill();
                        }
                        else
                        {
                            triggerredEnemy.Damage((int)(triggerredEnemy.MaxHealth * value / 100), triggerredEnemy.transform.position, stagger);
                        }
                    }
                    
                    cameraShakeEffect.Duration = 0.5f;
                    VisualEffectHelper.Instance.PlayEffect(cameraShakeEffect);
                }, () =>
                {
                    pool.Release(this, effectId);
                }));
            }
        }
        
        private IEnumerator IEThunder(Action actionDamage, Action actionComplete)
        {
            yield return new WaitForSeconds(0.25f);
            actionDamage?.Invoke();

            yield return new WaitForSeconds(1f);
            actionComplete?.Invoke();
        }
    }
}