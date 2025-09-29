using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.Common
{
    public class UIButtonAnimated : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [SerializeField] private float hoverScale = 1.1f;
        [SerializeField] private float pressScale = 1f;
        [SerializeField] private float duration = 0.2f;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            DOTween.Kill(this);
            DOTween.Sequence(this)
                .Append(transform.DOScale(hoverScale, duration)).Play();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            DOTween.Kill(this);
            DOTween.Sequence(this)
                .Append(transform.DOScale(1f, duration)).Play();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (coroutinePointerDown != null) StopCoroutine(coroutinePointerDown);
            coroutinePointerDown = StartCoroutine(IEPointerDown());
        }

        private Coroutine coroutinePointerDown;
        private IEnumerator IEPointerDown()
        {
            yield return new WaitForSeconds(0.2f);
            
            DOTween.Kill(this);
            DOTween.Sequence(this)
                .Append(transform.DOScale(pressScale, duration)).Play();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            DOTween.Kill(this);
            DOTween.Sequence(this)
                .Append(transform.DOScale(1f, duration)).Play();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (coroutinePointerDown != null) StopCoroutine(coroutinePointerDown);
            
            DOTween.Kill(this);
            DOTween.Sequence(this)
                .Append(transform.DOPunchScale(new Vector3(pressScale - hoverScale, pressScale - hoverScale, pressScale - hoverScale), duration)).Play();
        }
    }
}