using System;
using Dark.Scripts.OutGame.Common.NavButton;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Home
{
    public class UIHomeButton : UIButton
    {
        [SerializeField] private Image hoverPointer;
        [SerializeField] private Image selectLight;

        private void OnEnable()
        {
            hoverPointer.gameObject.SetActive(false);
            selectLight.gameObject.SetActive(false);
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
            DOTween.Kill(selectLight);
            return DOTween.Sequence(selectLight)
                .AppendCallback(() =>
                {
                    selectLight.SetAlpha(0f);
                    selectLight.gameObject.SetActive(true);
                })
                .Append(selectLight.DOFade(0.5f, 0.2f));
        }

        private Tween DoHideLight()
        {
            DOTween.Kill(selectLight);
            return DOTween.Sequence(selectLight)
                .Append(selectLight.DOFade(0f, 0.2f))
                .AppendCallback(() =>
                {
                    selectLight.gameObject.SetActive(false);
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
    }
}