using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Common.NavButton
{
    public class UIButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public int Index { get; set; }
        public UIButtonState State { get; protected set; }
        public virtual bool BlockSelect => false;
        public Func<UIButton, UIButtonState, bool> FuncUpdateNav { get; set; }

        public bool interactable = true;
        
        public void UpdateState(UIButtonState state)
        {
            State = state;
            UpdateUI(state);
        }
        
        public virtual void UpdateUI(UIButtonState state)
        {
            
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (BlockSelect) return;
            FuncUpdateNav(this, UIButtonState.Selected);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            FuncUpdateNav(this, UIButtonState.Hover);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            FuncUpdateNav(this, UIButtonState.None);
        }
    }

    public enum UIButtonState
    {
        None,
        Hover,
        Selected
    }
}