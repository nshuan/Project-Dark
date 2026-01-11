using System;
using Dark.Scripts.ForDemo;
using Dark.Scripts.Utils.Camera;
using TMPro;
using UnityEngine;

namespace OutGame.Upgrade.Tooltip
{
    public class UITooltip : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtKeyword;
        [SerializeField] private TextMeshProUGUI txtTooltip;
        [SerializeField] private RectTransform rectToolTip;
        [SerializeField] private RectTransform rectTransform;

        public void Show(string fullText, RectTransform parentRect, Vector2 padding)
        {
            if (!DemoConfig.ShowTooltip) return;
            var tooltip = TooltipConfig.Instance.TryGetTooltip(fullText);
            if (string.IsNullOrEmpty(tooltip.Item2))
            {
                rectToolTip.gameObject.SetActive(false);
                return;
            }
            
            var positionX = parentRect.anchoredPosition.x + parentRect.sizeDelta.x / 2 + rectTransform.sizeDelta.x / 2 + padding.x;
            var positionY = parentRect.anchoredPosition.y;
            
            // Check if the panel is outside the screen
            if (parentRect.position.x + parentRect.sizeDelta.x / 2 + rectTransform.sizeDelta.x + padding.x >
                SafeScaler.ScreenWidth)
            {
                positionX = parentRect.anchoredPosition.x;
                positionY = parentRect.anchoredPosition.y - ((RectTransform)parentRect.parent).sizeDelta.y / 2 - rectTransform.sizeDelta.y / 2 - padding.x;
                
                if (parentRect.position.y - ((RectTransform)parentRect.parent).sizeDelta.y / 2 - rectTransform.sizeDelta.y - padding.x < 0)
                {
                    positionY = parentRect.anchoredPosition.y + ((RectTransform)parentRect.parent).sizeDelta.y / 2 + rectTransform.sizeDelta.y / 2 + padding.x + 15;    
                }
            }
                
            rectToolTip.anchoredPosition = new Vector2(positionX, positionY);
            txtKeyword.SetText(tooltip.Item1);
            txtTooltip.SetText(tooltip.Item2);
            rectToolTip.gameObject.SetActive(true);
        }
    }
}