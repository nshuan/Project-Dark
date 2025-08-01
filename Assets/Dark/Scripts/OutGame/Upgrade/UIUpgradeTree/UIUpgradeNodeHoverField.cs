using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeNodeHoverField : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Action onHover;
        public Action onHoverExit;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            onHover?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onHoverExit?.Invoke();
        }
    }
}