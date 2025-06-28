using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace InGame
{
    public class ActionEffectManager : MonoSingleton<ActionEffectManager>
    {
        private Dictionary<int, bool> effectPendingMap;
        private Dictionary<int, Coroutine> effectCoroutineMap;

        protected override void Awake()
        {
            base.Awake();

            effectPendingMap = new Dictionary<int, bool>();
            effectCoroutineMap = new Dictionary<int, Coroutine>();
        }

        public void CheckAndTrigger(Vector2 position, ActionEffectConfig effectConfig)
        {
            effectPendingMap.TryAdd(effectConfig.effectId, false);

            if (effectPendingMap[effectConfig.effectId] == false)
            {
                // Check trigger chance
                if (Random.Range(0f, 1f) > effectConfig.chance) return;

                effectCoroutineMap.TryAdd(effectConfig.effectId, null);
                var coroutine = effectCoroutineMap[effectConfig.effectId];
                if (coroutine != null)
                    StopCoroutine(coroutine);
                effectPendingMap[effectConfig.effectId] = true;
                coroutine = StartCoroutine(IETrigger(position, effectConfig));
                effectCoroutineMap[effectConfig.effectId] = coroutine;
            }
        }

        private IEnumerator IETrigger(Vector2 position, ActionEffectConfig effectConfig)
        {
            effectConfig.logicType.TriggerEffect(
                effectConfig.effectId,
                position,
                effectConfig.size,
                effectConfig.value);
            yield return new WaitForSeconds(effectConfig.cooldown);
            effectPendingMap[effectConfig.effectId] = false;
        }
    }
}