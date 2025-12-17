using System;
using DG.Tweening;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeNodeHoverAnimation : MonoBehaviour
    {
        [SerializeField] private RectTransform rectVertical;
        [SerializeField] private RectTransform rectHorizontal;

        private Vector2 rectVerticalOriginalSize;
        private Vector2 rectHorizontalOriginalSize;
        private Vector2 punchAmount = new Vector2(0f, 5f);

        private void Awake()
        {
            rectVerticalOriginalSize = rectVertical.sizeDelta;
            rectHorizontalOriginalSize = rectHorizontal.sizeDelta;
        }

        private void OnEnable()
        {
            DOTween.Kill(this);
            
            rectVertical.sizeDelta = rectVerticalOriginalSize;
            rectHorizontal.sizeDelta = rectHorizontalOriginalSize;

            var seq = DOTween.Sequence(this);
            seq.Append(rectVertical.DOSizeDelta(punchAmount, 0.2f).SetEase(Ease.OutSine).SetRelative())
                .Join(rectHorizontal.DOSizeDelta(punchAmount, 0.2f).SetEase(Ease.OutSine).SetRelative())
                .Append(rectVertical.DOSizeDelta(rectVerticalOriginalSize, 0.2f).SetEase(Ease.InSine))
                .Join(rectHorizontal.DOSizeDelta(rectHorizontalOriginalSize, 0.2f).SetEase(Ease.InSine));
            seq.AppendInterval(0.8f);
            seq.SetLoops(-1, LoopType.Restart);
        }

        private void OnDisable()
        {
            DOTween.Kill(this);
        }
    }
}