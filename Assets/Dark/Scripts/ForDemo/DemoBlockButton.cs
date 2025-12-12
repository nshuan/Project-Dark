using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dark.Scripts.ForDemo
{
    public class DemoBlockButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] protected UIDemoPopupBlock popupWishlist; // Child 0 -> block raycast; child 1 -> popup
        
        [SerializeField] protected CanvasGroup buttonVisual;
        [SerializeField] protected GameObject hiddenButton;
        [SerializeField] protected bool hideOnExit = false;
        
        private Canvas parentCanvas;
        protected UIDemoPopupBlock cachePopupWishlist;

        private void Awake()
        {
            if (!DemoConfig.IsDemo) return;
            
            parentCanvas = GetComponentInParent<Canvas>();
        }

        private void Start()
        {
            if (!DemoConfig.IsDemo) return;
            
            if (!ShouldShowButton())
            {
                hiddenButton?.SetActive(true);
                gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            if (!DemoConfig.IsDemo) return;
            
            buttonVisual.alpha = 0f;
            hiddenButton?.SetActive(true);
        }

        protected virtual bool ShouldShowButton()
        {
            return true;
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (!DemoConfig.IsDemo) return;

            parentCanvas ??= GetComponentInParent<Canvas>();
            cachePopupWishlist ??= Instantiate(popupWishlist, parentCanvas.transform);
            cachePopupWishlist.transform.SetAsLastSibling();
            cachePopupWishlist.gameObject.SetActive(true);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!DemoConfig.IsDemo) return;
            
            DOTween.Kill(this);
            buttonVisual.alpha = 1f;
            hiddenButton?.SetActive(false);
            buttonVisual.transform.localRotation = Quaternion.Euler(0f, 0f, 20f);
            buttonVisual.transform.DOLocalRotate(Vector3.zero, 0.2f).SetEase(Ease.OutBack).SetTarget(this);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (!DemoConfig.IsDemo) return;
            
            if (hideOnExit)
            {
                DOTween.Kill(this);
                hiddenButton?.SetActive(true);
                buttonVisual.DOFade(0f, 0.2f).SetEase(Ease.InQuad).SetTarget(this);
            }
        }
    }
}