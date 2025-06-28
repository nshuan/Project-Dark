using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame
{
    public class EnemyPool : SerializedMonoSingleton<EnemyPool>
    {
        private Dictionary<int, Queue<EnemyEntity>> queueDict;

        protected override void Awake()
        {
            base.Awake();
            
            queueDict = new Dictionary<int, Queue<EnemyEntity>>();
        }
        
        public EnemyEntity Get(EnemyEntity prefab, int typeId, Transform targetParent, bool active = true)
        {
            EnemyEntity obj = null;
            if (queueDict.TryGetValue(typeId, out var pool))
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

        public void Release(EnemyEntity obj, int typeId)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(transform);
            queueDict.TryAdd(typeId, new Queue<EnemyEntity>());
            queueDict[typeId].Enqueue(obj);
        }

        public void Release(EnemyEntity obj, float delay)
        {
            Destroy(obj.gameObject, delay);
        }
    }
}