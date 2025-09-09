using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.Common
{
    public class UIButtonAnimated : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
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
    }
}