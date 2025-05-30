using System;
using System.Collections;
using InGame.Effects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame.Trap
{
    public class LightningTrap : MonoTrap
    {
        private Coroutine trapCoroutine;
        
        public override void Setup(Camera cam, Vector3 position, Vector2 size, float damage, float duration, Action onComplete)
        {
            if (trapCoroutine != null) StopCoroutine(trapCoroutine);
            trapCoroutine = StartCoroutine(IETrap(cam, position, size.x, damage, duration, onComplete));
        }

        private IEnumerator IETrap(Camera cam, Vector3 position, float radius, float damage, float duration, Action onComplete)
        {
            // Lightning burst effect
            var lightning = RadialLightningPool.Instance.Get(null);
            position.z = 0;
            lightning.transform.position = position;
            lightning.Init();
            lightning.length = radius;
            lightning.Execute(duration);
            
            var checkCount = duration * RefreshRate;
            var cd = 1 / RefreshRate;
            while (checkCount > 0)
            {
                var hits = Physics2D.CircleCastAll(position, radius, Vector2.zero, 0f, LayerMask.GetMask("Entity"));
                foreach (var hit in hits)
                {
                    if (hit.collider && hit.transform.TryGetComponent<EnemyEntity>(out var enemyEntity))
                    {
                        enemyEntity.OnHit(GameStats.CalculateStat(LevelManager.Instance.GameStats.sLightningBurstDamage));
                        
                        // Chance to create lightning burst
                        if (!enemyEntity.IsInLightning && Random.Range(0f, 1f) <= LevelManager.Instance.GameStats.sLightningBurstChance)
                        {
                            enemyEntity.IsInLightning = true;
                            var lb = LightningTrapPool.Instance.Get(null);
                            lb.Setup(cam, enemyEntity.transform.position, 3f * Vector2.one, damage, 0.5f, () =>
                            {
                                enemyEntity.IsInLightning = false;
                            });
                            var effectCamShake = new CameraShake
                            {
                                Cam = cam,
                                Duration = 0.5f
                            };
                            EffectHelper.Instance.PlayEffect(effectCamShake);
                        }
                    }
                }

                checkCount -= 1;
                yield return new WaitForSeconds(cd);
            }
            
            onComplete?.Invoke();
            LightningTrapPool.Instance.Release(this);
        }
    }
}