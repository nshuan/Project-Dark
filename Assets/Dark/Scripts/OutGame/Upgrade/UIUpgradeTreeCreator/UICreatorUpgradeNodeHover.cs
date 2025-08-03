using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.OutGame.Upgrade.UIUpgradeTreeCreator
{
    public class UICreatorUpgradeNodeHover : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        public RectTransform rectTransform;

        public Action onDrag;
        public Action onClick;

        private bool isDrag;

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDrag = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!rectTransform) return;
            rectTransform.position += (Vector3)eventData.delta;
            onDrag?.Invoke();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDrag = false;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (isDrag) return;
            onClick?.Invoke();
        }
    }
}