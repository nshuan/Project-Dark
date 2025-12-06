using Dark.Scripts.Common;
using Dark.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.OutGame.Home
{
    public class UIWishlistButton : UIHomeButton
    {
        public override bool BlockSelect => true;

        private void OpenSteamWishlist()
        {
            Application.OpenURL(GameConst.SteamWishlistURL);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            this.DelayCall(UIConst.HomeBtnDelayOnClick, OpenSteamWishlist);
            base.OnPointerClick(eventData);
        }
    }
}