using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeNodeHoverField : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private GameObject imgIconHovering;
        
        public Action onHover;
        public Action onHoverExit;
        public Action onPointerClick;
        
        public RectTransform nodeRepresentableRect;
        public bool interactable;
        public bool canShowIconHovering;
        
        private void Awake()
        {
            nodeRepresentableRect ??= GetComponent<RectTransform>();
            canShowIconHovering = imgIconHovering != null;
            if (canShowIconHovering) imgIconHovering.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (canShowIconHovering) imgIconHovering.SetActive(true);
            onHover?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (canShowIconHovering) imgIconHovering.SetActive(false);
            onHoverExit?.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!interactable) return;
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (canShowIconHovering)
            {
                DOTween.Kill(imgIconHovering);
                imgIconHovering.transform.localScale = Vector3.one;
                imgIconHovering.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.3f).SetTarget(imgIconHovering);
            }
            onPointerClick?.Invoke();
        }
    }
}