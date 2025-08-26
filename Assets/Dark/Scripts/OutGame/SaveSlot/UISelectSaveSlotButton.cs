using Dark.Scripts.OutGame.Common.NavButton;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.OutGame.SaveSlot
{
    public class UISelectSaveSlotButton : UIButton
    {
        [SerializeField] private GameObject uiHover;
        [SerializeField] private GameObject uiSelected;
        
        public override void UpdateUI(UIButtonState state)
        {
            switch (state)
            {
                case UIButtonState.None:
                    uiHover.SetActive(false);
                    uiSelected.SetActive(false);
                    break;
                case UIButtonState.Hover:
                    uiHover.SetActive(true);
                    uiSelected.SetActive(true);
                    break;
                case UIButtonState.Selected:
                    uiHover.SetActive(false);
                    uiSelected.SetActive(true);
                    break;
            }
        }

        [Space] [Header("Config")] 
        public int slotIndex;

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            SaveSlotManager.Instance.SelectSlot(slotIndex);
        }
    }
}