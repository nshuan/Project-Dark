using Core;
using UnityEngine;

namespace InGame.Trap
{
    public class LightningTrapPool : Pool<LightningTrap, LightningTrapPool>
    {
        public override LightningTrap Get(Transform targetParent, bool active = true)
        {
            if (pool.TryDequeue(out var obj))
            {
                obj.transform.SetParent(targetParent);
                obj.gameObject.SetActive(active);
                return obj;
            }
            
            _prefab = new GameObject("Lightning Trap").AddComponent<LightningTrap>();
            _prefab.transform.SetParent(transform);
            _prefab.gameObject.SetActive(false);
            obj = Instantiate(_prefab, targetParent);
            obj.gameObject.SetActive(active);;
            return obj;
        }
    }
}