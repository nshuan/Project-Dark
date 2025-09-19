using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Economic;
using UnityEngine;

namespace InGame.Economic.DropItems
{
    public class EItemDropPool : MonoSingleton<EItemDropPool>
    {
        [SerializeField] private EItemDropPoolInfo[] prefabs;

        private Dictionary<WealthType, Queue<EItemDrop>> poolMap;
        private EItemDrop tempItem;

        protected override void Awake()
        {
            base.Awake();
            
            poolMap = new Dictionary<WealthType, Queue<EItemDrop>>();
            foreach (WealthType kind in Enum.GetValues(typeof(WealthType)))
            {
                poolMap.Add(kind, new Queue<EItemDrop>());
            }
        }

        public void Get(WealthType kind, Action<EItemDrop> onItemSpawned)
        {
            Get(kind);
            onItemSpawned?.Invoke(tempItem);
        }

        public EItemDrop Get(WealthType kind)
        {
            if (poolMap.ContainsKey(kind))
            {
                if (!poolMap[kind].TryDequeue(out tempItem))
                {
                    var prefabInfo = prefabs.FirstOrDefault((item) => item.kind == kind);
                    if (prefabInfo == null) return null;
                    tempItem = Instantiate(prefabInfo.prefab, transform);
                }
            }
            
            tempItem.gameObject.SetActive(false);
            return tempItem;
        }

        public void Release(EItemDrop item)
        {
            item.gameObject.SetActive(false);
            if (poolMap.ContainsKey(item.kind))
            {
                poolMap[item.kind].Enqueue(item);
            }
        }

        [Serializable]
        private class EItemDropPoolInfo
        {
            public WealthType kind;
            public EItemDrop prefab;
        }
    }
}