using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeNodeHoverField : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Action onHover;
        public Action onHoverExit;
        public Action onPointerClick;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            onHover?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onHoverExit?.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onPointerClick?.Invoke();
        }
    }
}