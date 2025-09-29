using System;
using System.Collections;
using UnityEngine;

namespace Dark.Scripts.Utils
{
    public static class MonoUtility
    {
        public static Coroutine DelayCall(this MonoBehaviour target, float delay, Action callback)
        {
            return target.StartCoroutine(IEDelayCall(delay, callback));
        }
        
        public static bool TryDelayCall(this MonoBehaviour target, float delay, Action callback)
        {
            try
            {
                target.StartCoroutine(IEDelayCall(delay, callback));
            }
            catch (Exception e)
            {
                return false;
            }
            
            return true;
        }

        private static IEnumerator IEDelayCall(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback();
        }
    }
}