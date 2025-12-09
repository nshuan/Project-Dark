using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.ForDemo
{
    public class UIDemoPopupBlock : MonoBehaviour
    {
        [SerializeField] private RectTransform uiPopup;
        [SerializeField] private CanvasGroup groupPopup;

        private void OnEnable()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            var rectPopup = transform.GetComponent<RectTransform>();
            rectPopup.offsetMin = Vector2.zero;
            rectPopup.offsetMax = Vector2.zero;
            rectPopup.transform.localScale = Vector3.one;
            transform.gameObject.SetActive(true);
            // btnClosePopup = cachePopupWishlist.GetComponent<Button>();
            DebugUtility.LogWarning("Clicked button wishlist");

            groupPopup.alpha = 0f;
            uiPopup.localPosition = Vector3.zero - new Vector3(0f, 100f, 0f);
            // btnClosePopup.onClick.RemoveAllListeners();
            DOTween.Kill(this);
            var seq = DOTween.Sequence(this)
                .Append(groupPopup.DOFade(1f, 0.2f).SetEase(Ease.OutQuad))
                .Join(uiPopup.DOLocalMoveY(0f, 0.2f).SetEase(Ease.OutQuad));
        }
    }
}