using System.Collections.Generic;
using Core;
using TMPro;
using UnityEngine;

namespace InGame.UI.HitShowDamage
{
    public class UIThunderInstantTextPool : MonoSingleton<UIThunderInstantTextPool>
    {
        [Space]
        [Header("Instant kill")]
        [SerializeField] private TextMeshProUGUI txtPrefab;
        [SerializeField] private UIFloatTextCanvas manager;
        [SerializeField] private Color textColor;
        
        private Queue<TextMeshProUGUI> poolTxtInstant;
        private TextMeshProUGUI tempText;
        
        public void ShowText(string text, Vector3 worldPos)
        {
            Get();
            manager.ShowText(tempText, text, worldPos, textColor, new Vector2(0f, 30f), 1f, Release);
        }
        
        public TextMeshProUGUI Get()
        {
            poolTxtInstant ??= new Queue<TextMeshProUGUI>();
            
            if (!poolTxtInstant.TryDequeue(out tempText))
            {
                tempText = Instantiate(txtPrefab, manager.transform);
            }
            
            tempText.gameObject.SetActive(false);
            return tempText;
        }

        public void Release(TextMeshProUGUI text)
        {
            text.gameObject.SetActive(false);
            poolTxtInstant.Enqueue(text);
        }
    }
}