using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.Credits
{
    public class UIGameCreditsScroll : MonoBehaviour
    {
        [SerializeField] private CanvasGroup cvgCredits;
        [SerializeField] protected RectTransform content;
        [SerializeField] private RectTransform anchorStart;
        [SerializeField] protected RectTransform anchorEnd;
        [SerializeField] private Button btnClose;
        [SerializeField] private CanvasGroup cvgButtonClose;

        [Space] [Header("Config")] 
        [SerializeField] protected float movePointPerUpdate = 1;
        
        protected bool started;

        private void Awake()
        {
            btnClose.onClick.RemoveAllListeners();
            btnClose.onClick.AddListener(() => CloseCredits(0f));
        }

        private void OnEnable()
        {
            DOTween.Kill(this);
            
            cvgCredits.alpha = 0f;
            cvgButtonClose.alpha = 0f;
            btnClose.interactable = false;
            content.position = anchorStart.position;

            var seq = DOTween.Sequence(this).SetUpdate(true);
            seq.Append(cvgCredits.DOFade(1f, 0.5f))
                .AppendInterval(1f)
                .AppendCallback(StartCredits)
                .Append(cvgButtonClose.DOFade(1f, 0.2f))
                .AppendCallback(() => btnClose.interactable = true);
        }

        [Button]
        protected virtual void StartCredits()
        {
            started = true;
        }

        [Button]
        protected void StopCredits()
        {
            started = false;
        }
        
        protected virtual void Update()
        {
            if (!started) return;
            content.localPosition += new Vector3(0f, movePointPerUpdate, 0f);
            if (content.position.y > anchorEnd.position.y)
            {
                StopCredits();
                CloseCredits(2f);
            }
        }

        private void CloseCredits(float delay)
        {
            btnClose.interactable = false;

            DOTween.Kill(this);
            var seq = DOTween.Sequence(this).SetUpdate(true);
            seq.AppendInterval(delay)
                .Append(cvgButtonClose.DOFade(0f, 0.2f))
                .Append(cvgCredits.DOFade(0f, 0.5f))
                .AppendCallback(() =>
                {
                    gameObject.SetActive(false);
                });
            seq.Play();
        }
    }
}