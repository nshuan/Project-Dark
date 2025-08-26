using System;
using UnityEngine;

namespace Dark.Scripts.OutGame.Common.NavButton
{
    public class UINavButton : MonoBehaviour
    {
        [SerializeField] private UIButton[] buttons;
        [SerializeField] private int startIndex = -1;

        private UIButton currentButton;
        
        private void Awake()
        {
            for (var i = 0; i < buttons.Length; i++)
            {
                buttons[i].Index = i;
                buttons[i].FuncUpdateNav = OnButtonAction;
            }

            if (startIndex >= 0 && startIndex < buttons.Length)
            {
                currentButton = buttons[startIndex];
                OnButtonAction(currentButton, UIButtonState.Selected);
            }
        }

        private bool OnButtonAction(UIButton target, UIButtonState state)
        {
            foreach (var button in buttons)
            {
                button.UpdateUI(UIButtonState.None);
            }

            currentButton = target;
            currentButton.UpdateUI(state);
            return true;
        }
    }
}