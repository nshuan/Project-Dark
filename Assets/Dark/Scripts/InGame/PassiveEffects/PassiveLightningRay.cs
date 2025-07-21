using System;
using System.Collections;
using UnityEngine;

namespace InGame
{
    public class PassiveLightningRay : MonoPassiveEntity
    {
        [SerializeField] private LightningLineRenderer lineRenderer;
        [SerializeField] private int maxHit = 5;
        [SerializeField] private float delayEachHit = 0.05f;
        [SerializeField] private float durationEachHit = 0.13f;
        
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

        public override void Initialize()
        {
            hitOrder = new Transform[maxHit];
            lineRenderer.Initialize(maxHit);
        }

        public override void TriggerEffect(int effectId, IEffectTarget target, float size, float value, float stagger, PassiveEffectPool pool)
        {
            lineRenderer.ResetLine(maxHit, null, 0);
            transform.position = Vector3.zero;
            this.Position = target.Position;
            this.Stagger = stagger;

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

            StartCoroutine(IELightningRay(value, () => pool.Release(this, effectId)));
        }

        private IEnumerator IELightningRay(float damage, Action actionComplete)
        {
            for (var i = 0; i < orderCount; i++)
            {
                lineRenderer.ActiveAnchor(i, true);
                StartCoroutine(IEDelayHideAnchor(i, durationEachHit));
                if (hitOrder[i].TryGetComponent(out hitTarget))
                {
                    hitTarget.Damage((int)damage, Position, Stagger);
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
    }
}