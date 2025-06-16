using System;
using System.Collections.Generic;
using Core;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame
{
    public class EnemyPool : SerializedMonoSingleton<EnemyPool>
    {
        [NonSerialized, OdinSerialize] private Dictionary<EnemyType, EnemyEntity> prefabDict;

        private Dictionary<EnemyType, Queue<EnemyEntity>> queueDict;

        protected override void Awake()
        {
            base.Awake();

            queueDict = new Dictionary<EnemyType, Queue<EnemyEntity>>();
        }

        public EnemyEntity Get(EnemyType type, Transform targetParent, bool active = true)
        {
            EnemyEntity obj = null;
            if (queueDict.TryGetValue(type, out var pool))
            {
                if (pool.TryDequeue(out obj))
                {
                    obj.transform.SetParent(targetParent);
                    obj.gameObject.SetActive(active);
                    return obj;
                }
            }

            if (!prefabDict.TryGetValue(type, out var prefab)) return null;
            
            obj = Instantiate(prefab, targetParent);
            obj.gameObject.SetActive(active);
            return obj;

        }

        public void Release(EnemyType type, EnemyEntity obj)
        {
            obj.transform.SetParent(transform);
            obj.gameObject.SetActive(false);

            if (!queueDict.TryGetValue(type, out var pool))
            {
                pool = new Queue<EnemyEntity>();
                queueDict[type] = pool;
            }
            
            if (pool.Contains(obj)) return;
            pool.Enqueue(obj);
        }
    }
}