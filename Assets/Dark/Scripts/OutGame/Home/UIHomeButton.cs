using Dark.Scripts.OutGame.Common.NavButton;
using UnityEngine;

namespace Dark.Scripts.OutGame.Home
{
    public class UIHomeButton : UIButton
    {
        [SerializeField] private GameObject hoverPointer;
        [SerializeField] private GameObject selectLight;
        
        public override void UpdateUI(UIButtonState state)
        {
            switch (state)
            {
                case UIButtonState.None:
                    hoverPointer.SetActive(false);
                    selectLight.SetActive(false);
                    break;
                case UIButtonState.Hover:
                    hoverPointer.SetActive(true);
                    selectLight.SetActive(true);
                    break;
                case UIButtonState.Selected:
                    hoverPointer.SetActive(false);
                    selectLight.SetActive(true);
                    break;
            }
        }
    }
}