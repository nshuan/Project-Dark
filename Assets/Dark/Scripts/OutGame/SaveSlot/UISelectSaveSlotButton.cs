using System;
using Dark.Scripts.OutGame.Common.NavButton;
using Dark.Scripts.SceneNavigation;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.SaveSlot
{
    public class UISelectSaveSlotButton : UIButton
    {
        [SerializeField] private Image uiFrame;
        [SerializeField] private Image uiBlockClearSave;
        
        [Space] [Header("Config")]
        public int slotIndex;
        [SerializeField] private RectTransform resizeTarget;
        [SerializeField] private Image imgLight;
        [SerializeField] private Vector2 sizeUnselected = new Vector2(1f, 1f);
        [SerializeField] private Vector2 sizeSelected;
        [SerializeField] private float frameAlphaUnselected = 0.9f;
        [SerializeField] private float frameAlphaSelected = 1f;
        [SerializeField] private float hoverAnimDuration = 0.25f;

        private bool isHovering = false;

        private void OnEnable()
        {
            isHovering = false;
            uiFrame.SetAlpha(frameAlphaUnselected);
            resizeTarget.sizeDelta = sizeUnselected;
            imgLight.SetAlpha(0f);
            uiBlockClearSave.SetAlpha(1f);
        }

        public override void UpdateUI(UIButtonState state)
        {
            switch (state)
            {
                case UIButtonState.None:
                    uiFrame.SetAlpha(frameAlphaUnselected);
                    if (isHovering) DoHoverOut(hoverAnimDuration);
                    isHovering = false;
                    break;
                case UIButtonState.Hover:
                    uiFrame.SetAlpha(frameAlphaSelected);
                    if (!isHovering) DoHoverIn(hoverAnimDuration);
                    isHovering = true;
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

        private Tween DoHoverIn(float duration)
        {
            DOTween.Kill(this);
            var seq = DOTween.Sequence(this);

            seq.Append(resizeTarget.DOSizeDelta(sizeSelected, duration).SetEase(Ease.OutBack))
                .Join(imgLight.DOFade(1f, duration))
                .Join(uiBlockClearSave.DOFade(0f, duration));
            
            return seq;
        }
        
        private Tween DoHoverOut(float duration)
        {
            DOTween.Kill(this);
            var seq = DOTween.Sequence(this);

            seq.Append(resizeTarget.DOSizeDelta(sizeUnselected, duration).SetEase(Ease.InBack))
                .Join(imgLight.DOFade(0f, duration))
                .Join(uiBlockClearSave.DOFade(1f, duration));
            
            return seq;
        }
    }
}