using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Economic.UI
{
    public class UIEconomic : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] protected bool showInstruction = false;
        [SerializeField] protected GameObject panelInstruction;
        [SerializeField] protected Transform imgIcon;

        public Transform parentTxtChanged;
        public TextMeshProUGUI prefabTxtChanged;

        [Space] [Header("Config")] 
        public Color colorIncrease;
        public Color colorDecrease;
        
        private Queue<TextMeshProUGUI> poolTxtChanged = new Queue<TextMeshProUGUI>();
        private int totalActiveTxt; // limit total active text
        private int maxActiveTxt = 10;
        private TextMeshProUGUI lastActiveTxt;
        
        public static Action<WealthType> OnEconomicIconHoverIn { get; set; }
        public static Action OnEconomicIconHoverOut { get; set; }
        
        protected int current;
        protected int target;
        protected float updateInterval = 0.05f;
        protected float maxUpdateDuration = 3f;

        private void Awake()
        {
            InitPool();
        }

        public virtual void UpdateUI()
        {
            
        }
        
        public void AnimateUpdating(int target)
        {
            if (target == this.target) return;
            if (target > this.target)
            {
                ShowChanged($"+{target - this.target}", colorIncrease);
            }
            else
            {
                ShowChanged($"-{this.target - target}", colorDecrease);
            }
            
            this.target = target;
            current = target;
            UpdateUI();

            // DoAnimatedUpdating().OnComplete(() =>
            // {
            //     current = this.target;
            //     UpdateUI();
            // });
        }

        private Tween DoAnimatedUpdating()
        {
            DOTween.Kill(this);
            var seq = DOTween.Sequence(this);
            var stepSize = 1;
            var step = (int)(maxUpdateDuration / updateInterval);
            if (current < target)
            {
                if ((target - current) * updateInterval > maxUpdateDuration)
                    stepSize = (int)((target - current) / maxUpdateDuration * updateInterval);
                else step = target - current;

                TweenCallback actionUpdate = () =>
                {
                    current += stepSize;
                    UpdateUI();
                };
                
                for (var i = 0; i < step; i++)
                {
                    seq.AppendCallback(actionUpdate)
                        .AppendInterval(updateInterval);
                }
                
            }
            else if (current > target)
            {
                if ((-target + current) * updateInterval > maxUpdateDuration)
                    stepSize = (int)((-target + current) / maxUpdateDuration * updateInterval);
                else step = -target + current;
                
                TweenCallback actionUpdate = () =>
                {
                    current -= stepSize;
                    UpdateUI();
                };
                
                for (var i = 0; i < step; i++)
                {
                    seq.AppendCallback(actionUpdate)
                        .AppendInterval(updateInterval);
                }
            }

            seq.AppendCallback(() =>
            {
                current = target;
                UpdateUI();
            });

            return seq;
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
             if (!showInstruction) return;
            panelInstruction.SetActive(false);
            OnEconomicIconHoverOut?.Invoke();
       }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (showInstruction)
            {
                panelInstruction.SetActive(!panelInstruction.gameObject.activeSelf);
            }
        }
        
        #region Changed value

        protected void InitPool()
        {
            poolTxtChanged = new Queue<TextMeshProUGUI>();
            totalActiveTxt = 0;
        }

        protected void ShowChanged(string value, Color color)
        {
            TextMeshProUGUI txt;
            if (totalActiveTxt >= maxActiveTxt)
            {
                if (!lastActiveTxt) return;
                DOTween.Kill(lastActiveTxt);
                txt = lastActiveTxt;
            }
            else
            {
                if (!poolTxtChanged.TryDequeue(out txt))
                {
                    txt = Instantiate(prefabTxtChanged, parentTxtChanged);
                }

                totalActiveTxt += 1;
            }
            
            DOTween.Kill(txt);
            txt.transform.localPosition = new Vector3(0f, -10f, 0f);
            txt.SetText(value);
            color.a = 1f;
            txt.color = color;
            txt.gameObject.SetActive(true);
            txt.DOFade(0f, 1f).SetEase(Ease.InQuad).SetTarget(txt);
            txt.transform.DOLocalMoveY(10f, 1f).SetEase(Ease.OutQuad).SetTarget(txt)
                .OnComplete(() => Release(txt));

            if (!DOTween.IsTweening(transform))
            {
                DOTween.Kill(transform);
                transform.localScale = Vector3.one;
                transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.1f).SetTarget(transform).SetUpdate(true);
            }
        }

        protected void Release(TextMeshProUGUI txt)
        {
            DOTween.Kill(txt);
            txt.gameObject.SetActive(false);
            poolTxtChanged.Enqueue(txt);
            totalActiveTxt -= 1;
        }

        #endregion
    }
}