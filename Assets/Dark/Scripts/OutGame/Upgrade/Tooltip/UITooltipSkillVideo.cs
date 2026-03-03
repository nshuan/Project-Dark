using System.Collections.Generic;
using Dark.Scripts.Utils.Camera;
using Dark.Tools.Language.Runtime;
using Data;
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
        [SerializeField] private Dictionary<int, GameObject[]> map;
        
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
            txtTitle.SetText(LanguageData.Instance.GetLocalizedString(nodeConfig.nodeNameKey, LanguageManager.Instance.CurrentLanguage));
            foreach (var vid in map.Values)
            {
                foreach (var v in vid)
                {
                    v.gameObject.SetActive(false);
                }
            }
            var classType = 0;
            if (map[nodeConfig.nodeId].Length > 1)
                classType = PlayerDataManager.Instance.Data.characterClass;
            if (classType >= map[nodeConfig.nodeId].Length)
                classType = 0;
            for (var i = 0; i < map[nodeConfig.nodeId].Length; i++)
            {
                map[nodeConfig.nodeId][i].SetActive(i == classType);
            }
            map[nodeConfig.nodeId][classType].SetActive(true);
            rectToolTip.gameObject.SetActive(true);
            return true;
        }
    }
}