using System.Collections;
using Core;
using UnityEngine;

namespace InGame
{
    public class EffectHelper : MonoSingleton<EffectHelper>
    {
        private Coroutine effectCoroutine;
        
        public void PlayEffect<T>(T effectInstance) where T : IEffect
        {
            if (effectInstance == null) return;
            if (effectCoroutine != null) StopCoroutine(effectCoroutine);

            effectCoroutine = StartCoroutine(effectInstance.DoEffect());
        }
    }

    public interface IEffect
    {
        IEnumerator DoEffect();
    }
}