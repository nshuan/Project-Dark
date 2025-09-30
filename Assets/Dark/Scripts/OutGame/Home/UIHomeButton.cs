using System;
using Coffee.UIExtensions;
using Dark.Scripts.Common;
using Dark.Scripts.OutGame.Common.NavButton;
using Dark.Scripts.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Home
{
    public class UIHomeButton : UIButton
    {
        [SerializeField] private Image hoverPointer;
        [SerializeField] private UIParticle vfxHover;
        [SerializeField] private UIParticle vfxSelect;

        private void OnEnable()
        {
            hoverPointer.gameObject.SetActive(false);
        }

        public override void UpdateUI(UIButtonState state)
        {
            switch (state)
            {
                case UIButtonState.None:
                    DoHidePointer();
                    DoHideLight();
                    break;
                case UIButtonState.Hover:
                    DoShowPointer();
                    DoShowLight();
                    break;
                case UIButtonState.Selected:
                    DoHidePointer();
                    DoShowLight();
                    break;
            }
        }

        private Tween DoShowLight()
        {
            DOTween.Kill(vfxHover);
            return DOTween.Sequence(vfxHover)
                .AppendCallback(() =>
                {
                    vfxHover.gameObject.SetActive(true);
                    vfxHover.Play();
                });
        }

        private Tween DoHideLight()
        {
            DOTween.Kill(vfxHover);
            return DOTween.Sequence(vfxHover)
                .AppendCallback(() =>
                {
                    vfxHover.gameObject.SetActive(false);
                });
        }

        private Tween DoShowPointer()
        {
            DOTween.Kill(hoverPointer);
            return DOTween.Sequence(hoverPointer)
                .AppendCallback(() =>
                {
                    hoverPointer.SetAlpha(0f);
                    hoverPointer.transform.localScale = 0.3f * Vector3.one;
                    hoverPointer.gameObject.SetActive(true);
                })
                .Append(hoverPointer.DOFade(1f, 0.2f))
                .Join(hoverPointer.transform.DOScale(1f, 0.2f));
        }

        private Tween DoHidePointer()
        {
            DOTween.Kill(hoverPointer);
            return DOTween.Sequence(hoverPointer)
                .Append(hoverPointer.DOFade(0f, 0.2f))
                .Join(hoverPointer.transform.DOScale(0f, 0.2f))
                .AppendCallback(() =>
                {
                    hoverPointer.gameObject.SetActive(false);
                });
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            PlaySelectVfx();
            base.OnPointerClick(eventData);
        }

        protected void PlaySelectVfx()
        {
            DOTween.Kill(vfxSelect);
            DOTween.Sequence(vfxSelect)
                .AppendCallback(() =>
                {
                    vfxSelect.gameObject.SetActive(true);
                    vfxSelect.Play();
                })
                .AppendInterval(1f)
                .AppendCallback(() =>
                {
                    vfxSelect.gameObject.SetActive(false);
                }).Play();
        }
    }
}