using System.Collections.Generic;
using UnityEngine;

namespace Dark.Scripts.Leaderboard.UI
{
    public sealed class ComponentPool<T> where T : Component
    {
        readonly Queue<T> _pool = new Queue<T>();

        public T Get(T prefab, Transform parent)
        {
            if (_pool.Count > 0)
            {
                var item = _pool.Dequeue();
                item.transform.SetParent(parent, false);
                item.gameObject.SetActive(true);
                return item;
            }

            return Object.Instantiate(prefab, parent, false);
        }

        public void Release(T item)
        {
            item.gameObject.SetActive(false);
            _pool.Enqueue(item);
        }

        public void Clear()
        {
            _pool.Clear();
        }
    }
}
