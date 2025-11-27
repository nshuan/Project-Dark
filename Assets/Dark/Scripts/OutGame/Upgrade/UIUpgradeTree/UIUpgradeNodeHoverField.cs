using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeNodeHoverField : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Action onHover;
        public Action onHoverExit;
        public Action onPointerClick;
        
        public RectTransform nodeRepresentableRect;
        public bool interactable;

        private void Awake()
        {
            nodeRepresentableRect ??= GetComponent<RectTransform>();
        }

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
            if (!interactable) return;
            onPointerClick?.Invoke();
        }
    }
}