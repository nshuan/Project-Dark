using System;
using System.Collections;
using System.Collections.Generic;
using InGame.Effects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame.Trap
{
    public class LightningTrap : MonoTrap
    {
        private Coroutine trapCoroutine;

        private List<(Lightning, EnemyEntity)> lightningEffects;
        
        private EnemyEntity Target { get; set; }
        
        public override void Setup(Camera cam, EnemyEntity target, Vector2 size, float damage, float duration, Action onComplete)
        {
            Target = target;
            Target.OnDead += OnTrappedEnemyDead;
            lightningEffects = new List<(Lightning, EnemyEntity)>();
            
            if (trapCoroutine != null) StopCoroutine(trapCoroutine);
            trapCoroutine = StartCoroutine(IETrap(cam, size.x, damage, duration, onComplete));
        }

        private IEnumerator IETrap(Camera cam, float radius, float damage, float duration, Action onComplete)
        {
            var checkCount = duration * RefreshRate;
            var cd = 1 / RefreshRate;
            while (checkCount > 0 && Target)
            {
                var target = Target.transform;
                var hits = Physics2D.CircleCastAll(target.position, radius, Vector2.zero, 0f, LayerMask.GetMask("Entity"));
                foreach (var hit in hits)
                {
                    if (hit.collider && hit.transform.TryGetComponent<EnemyEntity>(out var enemyEntity))
                    {
                        // Chance to create lightning burst
                        if (!enemyEntity.IsInLightning && !enemyEntity.IsDead && Random.Range(0f, 1f) <= LevelManager.Instance.GameStats.sLightningChance)
                        {
                            // Lightning ray effect
                            var lightning = LightningPool.Instance.Get(null);
                            lightning.points = new List<Transform>() { target, enemyEntity.transform };
                            var pair = (lightning, enemyEntity);
                            lightningEffects.Add(pair);
                            lightning.Execute(duration);
                            
                            enemyEntity.IsInLightning = true;
                            enemyEntity.OnDead += () =>
                            {
                                lightning.ForceStop();
                                lightningEffects.Remove(pair);
                            };
                            
                            var skill = LightningTrapPool.Instance.Get(null);
                            skill.Setup(cam, enemyEntity, 3f * Vector2.one, damage, 0.5f, () =>
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
                        
                        enemyEntity.OnHit(GameStats.CalculateStat(LevelManager.Instance.GameStats.sLightningDamage));

                    }
                }

                checkCount -= 1;
                yield return new WaitForSeconds(cd);
            }
            
            onComplete?.Invoke();
            LightningTrapPool.Instance.Release(this);
        }

        private void OnTrappedEnemyDead()
        {
            Target = null;
            if (trapCoroutine != null) StopCoroutine(trapCoroutine);
            foreach (var effect in lightningEffects)
            {
                effect.Item1.ForceStop();
                LightningPool.Instance.Release(effect.Item1);
                effect.Item2.IsInLightning = false;
            }
            lightningEffects.Clear();
        }
    }
}