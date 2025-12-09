using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.Common
{
    public class UIButtonZoomingAnimated : UIButtonAnimated
    {
        [SerializeField] private float zoomingInScale = 1.05f;
        [SerializeField] private float zoomingOutScale = 1f;
        
        private void OnEnable()
        {
            DoZooming().SetTarget(this);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            DOTween.Kill(this);
            DOTween.Sequence(this)
                .Append(transform.DOScale(1f, duration))
                .AppendInterval(1f)
                .Append(DoZooming());
        }

        private Tween DoZooming()
        {
            return DOTween.Sequence()
                .Append(transform.DOScale(zoomingInScale, duration).SetEase(Ease.OutQuad))
                .Append(transform.DOScale(zoomingOutScale, duration).SetEase(Ease.OutQuad))
                    .SetLoops(10, LoopType.Yoyo);
        }
    }
}