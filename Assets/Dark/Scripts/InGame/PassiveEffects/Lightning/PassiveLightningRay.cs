using System;
using System.Collections;
using Dark.Scripts.Audio;
using InGame.Effects;
using UnityEngine;

namespace InGame
{
    public class PassiveLightningRay : MonoPassiveEntity
    {
        // [SerializeField] private LightningLineRenderer lineRenderer;
        [SerializeField] private LightningLineRendererV2 lineRenderer;
        [SerializeField] private Transform vfxImpact;
        [SerializeField] private int maxHit = 5;
        [SerializeField] private float delayEachHit = 0.05f;
        [SerializeField] private float durationEachHit = 0.13f;
        [SerializeField] private float durationImpact = 1.2f;
        [SerializeField] private AudioComponent sfx;
        
        private Vector2 Position { get; set; }
        private float Stagger { get; set; }
        private RaycastHit2D[] hits = new RaycastHit2D[50];
        private EnemyEntity[] unorderedEnemies = new EnemyEntity[50];
        private EnemyEntity[] enemyOrder;
        private LightningBall[] unorderedHits = new LightningBall[50];
        private LightningBall[] hitOrder;
        private Vector2 anchorForOrdering;
        private int tempClosestHitIndex;
        private float tempMinDistance = 100f;
        private float tempDistance;
        private int orderCount;
        private IDamageable hitTarget;
        private CameraShake cameraShakeEffect;
        private int maxHitWithBonus;
        private EnemyEntity tempEnemy;

        private void OnDisable()
        {
            lineRenderer.gameObject.SetActive(false);
        }

        public override void Initialize()
        {
            hitOrder = new LightningBall[20];
            enemyOrder = new EnemyEntity[20];
            lineRenderer.gameObject.SetActive(false);
            lineRenderer.Initialize();
            lineRenderer.ResetLine(null);
            cameraShakeEffect = new CameraShake() { Cam = VisualEffectHelper.Instance.DefaultCamera };
        }

        public override void TriggerEffect(int effectId, IEffectTarget target, float size, float value, float stagger, PassiveEffectPool pool)
        {
            maxHitWithBonus = (int)size;
            lineRenderer.ResetLine(null);
            transform.position = Vector3.zero;
            this.Position = target.Position;
            this.Stagger = stagger;
            gameObject.SetActive(true);

            var count = Physics2D.CircleCastNonAlloc(Position, 3f, Vector2.zero, hits, 0f, targetLayer);
            var lightningBall = LightningBallPool.Instance.Get(null, true);
            lightningBall.transform.position = Position;
            lightningBall.Target = target.TargetTransform;
            unorderedHits[0] = lightningBall;
            unorderedEnemies[0] = target.TargetTransform.GetComponent<EnemyEntity>();
            var unorderedHitIndex = 1;
            for (var i = 0; i < count; i++)
            {
                if (hits[i].transform.TryGetComponent(out tempEnemy) && !ReferenceEquals(tempEnemy.transform, target.TargetTransform) && tempEnemy.IsDestroyed == false)
                {
                    lightningBall = LightningBallPool.Instance.Get(null, true);
                    lightningBall.transform.position = tempEnemy.transform.position;
                    lightningBall.Target = tempEnemy.transform;
                    unorderedHits[unorderedHitIndex] = lightningBall;
                    unorderedEnemies[unorderedHitIndex] = tempEnemy;
                    unorderedHitIndex += 1;
                }
            }

            count = unorderedHitIndex;
            anchorForOrdering.x = target.Position.x;
            anchorForOrdering.y = target.Position.y;
            orderCount = 0;
            while (orderCount < maxHitWithBonus && orderCount < count)
            {
                for (var i = 0; i < count; i++)
                {
                    if (!unorderedHits[i]) continue;
                    tempDistance = Vector2.Distance(unorderedHits[i].transform.position, anchorForOrdering);
                    if (tempDistance < tempMinDistance)
                    {
                        tempClosestHitIndex = i;
                        tempMinDistance = tempDistance;
                    }
                }

                hitOrder[orderCount] = unorderedHits[tempClosestHitIndex];
                enemyOrder[orderCount] = unorderedEnemies[tempClosestHitIndex];
                unorderedHits[tempClosestHitIndex] = null;
                unorderedEnemies[tempClosestHitIndex] = null;
                var a = orderCount;
                enemyOrder[orderCount].OnDead += () =>
                {
                    lineRenderer.ActiveAnchor(a, false);
                };
                
                anchorForOrdering.x = hitOrder[orderCount].transform.position.x;
                anchorForOrdering.y = hitOrder[orderCount].transform.position.y;
                tempMinDistance = 100f;
                orderCount += 1;
            }
            
            lineRenderer.ResetLine(hitOrder);

            StartCoroutine(IELightningRay(value, () =>
            {
                StartCoroutine(IEDelayRelease(2f, () => pool.Release(this, effectId)));
            }));
            vfxImpact.position = Position;
            vfxImpact.gameObject.SetActive(true);
            sfx.Play();
            StartCoroutine(IEDelayHideVfxImpact(durationImpact));
            cameraShakeEffect.Duration = Mathf.Max(orderCount * delayEachHit, durationEachHit);
            VisualEffectHelper.Instance.PlayEffect(cameraShakeEffect);
        }

        private IEnumerator IELightningRay(float damage, Action actionComplete)
        {
            lineRenderer.gameObject.SetActive(true);
            
            for (var i = 0; i < orderCount; i++)
            {
                lineRenderer.ActiveAnchor(i, true);
                hitOrder[i].ShowVfx();
                StartCoroutine(IEDelayHideAnchor(i, durationEachHit));
                StartCoroutine(IEDelayHideVfxMiniImpact(hitOrder[i], durationImpact));
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
                        hitTarget.HitDirectionX = hitOrder[i].transform.position.x - hitOrder[i - 1].transform.position.x;
                        hitTarget.HitDirectionY = hitOrder[i].transform.position.y - hitOrder[i - 1].transform.position.y;
                        hitTarget.Damage((int)damage, hitOrder[i - 1].transform.position, Stagger, DamageType.Normal);
                    }
                }

                yield return new WaitForSeconds(delayEachHit);
            }
            
            // yield return new WaitForSeconds(1f);
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

        private IEnumerator IEDelayHideVfxMiniImpact(LightningBall ball, float delay)
        {
            yield return new WaitForSeconds(delay);
            ball.HideVfx();
        }

        private IEnumerator IEDelayRelease(float delay, Action actionDelay)
        {
            yield return new WaitForSeconds(delay);
            lineRenderer.ResetLine(null);
            actionDelay?.Invoke();
        }
    }
}