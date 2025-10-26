using System;
using System.Collections;
using Dark.Scripts.Audio;
using InGame.Effects;
using UnityEngine;

namespace InGame
{
    public class PassiveLightningRay : MonoPassiveEntity
    {
        [SerializeField] private LightningLineRenderer lineRenderer;
        [SerializeField] private Transform vfxImpact;
        [SerializeField] private int maxHit = 5;
        [SerializeField] private float delayEachHit = 0.05f;
        [SerializeField] private float durationEachHit = 0.13f;
        [SerializeField] private AudioComponent sfx;
        
        private Vector2 Position { get; set; }
        private float Stagger { get; set; }
        private RaycastHit2D[] hits = new RaycastHit2D[50];
        private Transform[] unorderedHits = new Transform[50];
        private Transform[] hitOrder;
        private Vector2 anchorForOrdering;
        private int tempClosestHitIndex;
        private float tempMinDistance = 100f;
        private float tempDistance;
        private int orderCount;
        private IDamageable hitTarget;
        private CameraShake cameraShakeEffect;

        public override void Initialize()
        {
            hitOrder = new Transform[maxHit];
            lineRenderer.Initialize(maxHit);
            cameraShakeEffect = new CameraShake() { Cam = VisualEffectHelper.Instance.DefaultCamera };
        }

        public override void TriggerEffect(int effectId, IEffectTarget target, float size, float value, float stagger, PassiveEffectPool pool)
        {
            lineRenderer.ResetLine(maxHit, null, 0);
            transform.position = Vector3.zero;
            this.Position = target.Position;
            this.Stagger = stagger;
            gameObject.SetActive(true);

            var count = Physics2D.CircleCastNonAlloc(Position, size, Vector2.zero, hits, 0f, targetLayer);
            for (var i = 0; i < count; i++)
            {
                unorderedHits[i] = hits[i].transform;
            }

            anchorForOrdering.x = target.Position.x;
            anchorForOrdering.y = target.Position.y;
            orderCount = 0;
            while (orderCount < maxHit && orderCount < count)
            {
                for (var i = 0; i < count; i++)
                {
                    if (!unorderedHits[i]) continue;
                    tempDistance = Vector2.Distance(unorderedHits[i].position, anchorForOrdering);
                    if (tempDistance < tempMinDistance)
                    {
                        tempClosestHitIndex = i;
                        tempMinDistance = tempDistance;
                    }
                }

                hitOrder[orderCount] = unorderedHits[tempClosestHitIndex];
                unorderedHits[tempClosestHitIndex] = null;
                
                anchorForOrdering.x = hitOrder[orderCount].position.x;
                anchorForOrdering.y = hitOrder[orderCount].position.y;
                tempMinDistance = 100f;
                orderCount += 1;
            }
            
            lineRenderer.ResetLine(maxHit, hitOrder, orderCount);

            StartCoroutine(IELightningRay(value, () =>
            {
                lineRenderer.ResetLine(maxHit, null, 0);
                pool.Release(this, effectId);
            }));
            vfxImpact.position = Position;
            vfxImpact.gameObject.SetActive(true);
            sfx.Play();
            StartCoroutine(IEDelayHideVfxImpact(durationEachHit));
            cameraShakeEffect.Duration = Mathf.Max(orderCount * delayEachHit, durationEachHit);
            VisualEffectHelper.Instance.PlayEffect(cameraShakeEffect);
        }

        private IEnumerator IELightningRay(float damage, Action actionComplete)
        {
            for (var i = 0; i < orderCount; i++)
            {
                lineRenderer.ActiveAnchor(i, true);
                StartCoroutine(IEDelayHideAnchor(i, durationEachHit));
                if (hitOrder[i].TryGetComponent(out hitTarget))
                {
                    if (i == 0)
                    {
                        hitTarget.HitDirectionX = 0f;
                        hitTarget.HitDirectionY = 0f;
                        hitTarget.Damage((int)damage, Position, Stagger, DamageType.Normal);
                    }
                    else
                    {
                        hitTarget.HitDirectionX = hitOrder[i].position.x - hitOrder[i - 1].position.x;
                        hitTarget.HitDirectionY = hitOrder[i].position.y - hitOrder[i - 1].position.y;
                        hitTarget.Damage((int)damage, hitOrder[i - 1].position, Stagger, DamageType.Normal);
                    }
                }

                yield return new WaitForSeconds(delayEachHit);
            }
            
            yield return new WaitForSeconds(1f);
            actionComplete?.Invoke();
        }

        private IEnumerator IEDelayHideAnchor(int index, float delay)
        {
            yield return new WaitForSeconds(delay);
            lineRenderer.ActiveAnchor(index, false);
        }

        private IEnumerator IEDelayHideVfxImpact(float delay)
        {
            yield return new WaitForSeconds(delay);
            vfxImpact.gameObject.SetActive(false);
        }
    }
}