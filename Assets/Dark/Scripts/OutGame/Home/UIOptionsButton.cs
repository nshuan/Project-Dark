using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.OutGame.Home
{
    public class UIOptionsButton : UIHomeButton
    {
        [SerializeField] private GameObject panelOptions;

        public override void OnPointerClick(PointerEventData eventData)
        {
            panelOptions.SetActive(true);
            base.OnPointerClick(eventData);
        }
    }
}