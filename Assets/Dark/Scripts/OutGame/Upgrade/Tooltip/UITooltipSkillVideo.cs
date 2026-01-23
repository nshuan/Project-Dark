using System.Collections.Generic;
using Dark.Scripts.Utils.Camera;
using InGame.Upgrade;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace OutGame.Upgrade.Tooltip
{
    public class UITooltipSkillVideo : SerializedMonoBehaviour
    {
        [SerializeField] private RectTransform rectToolTip;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private TextMeshProUGUI txtTitle;
        [SerializeField] private Dictionary<int, GameObject> map;
        
        public bool Show(UpgradeNodeConfig nodeConfig, RectTransform parentRect, Vector2 padding)
        {
            if (map == null || !map.ContainsKey(nodeConfig.nodeId))
            {
                rectToolTip.gameObject.SetActive(false);
                return false;
            }
            
            var positionX = parentRect.anchoredPosition.x;
            var positionY = parentRect.anchoredPosition.y - ((RectTransform)parentRect.parent).sizeDelta.y / 2 - rectTransform.sizeDelta.y / 2 - padding.x;
                
            if (parentRect.position.y - ((RectTransform)parentRect.parent).sizeDelta.y / 2 - rectTransform.sizeDelta.y - padding.x < 0)
            {
                positionY = parentRect.anchoredPosition.y + ((RectTransform)parentRect.parent).sizeDelta.y / 2 + rectTransform.sizeDelta.y / 2 + padding.x + 15;    
            }
            
            rectToolTip.anchoredPosition = new Vector2(positionX, positionY);
            txtTitle.SetText(nodeConfig.nodeName);
            foreach (var vid in map.Values)
            {
                vid.gameObject.SetActive(false);
            }
            map[nodeConfig.nodeId].SetActive(true);
            rectToolTip.gameObject.SetActive(true);
            return true;
        }
    }
}