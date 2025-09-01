using System;
using UnityEngine;

namespace Dark.Scripts.OutGame.Common.NavButton
{
    public class UINavButton : MonoBehaviour
    {
        [SerializeField] private UIButton[] buttons;
        [SerializeField] private int startIndex = -1;

        private UIButton currentSelectButton;
        
        private void Awake()
        {
            for (var i = 0; i < buttons.Length; i++)
            {
                buttons[i].Index = i;
                buttons[i].UpdateState(UIButtonState.None);
                buttons[i].FuncUpdateNav = OnButtonAction;
            }

            if (startIndex >= 0 && startIndex < buttons.Length)
            {
                currentSelectButton = buttons[startIndex];
                OnButtonAction(currentSelectButton, UIButtonState.Selected);
            }
        }

        private bool OnButtonAction(UIButton target, UIButtonState state)
        {
            switch (state)
            {
                case UIButtonState.None:
                    if (target.State != UIButtonState.Selected)
                        target.UpdateState(state);
                    break;
                case UIButtonState.Hover:
                    foreach (var button in buttons)
                    {
                        if (button.State == UIButtonState.Selected) continue;
                        button.UpdateState(UIButtonState.None);
                    }
                    if (target.State != UIButtonState.Selected)
                        target.UpdateState(state);
                    break;
                case UIButtonState.Selected:
                    currentSelectButton = target;
                    currentSelectButton.UpdateState(state);
                    
                    foreach (var button in buttons)
                    {
                        if (button.Index != target.Index)
                            button.UpdateState(UIButtonState.None);
                    }
                    break;
            }
            
            return true;
        }
    }
}