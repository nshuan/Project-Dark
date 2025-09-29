using System;
using System.Collections;
using Dark.Scripts.Audio;
using InGame.Effects;
using UnityEngine;

namespace InGame
{
    public class PassiveThunder : MonoPassiveEntity
    {
        [SerializeField] private int maxTriggeredEnemies = 20;
        [SerializeField] private bool triggerAllEnemies = false;
        [SerializeField] private AudioComponent sfx;
        
        private EnemyEntity[] triggerredEnemies;
        private RaycastHit2D[] hits = new RaycastHit2D[50];
        private IEffectTarget tempTarget;
        private CameraShake cameraShakeEffect;
        
        public override void Initialize()
        {
            cameraShakeEffect = new CameraShake() { Cam = VisualEffectHelper.Instance.DefaultCamera, Duration = 0.5f, Magnitude = 0.2f};
            triggerredEnemies = new EnemyEntity[maxTriggeredEnemies];
            hits = new RaycastHit2D[50];
        }
        
        public override void TriggerEffect(int effectId, IEffectTarget target, float size, float value, float stagger, PassiveEffectPool pool)
        {
            gameObject.SetActive(true);
            if (triggerAllEnemies)
            {
                StartCoroutine(IEThunder(() =>
                {
                    var triggerCount = EnemyManager.Instance.FilterEnemiesNonAlloc(enemy => enemy.PercentageHpLeft < value, ref triggerredEnemies);
                    var indexVfx = 0;
                    if (triggerCount > 0)
                    {
                        cameraShakeEffect.Duration = 0.5f + 0.1f * (triggerCount - 1);
                        VisualEffectHelper.Instance.PlayEffect(cameraShakeEffect);
                    }
                    while (triggerCount > 0)
                    {
                        VfxThunderPool.Instance.GetAndRelease(null, triggerredEnemies[triggerCount - 1].transform.position, 0.1f * indexVfx, 1f);
                        triggerredEnemies[triggerCount - 1].Kill();
                        triggerCount -= 1;
                        indexVfx += 1;
                        sfx.Play();
                    }
                }, () =>
                {
                    pool.Release(this, effectId);
                }));
            }
            else
            {
                StartCoroutine(IEThunder(() =>
                {
                    // Check hit target
                    var count = Physics2D.CircleCastNonAlloc(transform.position, size, Vector2.zero, hits, 0f,
                        targetLayer);
                    if (count > 0)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            if (hits[i].transform.TryGetComponent(out tempTarget))
                            {
                                VfxThunderPool.Instance.GetAndRelease(null, tempTarget.Position, 0.1f * i, 1f);
                                if (tempTarget.PercentageHpLeft < value)
                                {
                                    tempTarget.Kill();
                                }
                            }
                        }
                        
                        cameraShakeEffect.Duration = 0.5f + 0.1f * (count - 1);
                        VisualEffectHelper.Instance.PlayEffect(cameraShakeEffect);
                    }
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