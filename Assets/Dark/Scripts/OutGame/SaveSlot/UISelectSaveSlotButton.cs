using System;
using Dark.Scripts.OutGame.Common.NavButton;
using Dark.Scripts.SceneNavigation;
using DG.Tweening;
using TMPro;
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
        
        [Header("General")]
        [SerializeField] private RectTransform resizeTarget;
        [SerializeField] private Vector2 sizeUnselected = new Vector2(1f, 1f);
        [SerializeField] private Vector2 sizeSelected;
        [SerializeField] private float frameAlphaUnselected = 0.9f;
        [SerializeField] private float frameAlphaSelected = 1f;
        [SerializeField] private float hoverAnimDuration = 0.25f;
        
        [Header("Hover light")]
        [SerializeField] private Image imgLight;
        
        [Header("Group Icon")]
        [SerializeField] private CanvasGroup cvgIconUnselected;
        [SerializeField] private CanvasGroup cvgIconSelected;
        [SerializeField] private Vector2 sizeIconSelected;
        [SerializeField] private Vector2 sizeIconUnselected = new Vector2(1f, 1f);
        
        [Header("Text color")]
        [SerializeField] private Graphic[] texts;
        [SerializeField] private Color colorTextUnselected;
        [SerializeField] private Color colorTextSelected;

        private bool isHovering = false;
        private RectTransform rectIconUnselected;
        private RectTransform rectIconSelected;

        private void OnEnable()
        {
            isHovering = false;
            uiFrame.SetAlpha(frameAlphaUnselected);
            resizeTarget.sizeDelta = sizeUnselected;
            imgLight.SetAlpha(0f);
            cvgIconUnselected.alpha = 1f;
            cvgIconSelected.alpha = 0f;
            uiBlockClearSave.SetAlpha(1f);
            rectIconUnselected = cvgIconUnselected.GetComponent<RectTransform>();
            rectIconSelected = cvgIconSelected.GetComponent<RectTransform>();
            rectIconUnselected.sizeDelta = sizeIconUnselected;
            rectIconSelected.sizeDelta = sizeIconUnselected;
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

            seq.Append(resizeTarget.DOSizeDelta(sizeSelected, duration))
                .Join(imgLight.DOFade(1f, duration))
                .Join(rectIconSelected.DOSizeDelta(sizeIconSelected, duration))
                .Join(rectIconUnselected.DOSizeDelta(sizeIconSelected, duration))
                .Join(cvgIconSelected.DOFade(1f, duration))
                .Join(cvgIconUnselected.DOFade(0f, duration))
                .Join(uiBlockClearSave.DOFade(0f, duration));

            foreach (var txt in texts)
            {
                seq.Join(txt.DOColor(colorTextSelected, duration));
            }
            
            return seq;
        }
        
        private Tween DoHoverOut(float duration)
        {
            DOTween.Kill(this);
            var seq = DOTween.Sequence(this);

            seq.Append(resizeTarget.DOSizeDelta(sizeUnselected, duration))
                .Join(imgLight.DOFade(0f, duration))
                .Join(rectIconSelected.DOSizeDelta(sizeIconUnselected, duration))
                .Join(rectIconUnselected.DOSizeDelta(sizeIconUnselected, duration))
                .Join(cvgIconSelected.DOFade(0f, duration))
                .Join(cvgIconUnselected.DOFade(1f, duration))
                .Join(uiBlockClearSave.DOFade(1f, duration));
            
            foreach (var txt in texts)
            {
                seq.Join(txt.DOColor(colorTextUnselected, duration));
            }
            
            return seq;
        }

        // private RectTransform rectThis;
        // private Canvas canvasThis;
        // public override void OnPointerExit(PointerEventData eventData)
        // {
        //     rectThis ??= GetComponent<RectTransform>();
        //     canvasThis ??= GetComponentInParent<Canvas>();
        //     
        //     if (rectThis.PointerStillOverMe(canvasThis)) return;
        //     
        //     base.OnPointerExit(eventData);
        // }
    }
}