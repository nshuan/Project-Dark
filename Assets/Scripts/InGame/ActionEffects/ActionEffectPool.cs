using System;
using System.Collections.Generic;
using Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InGame
{
    public class ActionEffectPool : MonoBehaviour
    {
        private Dictionary<int, Queue<MonoEffectEntity>> queueDict;

        private void Awake()
        {
            queueDict = new Dictionary<int, Queue<MonoEffectEntity>>();
        }

        public MonoEffectEntity Get(MonoEffectEntity prefab, int effectId, Transform targetParent, bool active = true)
        {
            MonoEffectEntity obj = null;
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

        public void Release(MonoEffectEntity obj, int effectId)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(transform);
            queueDict.TryAdd(effectId, new Queue<MonoEffectEntity>());
            queueDict[effectId].Enqueue(obj);
        }
    }
}