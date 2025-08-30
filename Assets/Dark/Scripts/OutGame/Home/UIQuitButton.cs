using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.OutGame.Home
{
    public class UIQuitButton : UIHomeButton
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            Application.Quit();
        }
    }
}