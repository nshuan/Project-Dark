using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class Pool<T, K> : MonoSingleton<K> where T : MonoBehaviour where K : Pool<T, K>
    {
        [SerializeField] protected T _prefab;
        
        protected Queue<T> pool = new();

        public virtual T Get(Transform targetParent, bool active = true)
        {
            if (pool.TryDequeue(out var obj))
            {
                obj.transform.SetParent(targetParent);
                obj.gameObject.SetActive(active);
                return obj;
            }
            
            obj = Instantiate(_prefab, targetParent);
            obj.gameObject.SetActive(active);
            return obj;
        }

        public virtual void Release(T obj)
        {
            obj.transform.SetParent(transform);
            obj.gameObject.SetActive(false);
            if (pool.Contains(obj)) return;
            pool.Enqueue(obj);
        }
    }
}