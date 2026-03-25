using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Home
{
    public class UIHomeFullscreenStartButton : MonoBehaviour, IPointerClickHandler
    {
        public static bool EnableFullscreenInput { get; set; } = true;
        
        [SerializeField] private Image imgBlockRaycast;
        [SerializeField] private Transform rectTitle;
        [SerializeField] private Transform rectBackground;
        [SerializeField] private Transform[] listButtonInAppearOrder;
        [SerializeField] private TextMeshProUGUI txtInstruction;
        [SerializeField] private CanvasGroup groupStudioLogos;
        [SerializeField] private CanvasGroup groupLeaderboardPreview;

        [Space] [Header("Config")] 
        [SerializeField] private Vector2 bgHidePosition;
        [SerializeField] private Vector2 bgAppearPosition;
        [SerializeField] private Vector2 titleHidePosition;
        [SerializeField] private Vector2 titleAppearPosition;
        [SerializeField] private float titleHideScale = 1f;
        [SerializeField] private float titleAppearScale = 1f;
        [SerializeField] private float durationTitleAndBackground = 0.5f;
        [SerializeField] private float durationEachButton = 0.2f;
        [SerializeField] private float delayEachButton = 0.1f;
        [SerializeField] private Vector2 buttonOffsetOnHide;

        private List<CanvasGroup> listButtonCanvasGroup;
        private List<Vector2> listButtonShowPosition;
        private bool isShowHome = false;
        private bool isAnimating = false;
        
        private void Awake()
        {
            if (listButtonInAppearOrder != null)
            {
                listButtonCanvasGroup = new List<CanvasGroup>();
                listButtonShowPosition = new List<Vector2>();
                foreach (var button in listButtonInAppearOrder)
                {
                    var canvasGroup = button.AddComponent<CanvasGroup>();
                    listButtonCanvasGroup.Add(canvasGroup);
                    listButtonShowPosition.Add(button.localPosition);    
                }
            }

            imgBlockRaycast.enabled = true;
            
            Init();
        }

        private void Update()
        {
            if (!isShowHome) return;
            if (isAnimating) return;
            if (!EnableFullscreenInput) return;
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                imgBlockRaycast.enabled = true;
                isAnimating = true;
                DoHide().OnComplete(() =>
                {
                    isShowHome = false;
                    isAnimating = false;
                    txtInstruction.DOFade(1f, 3f).SetTarget(txtInstruction).SetDelay(2f).SetLoops(-1, LoopType.Yoyo);
                });
            }
        }

        private void Init()
        {
            rectBackground.localPosition = bgHidePosition;
            rectTitle.localPosition = titleHidePosition;
            rectTitle.localScale = titleHideScale * Vector3.one;
            foreach (var canvasGroup in listButtonCanvasGroup)
            {
                canvasGroup.alpha = 0f;
            }

            txtInstruction.SetAlpha(0f);
            txtInstruction.DOFade(1f, 3f).SetTarget(txtInstruction).SetDelay(2f).SetLoops(-1, LoopType.Yoyo);
            groupStudioLogos.alpha = 0f;
            groupLeaderboardPreview.alpha = 0f;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (isShowHome) return;
            if (isAnimating) return;
            isAnimating = true;

            DoShow().OnComplete(() =>
            {
                imgBlockRaycast.enabled = false;
                isShowHome = true;
                isAnimating = false;
            });
        }

        private Tween DoShow()
        {
            DOTween.Kill(this);
            DOTween.Kill(txtInstruction);
            txtInstruction.DOFade(0f, 0.2f);
            var seq = DOTween.Sequence(this);
            seq.Append(rectBackground.DOLocalMove(bgAppearPosition, durationTitleAndBackground).SetEase(Ease.OutQuad))
                .Join(rectTitle.DOLocalMove(titleAppearPosition, durationTitleAndBackground).SetEase(Ease.OutQuad))
                .Join(rectTitle.DOScale(titleAppearScale, durationTitleAndBackground).SetEase(Ease.OutQuad));
            
            for (var i = 0; i < listButtonCanvasGroup.Count; i++)
            {
                var canvasGroup = listButtonCanvasGroup[i];
                var targetPosition = listButtonShowPosition[i];
                canvasGroup.transform.localPosition = targetPosition + buttonOffsetOnHide;
                canvasGroup.alpha = 0f;
                seq.AppendCallback(() =>
                    {
                        canvasGroup.DOFade(1f, durationEachButton).SetEase(Ease.OutQuad);
                        canvasGroup.transform.DOLocalMove(targetPosition, durationEachButton).SetEase(Ease.OutQuad);
                    })
                    .AppendInterval(delayEachButton);
            }

            seq.Append(groupStudioLogos.DOFade(1f, durationEachButton))
                .Append(groupLeaderboardPreview.DOFade(1f, durationEachButton));
            
            return seq;
        }

        private Tween DoHide()
        {
            DOTween.Kill(this);
            var seq = DOTween.Sequence(this);
            
            seq.Append(groupStudioLogos.DOFade(0f, durationEachButton))
                .Join(groupLeaderboardPreview.DOFade(0f, durationEachButton));
            
            for (var i = listButtonCanvasGroup.Count - 1; i >= 0 ; i--)
            {
                var canvasGroup = listButtonCanvasGroup[i];
                var targetPosition = listButtonShowPosition[i] + buttonOffsetOnHide;
                seq.AppendCallback(() =>
                    {
                        canvasGroup.DOFade(0f, durationEachButton).SetEase(Ease.OutQuad);
                        canvasGroup.transform.DOLocalMove(targetPosition, durationEachButton).SetEase(Ease.OutQuad);
                    })
                    .AppendInterval(delayEachButton);
            }
            
            seq.AppendInterval(delayEachButton * (listButtonCanvasGroup.Count - 2) + durationEachButton - 0.2f);
            
            seq.Append(rectBackground.DOLocalMove(bgHidePosition, durationTitleAndBackground).SetEase(Ease.OutQuad))
                .Join(rectTitle.DOLocalMove(titleHidePosition, durationTitleAndBackground).SetEase(Ease.OutQuad))
                .Join(rectTitle.DOScale(titleHideScale, durationTitleAndBackground).SetEase(Ease.OutQuad));

            
            return seq;
        }
    }
}