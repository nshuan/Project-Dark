using System;
using Coffee.UIExtensions;
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
        [SerializeField] private Vector3 scaleUnselected = new Vector3(1f, 1f, 1f);
        [SerializeField] private Vector3 scaleSelected;
        [SerializeField] private float frameAlphaUnselected = 0.9f;
        [SerializeField] private float frameAlphaSelected = 1f;
        [SerializeField] private float hoverAnimDuration = 0.25f;
        
        [Header("Hover light")]
        [SerializeField] private CanvasGroup imgLight;
        [SerializeField] private UIParticle _fxHover;
        [SerializeField] private UIParticle _fxBurst;
        
        [Header("Group Icon")]
        [SerializeField] private CanvasGroup cvgIconUnselected;
        [SerializeField] private CanvasGroup cvgIconSelected;
        
        [Header("Text color")]
        [SerializeField] private Graphic[] texts;
        [SerializeField] private Color colorTextUnselected;
        [SerializeField] private Color colorTextSelected;

        private bool isHovering = false;

        private void OnEnable()
        {
            isHovering = false;
            uiFrame.SetAlpha(frameAlphaUnselected);
            resizeTarget.localScale = scaleUnselected;
            imgLight.alpha = 0f;
            cvgIconUnselected.alpha = 1f;
            cvgIconSelected.alpha = 0f;
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
                    if (!isHovering)
                    {
                        DoHoverIn(hoverAnimDuration);
                        _fxHover.Play();
                        _fxBurst.Play();
                    }
                    isHovering = true;
                    break;
                case UIButtonState.Selected:
                    uiFrame.SetAlpha(frameAlphaSelected);
                    resizeTarget.localScale = scaleSelected;
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
            
            seq.Append(resizeTarget.DOScale(scaleSelected, duration))
                .Join(imgLight.DOFade(1f, duration))
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
            
            seq.Append(resizeTarget.DOScale(scaleUnselected, duration))
                .Join(imgLight.DOFade(0f, duration))
                .Join(cvgIconSelected.DOFade(0f, duration))
                .Join(cvgIconUnselected.DOFade(1f, duration))
                .Join(uiBlockClearSave.DOFade(1f, duration));
            
            foreach (var txt in texts)
            {
                seq.Join(txt.DOColor(colorTextUnselected, duration));
            }
            
            return seq;
        }

        private RectTransform rectThis;
        private Canvas canvasThis;
        public override void OnPointerExit(PointerEventData eventData)
        {
            rectThis ??= GetComponent<RectTransform>();
            canvasThis ??= GetComponentInParent<Canvas>();
            
            if (rectThis.PointerStillOverMe(canvasThis)) return;
            
            base.OnPointerExit(eventData);
        }
    }
}