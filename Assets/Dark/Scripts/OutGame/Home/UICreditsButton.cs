using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.OutGame.Home
{
    public class UICreditsButton : UIHomeButton
    {
        [SerializeField] private GameObject panelCredits;

        public override void OnPointerClick(PointerEventData eventData)
        {
            panelCredits.SetActive(true);
            base.OnPointerClick(eventData);
        }
    }
}