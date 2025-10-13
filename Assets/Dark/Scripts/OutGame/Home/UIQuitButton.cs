using Dark.Scripts.Common;
using Dark.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.OutGame.Home
{
    public class UIQuitButton : UIHomeButton
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            this.DelayCall(UIConst.HomeBtnDelayOnClick, Application.Quit);
            base.OnPointerClick(eventData);
        }
    }
}