using System;
using System.Collections;
using Dark.Scripts.AudioV2;
using InGame.Effects;
using UnityEngine;

namespace InGame
{
    public class PassiveThunderSingle : MonoPassiveEntity
    {
        [SerializeField] private AudioPlayComponentV2 sfx;
        
        private EnemyEntity triggerredEnemy;
        private RaycastHit2D[] hits = new RaycastHit2D[50];
        private IEffectTarget tempTarget;
        private CameraShake cameraShakeEffect;
        
        public override void Initialize()
        {
            cameraShakeEffect = new CameraShake() { Cam = VisualEffectHelper.Instance.DefaultCamera, Duration = 0.4f, Magnitude = 0.1f};
            hits = new RaycastHit2D[10];
        }
        
        public override void TriggerEffect(int effectId, IEffectTarget target, float size, float value, float stagger, PassiveEffectPool pool)
        {
            gameObject.SetActive(true);
            
            tempTarget = target;
            StartCoroutine(IEThunder(() =>
            {
                VfxThunderPool.Instance.GetAndRelease(null, tempTarget.Position, 0f, 1f);
                if (tempTarget.TargetTransform.TryGetComponent(out triggerredEnemy))
                {
                    VfxThunderPool.Instance.GetAndRelease(null, triggerredEnemy.transform.position, 0f, 1f);
                    triggerredEnemy.Damage(Mathf.RoundToInt(value), triggerredEnemy.transform.position, stagger, DamageType.Normal);
                    if (!triggerredEnemy.IsDestroyed && triggerredEnemy.PercentageHpLeft < size)
                    {
                        triggerredEnemy.Kill(DamageType.Normal);
                    }
                    sfx.Play();
                }
                    
                cameraShakeEffect.Duration = 0.3f;
                VisualEffectHelper.Instance.PlayEffect(cameraShakeEffect);
            }, () =>
            {
                pool.Release(this, effectId);
            }));
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