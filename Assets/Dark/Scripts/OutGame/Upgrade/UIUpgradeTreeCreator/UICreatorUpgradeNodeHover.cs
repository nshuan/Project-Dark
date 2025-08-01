using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.OutGame.Upgrade.UIUpgradeTreeCreator
{
    public class UICreatorUpgradeNodeHover : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public RectTransform rectTransform;

        public Action onDrag;

        public void OnBeginDrag(PointerEventData eventData) { }

        public void OnDrag(PointerEventData eventData)
        {
            if (!rectTransform) return;
            rectTransform.position += (Vector3)eventData.delta;
            onDrag?.Invoke();
        }

        public void OnEndDrag(PointerEventData eventData) { }
    }
}