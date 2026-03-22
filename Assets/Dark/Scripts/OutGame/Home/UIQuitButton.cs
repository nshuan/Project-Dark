using Dark.Scripts.Common;
using Dark.Scripts.Common.UIWarning;
using Dark.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.OutGame.Home
{
    public class UIQuitButton : UIHomeButton
    {
        [SerializeField] private UIPopupWarning popupWarning;
        
        public override void OnPointerClick(PointerEventData eventData)
        {
            popupWarning.LocalizedSetup(
                "key_are_you_sure",
                "",
                Application.Quit);
            popupWarning.DoOpenFadeIn(UIConst.HomeBtnDelayOnClick);

            base.OnPointerClick(eventData);
        }
    }
}