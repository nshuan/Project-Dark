using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Common.NavButton
{
    public class UIButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
    {
        public int Index { get; set; }
        public UIButtonState State { get; protected set; }
        public Func<UIButton, UIButtonState, bool> FuncUpdateNav { get; set; }

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
            FuncUpdateNav(this, UIButtonState.Selected);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            FuncUpdateNav(this, UIButtonState.Hover);
        }
    }

    public enum UIButtonState
    {
        None,
        Hover,
        Selected
    }
}