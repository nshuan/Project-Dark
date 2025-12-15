using Dark.Scripts.ForDemo;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.OutGame.Home
{
    public class UIWishlistButton : MonoBehaviour, IPointerClickHandler
    {
        private void OpenSteamWishlist()
        {
            Application.OpenURL(DemoConfig.SteamWishlistURL);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OpenSteamWishlist();
        }
    }
}