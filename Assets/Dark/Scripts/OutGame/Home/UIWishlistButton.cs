using Dark.Scripts.Common;
using Dark.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.OutGame.Home
{
    public class UIWishlistButton : MonoBehaviour, IPointerClickHandler
    {
        private void OpenSteamWishlist()
        {
            Application.OpenURL(GameConst.SteamWishlistURL);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OpenSteamWishlist();
        }
    }
}