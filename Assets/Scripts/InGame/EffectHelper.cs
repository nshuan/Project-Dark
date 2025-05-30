using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace InGame
{
    public class EffectHelper : MonoSingleton<EffectHelper>
    {
        private Dictionary<Type, IEffect> effectMap;
        private Dictionary<Type, Coroutine> effectCoroutineMap;

        protected override void Awake()
        {
            base.Awake();

            effectMap = new Dictionary<Type, IEffect>();
            effectCoroutineMap = new Dictionary<Type, Coroutine>();
        }

        public void PlayEffect<T>(T effectInstance) where T : IEffect
        {
            var t = typeof(T);

            if (!effectMap.ContainsKey(t) && effectInstance == null) return;
            if (effectCoroutineMap.TryGetValue(t, out var coroutine)) StopCoroutine(coroutine);

            if (effectMap.TryGetValue(t, out var effect))
            {
                if (effectInstance != null)
                    effect.CloneStats(effectInstance);
                
                effectMap[t] = effect;
                effectCoroutineMap[t] = StartCoroutine(effect.DoEffect());
            }
            else
            {
                effectMap[t] = effectInstance;
                effectCoroutineMap[t] = StartCoroutine(effectInstance.DoEffect());
            }
        }
    }

    public interface IEffect
    {
        float Duration { get; set; }
        IEnumerator DoEffect();
        void CloneStats(IEffect target);
    }
}