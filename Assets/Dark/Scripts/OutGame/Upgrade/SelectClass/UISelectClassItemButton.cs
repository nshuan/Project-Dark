using System;
using Dark.Scripts.OutGame.Common.NavButton;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UISelectClassItemButton : UIButton
    {
        [SerializeField] private RectTransform rectFrame;
        
        [Space] [Header("Config")]
        [SerializeField] private float widthCollapse;
        [SerializeField] private float widthExpand;
        
        public override void UpdateUI(UIButtonState state)
        {
            switch (state)
            {
                case UIButtonState.None:
                    rectFrame.sizeDelta = new Vector2(widthCollapse, rectFrame.sizeDelta.y);
                    break;
                case UIButtonState.Hover:
                    break;
                case UIButtonState.Selected:
                    rectFrame.sizeDelta = new Vector2(widthExpand, rectFrame.sizeDelta.y);
                    break;
            }    
        }
    }
}