using System;
using System.Collections;
using UnityEngine;

namespace InGame
{
    public class PassiveThunder : MonoPassiveEntity
    {
        [SerializeField] private int maxTriggeredEnemies = 20;
        [SerializeField] private bool triggerAllEnemies = false;
        
        private EnemyEntity[] triggerredEnemies;
        private RaycastHit2D[] hits = new RaycastHit2D[50];
        private IEffectTarget tempTarget;
        
        public override void TriggerEffect(int effectId, IEffectTarget target, float size, float value, float stagger, PassiveEffectPool pool)
        {
            triggerredEnemies ??= new EnemyEntity[maxTriggeredEnemies];

            if (triggerAllEnemies)
            {
                StartCoroutine(IEThunder(() =>
                {
                    var triggerCount = EnemyManager.Instance.FilterEnemiesNonAlloc(enemy => enemy.PercentageHpLeft < value, ref triggerredEnemies);
                    while (triggerCount > 0)
                    {
                        triggerredEnemies[triggerCount - 1].Kill();
                        triggerCount -= 1;
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
                                if (tempTarget.PercentageHpLeft < value)
                                {
                                    tempTarget.Kill();
                                }
                            }
                        }
                    }
                }, () =>
                {
                    pool.Release(this, effectId);
                }));
            }
        }
        
        private IEnumerator IEThunder(Action actionDamage, Action actionComplete)
        {
            yield return new WaitForSeconds(0.5f);
            actionDamage?.Invoke();

            yield return new WaitForSeconds(1f);
            actionComplete?.Invoke();
        }
    }
}