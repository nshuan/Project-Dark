using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.Common
{
    public class UIButtonAnimated : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [SerializeField] protected float hoverScale = 1.1f;
        [SerializeField] protected float pressScale = 1f;
        [SerializeField] protected float duration = 0.2f;
        [SerializeField] private Transform target;

        private void Awake()
        {
            if (!target) target = transform;
        }

        private void OnDisable()
        {
            DOTween.Kill(this);
            target.localScale = Vector3.one;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            DOTween.Kill(this);
            DOTween.Sequence(this).SetUpdate(true)
                .Append(target.DOScale(hoverScale, duration)).Play();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            DOTween.Kill(this);
            DOTween.Sequence(this).SetUpdate(true)
                .Append(target.DOScale(1f, duration)).Play();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (coroutinePointerDown != null) StopCoroutine(coroutinePointerDown);
            coroutinePointerDown = StartCoroutine(IEPointerDown());
        }

        private Coroutine coroutinePointerDown;
        private IEnumerator IEPointerDown()
        {
            yield return new WaitForSecondsRealtime(0.2f);
            
            DOTween.Kill(this);
            DOTween.Sequence(this).SetUpdate(true)
                .Append(target.DOScale(pressScale, duration).SetEase(Ease.OutQuad)).Play();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            DOTween.Kill(this);
            DOTween.Sequence(this).SetUpdate(true)
                .Append(target.DOScale(1f, duration)).Play();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (coroutinePointerDown != null) StopCoroutine(coroutinePointerDown);
            
            DOTween.Kill(this);
            DOTween.Sequence(this).SetUpdate(true)
                .Append(target.DOPunchScale(new Vector3(pressScale - hoverScale, pressScale - hoverScale, pressScale - hoverScale), duration)).Play();
        }
    }
}