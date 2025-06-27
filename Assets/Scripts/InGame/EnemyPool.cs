using System;
using System.Collections.Generic;
using Core;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame
{
    public class EnemyPool : SerializedMonoSingleton<EnemyPool>
    {
        private Dictionary<EnemyEntity, Queue<EnemyEntity>> queueDict;

        protected override void Awake()
        {
            base.Awake();
            
            queueDict = new Dictionary<EnemyEntity, Queue<EnemyEntity>>();
        }
        
        public EnemyEntity Get(EnemyEntity prefab, Transform targetParent, bool active = true)
        {
            EnemyEntity obj = null;
            if (queueDict.TryGetValue(prefab, out var pool))
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

        public void Release(EnemyEntity obj)
        {
            Destroy(obj.gameObject);
        }
    }
}