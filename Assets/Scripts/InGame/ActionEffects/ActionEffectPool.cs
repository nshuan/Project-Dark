using System.Collections.Generic;
using Core;
using UnityEngine;

namespace InGame
{
    public class ActionEffectPool : Singleton<ActionEffectPool>
    {
        private Transform poolObject;
        private Dictionary<int, Queue<ActionEffectEntity>> queueDict;

        public ActionEffectPool()
        {
            queueDict = new Dictionary<int, Queue<ActionEffectEntity>>();
            poolObject = new GameObject("Action Effect Pool").transform;
        }
        
        public ActionEffectEntity Get(ActionEffectEntity prefab, int effectId, Transform targetParent, bool active = true)
        {
            ActionEffectEntity obj = null;
            if (queueDict.TryGetValue(effectId, out var pool))
            {
                if (pool.TryDequeue(out obj))
                {
                    obj.transform.SetParent(targetParent);
                    obj.gameObject.SetActive(active);
                    return obj;
                }
            } 
            
            obj = Object.Instantiate(prefab, targetParent);
            obj.gameObject.SetActive(active);
            return obj;

        }

        public void Release(ActionEffectEntity obj, int effectId)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(poolObject);
            queueDict.TryAdd(effectId, new Queue<ActionEffectEntity>());
            queueDict[effectId].Enqueue(obj);
        }
    }
}