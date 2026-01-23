using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Dark.Scripts.Common
{
    public class UIInteractiveHoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private RectTransform rectCheckHoverOut;
        [SerializeField] private ButtonHoverEvent actionHoverIn;
        [SerializeField] private ButtonHoverEvent actionHoverOut;
 
        private bool flagCheckHoverOut = false;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (flagCheckHoverOut) return;
            actionHoverIn?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            flagCheckHoverOut = true;
        }

        private void Update()
        {
            if (!flagCheckHoverOut) return;
            
            var mousePos = Input.mousePosition;
            if (mousePos.x < rectCheckHoverOut.position.x - rectCheckHoverOut.sizeDelta.x * UICanvasGetScaleFactor.scaleFactor / 2f
                || mousePos.x > rectCheckHoverOut.position.x + rectCheckHoverOut.sizeDelta.x * UICanvasGetScaleFactor.scaleFactor / 2f
                || mousePos.y < rectCheckHoverOut.position.y - rectCheckHoverOut.sizeDelta.y * UICanvasGetScaleFactor.scaleFactor / 2f
                || mousePos.y > rectCheckHoverOut.position.y + rectCheckHoverOut.sizeDelta.y * UICanvasGetScaleFactor.scaleFactor / 2f)
            {
                actionHoverOut?.Invoke();
                flagCheckHoverOut = false;
            }
        }

        [Serializable]
        public class ButtonHoverEvent : UnityEvent {}
    }
}