using Dark.Scripts.OutGame.Common.NavButton;
using Dark.Scripts.SceneNavigation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.SaveSlot
{
    public class UISelectSaveSlotButton : UIButton
    {
        [SerializeField] private Image uiFrame;
        
        [Space] [Header("Config")]
        public int slotIndex;
        [SerializeField] private RectTransform resizeTarget;
        [SerializeField] private Vector2 sizeUnselected = new Vector2(1f, 1f);
        [SerializeField] private Vector2 sizeSelected;
        [SerializeField] private float frameAlphaUnselected = 0.9f;
        [SerializeField] private float frameAlphaSelected = 1f;
        
        public override void UpdateUI(UIButtonState state)
        {
            switch (state)
            {
                case UIButtonState.None:
                    uiFrame.SetAlpha(frameAlphaUnselected);
                    resizeTarget.sizeDelta = sizeUnselected;
                    break;
                case UIButtonState.Hover:
                    uiFrame.SetAlpha(frameAlphaSelected);
                    resizeTarget.sizeDelta = sizeUnselected;
                    break;
                case UIButtonState.Selected:
                    uiFrame.SetAlpha(frameAlphaSelected);
                    resizeTarget.sizeDelta = sizeSelected;
                    break;
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            SaveSlotManager.Instance.SelectSlot(slotIndex);
            Loading.Instance.LoadScene(SceneConstants.SceneUpgrade);
        }
    }
}