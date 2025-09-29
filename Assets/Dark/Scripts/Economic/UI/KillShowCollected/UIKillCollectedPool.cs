using System;
using System.Collections.Generic;
using Core;
using Economic;
using InGame.UI.HitShowDamage;
using TMPro;
using UnityEngine;

namespace Economic.UI.KillShowCollected
{
    public class UIKillCollectedPool : MonoSingleton<UIKillCollectedPool>
    {
        [SerializeField] private TextMeshProUGUI[] prefab;
        [SerializeField] private UIKillCollected manager;
        
        private Dictionary<WealthType, Queue<TextMeshProUGUI>> poolMap;
        private TextMeshProUGUI tempText;
        
        protected override void Awake()
        {
            base.Awake();
            
            poolMap = new Dictionary<WealthType, Queue<TextMeshProUGUI>>();
            foreach (WealthType kind in Enum.GetValues(typeof(WealthType)))
            {
                poolMap.Add(kind, new Queue<TextMeshProUGUI>());
            }
        }

        public void ShowCollected(WealthType kind, int value, Vector3 worldPos)
        {
            Get(kind);
            manager.ShowCollected(kind, value, tempText, worldPos);
        }
        
        public TextMeshProUGUI Get(WealthType kind)
        {
            if (poolMap.ContainsKey(kind))
            {
                if (!poolMap[kind].TryDequeue(out tempText))
                {
                    tempText = Instantiate(prefab[(int)kind], manager.transform);
                }
            }
            
            tempText.gameObject.SetActive(false);
            return tempText;
        }

        public void Release(WealthType kind, TextMeshProUGUI text)
        {
            text.gameObject.SetActive(false);
            if (poolMap.ContainsKey(kind))
            {
                poolMap[kind].Enqueue(text);
            }
        }
    }
}