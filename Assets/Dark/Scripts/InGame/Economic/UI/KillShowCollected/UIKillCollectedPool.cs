using System;
using System.Collections.Generic;
using Core;
using InGame.UI.HitShowDamage;
using TMPro;
using UnityEngine;

namespace InGame.UI.Economic.KillShowCollected
{
    public class UIKillCollectedPool : MonoSingleton<UIKillCollectedPool>
    {
        [SerializeField] private TextMeshProUGUI prefab;
        [SerializeField] private UIKillCollected manager;
        
        private Queue<TextMeshProUGUI> pool;
        private TextMeshProUGUI tempText;
        
        protected override void Awake()
        {
            base.Awake();
            
            pool = new Queue<TextMeshProUGUI>();
        }

        public void ShowCollected(int value, Vector3 worldPos)
        {
            Get();
            manager.ShowCollected(tempText, value, worldPos);
        }
        
        public TextMeshProUGUI Get()
        {
            if (!pool.TryDequeue(out tempText))
            {
                tempText = Instantiate(prefab, manager.transform);
            }
            
            tempText.gameObject.SetActive(false);
            return tempText;
        }

        public void Release(TextMeshProUGUI text)
        {
            text.gameObject.SetActive(false);
            pool.Enqueue(text);
        }
    }
}