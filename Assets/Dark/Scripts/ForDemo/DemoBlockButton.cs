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
        [SerializeField] protected CanvasGroup popupWishlist; // Child 0 -> block raycast; child 1 -> popup
        
        [SerializeField] protected CanvasGroup buttonVisual;
        [SerializeField] protected GameObject hiddenButton;
        [SerializeField] protected bool hideOnExit = false;

        // private Button btnClosePopup;
        private Canvas parentCanvas;
        private CanvasGroup cachePopupWishlist;
        private Transform cacheUIPopupWishlist;

        private void Awake()
        {
            parentCanvas = GetComponentInParent<Canvas>();
        }

        private void Start()
        {
            if (!ShouldShowButton())
            {
                hiddenButton?.SetActive(true);
                gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            buttonVisual.alpha = 0f;
            hiddenButton?.SetActive(true);
        }

        protected virtual bool ShouldShowButton()
        {
            return true;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            cachePopupWishlist ??= Instantiate(popupWishlist, parentCanvas.transform);
            cachePopupWishlist.transform.SetAsLastSibling();
            var rectPopup = cachePopupWishlist.GetComponent<RectTransform>();
            rectPopup.offsetMin = Vector2.zero;
            rectPopup.offsetMax = Vector2.zero;
            rectPopup.transform.localScale = Vector3.one;
            cacheUIPopupWishlist ??= rectPopup.GetChild(1);
            cachePopupWishlist.gameObject.SetActive(true);
            // btnClosePopup = cachePopupWishlist.GetComponent<Button>();
            DebugUtility.LogWarning("Clicked button wishlist");

            cachePopupWishlist.alpha = 0f;
            cacheUIPopupWishlist.localPosition = Vector3.zero - new Vector3(0f, 100f, 0f);
            // btnClosePopup.onClick.RemoveAllListeners();
            DOTween.Kill(cachePopupWishlist);
            var seq = DOTween.Sequence(cachePopupWishlist)
                .Append(cachePopupWishlist.DOFade(1f, 0.2f).SetEase(Ease.OutQuad))
                .Join(cacheUIPopupWishlist.DOLocalMoveY(0f, 0.2f).SetEase(Ease.OutQuad));
            // .AppendCallback(() =>
            // {
            //     btnClosePopup.onClick.RemoveAllListeners();
            //     btnClosePopup.onClick.AddListener(() =>
            //     {
            //         cachePopupWishlist.gameObject.SetActive(false);
            //     });
            // });
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            DOTween.Kill(this);
            buttonVisual.alpha = 1f;
            hiddenButton?.SetActive(false);
            buttonVisual.transform.localRotation = Quaternion.Euler(0f, 0f, 20f);
            buttonVisual.transform.DOLocalRotate(Vector3.zero, 0.2f).SetEase(Ease.OutBack).SetTarget(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (hideOnExit)
            {
                DOTween.Kill(this);
                hiddenButton?.SetActive(true);
                buttonVisual.DOFade(0f, 0.2f).SetEase(Ease.InQuad).SetTarget(this);
            }
        }
    }
}