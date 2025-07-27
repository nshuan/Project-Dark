using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace InGame
{
    public class ProjectilePool : MonoSingleton<ProjectilePool>
    {
        private Dictionary<Type, Queue<ProjectileEntity>> queueDict;

        protected override void Awake()
        {
            base.Awake();

            queueDict = new Dictionary<Type, Queue<ProjectileEntity>>();
        }

        public ProjectileEntity Get(ProjectileEntity prefab, Transform targetParent, bool active = true)
        {
            var type = prefab.GetType();
            ProjectileEntity obj = null;
            if (queueDict.TryGetValue(type, out var pool))
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

        public void Release(ProjectileEntity obj)
        {
            var type = obj.GetType();
            obj.transform.SetParent(transform);
            obj.gameObject.SetActive(false);

            if (!queueDict.TryGetValue(type, out var pool))
            {
                pool = new Queue<ProjectileEntity>();
                queueDict[type] = pool;
            }
            
            if (pool.Contains(obj)) return;
            pool.Enqueue(obj);
        }
    }
}