using Dark.Scripts.Common;
using Dark.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.OutGame.Home
{
    public class UIWishlistButton : UIHomeButton
    {
        [Space]
        public string steamWishlistURL = "https://store.steampowered.com/app/3913310/Ash_Warden/";

        public override bool BlockSelect => true;

        private void OpenSteamWishlist()
        {
            Application.OpenURL(steamWishlistURL);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            this.DelayCall(UIConst.HomeBtnDelayOnClick, OpenSteamWishlist);
            base.OnPointerClick(eventData);
        }
    }
}