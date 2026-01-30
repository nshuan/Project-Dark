using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Dark.Scripts.Common
{
    public class UIInteractiveHoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private RectTransform rectCheckHoverOut;
        [SerializeField] public ButtonHoverEvent actionHoverIn;
        [SerializeField] public ButtonHoverEvent actionHoverOut;
        [SerializeField] public ButtonHoverEvent actionOnClick;
 
        private bool flagCheckHoverOut = false;
        public bool clickable = true;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (flagCheckHoverOut) return;
            actionHoverIn?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            flagCheckHoverOut = true;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (clickable)
                actionOnClick?.Invoke();
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