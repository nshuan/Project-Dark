using System;
using System.Collections.Generic;
using Core;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame.UI.HitShowDamage
{
    public class UIHitDameTextPool : MonoSingleton<UIHitDameTextPool>
    {
        [SerializeField] private TextMeshProUGUI prefab;
        [SerializeField] private UIHitDameCanvas manager;
        
        private Queue<TextMeshProUGUI> pool;
        private TextMeshProUGUI tempText;
        
        protected override void Awake()
        {
            base.Awake();
            
            pool = new Queue<TextMeshProUGUI>();
        }

        public void ShowDamage(int damage, Vector3 worldPos)
        {
            Get();
            manager.ShowDamage(tempText, damage, worldPos);
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

        [Serializable]
        public struct HitDamageTextColorInfo
        {
            public int threshold;
            public Color color;
        }
    }
}