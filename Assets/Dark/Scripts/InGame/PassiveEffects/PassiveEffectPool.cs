using System;
using System.Collections.Generic;
using Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InGame
{
    public class PassiveEffectPool : MonoBehaviour
    {
        private Dictionary<int, Queue<MonoPassiveEntity>> queueDict;

        private void Awake()
        {
            queueDict = new Dictionary<int, Queue<MonoPassiveEntity>>();
        }

        public MonoPassiveEntity Get(MonoPassiveEntity prefab, int effectId, Transform targetParent, bool active = true)
        {
            MonoPassiveEntity obj = null;
            if (queueDict.TryGetValue(effectId, out var pool))
            {
                if (pool.TryDequeue(out obj))
                {
                    obj.transform.SetParent(targetParent);
                    obj.gameObject.SetActive(active);
                    return obj;
                }
            } 
            
            obj = Instantiate(prefab, targetParent);
            obj.gameObject.SetActive(active);
            return obj;

        }

        public void Release(MonoPassiveEntity obj, int effectId)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(transform);
            queueDict.TryAdd(effectId, new Queue<MonoPassiveEntity>());
            queueDict[effectId].Enqueue(obj);
        }
    }
}