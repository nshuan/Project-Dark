using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Settings
{
    public class UIPopupSettings : MonoBehaviour
    {
        private static readonly int MatDisolveValue = Shader.PropertyToID("Disolve_Value");

        [SerializeField] private CanvasGroup popupCanvasGroup;
        
        [Space] [Header("Heading")] 
        [SerializeField] private Image imgSymbol;
        [SerializeField] private Image imgSymbolShiny;
        [SerializeField] private CanvasGroup imgTitle;
        [SerializeField] private GameObject vfxLight;
        [SerializeField] private Image imgDayLine;
        [SerializeField] private List<CanvasGroup> listBossIcons;

        [Space] 
        [SerializeField] private float durationShowSymbol = 1f;
        [SerializeField] private float durationShowTitle = 0.5f;
        [SerializeField] private float durationShowDayLine = 1f;
        [SerializeField] private float durationShowBossIcon = 0.25f;

        [Space] [Header("Settings")] 
        [SerializeField] private CanvasGroup groupSettings;
        
        [Space] [Header("Nav")] 
        [SerializeField] private CanvasGroup groupBack;
        [SerializeField] private CanvasGroup groupSave;
        
        private Material matSymbol;
        private Button btnBack;
        private Button btnSave;

        private void Awake()
        {
            matSymbol = new Material(imgSymbol.material);
            imgSymbol.material = matSymbol;

            btnBack = groupBack.GetComponent<Button>();
            btnSave = groupSave.GetComponent<Button>();
        }

        private void OnEnable()
        {
            DoShowUIPopup();
        }

        private void ResetPopupUI()
        {
            popupCanvasGroup.alpha = 0f;
            matSymbol.SetFloat(MatDisolveValue, 1f);
            imgSymbolShiny.gameObject.SetActive(false);
            imgTitle.alpha = 0f;
            vfxLight.SetActive(false);
            imgDayLine.SetAlpha(0f);
            foreach (var icon in listBossIcons)
            {
                icon.alpha = 0f;
            }

            groupSettings.alpha = 0f;
            groupBack.gameObject.SetActive(false);
            groupSave.gameObject.SetActive(false);
            if (btnBack) btnBack.interactable = false;
            if (btnSave) btnSave.interactable = false;
        }
        
        private Tween DoShowUIPopup()
        {
            DOTween.Kill(this);
            var seq = DOTween.Sequence(this).SetUpdate(true);

            seq.AppendCallback(() =>
                {
                    ResetPopupUI();
                })
                .Append(popupCanvasGroup.DOFade(1f, 0.3f))
                .AppendCallback(() =>
                {
                    vfxLight.SetActive(true);
                    imgTitle.DOFade(1f, durationShowTitle).SetUpdate(true);
                    imgDayLine.DOFade(1f, durationShowDayLine).SetUpdate(true);
                    DOTween.To(() => 1f, (x) => matSymbol.SetFloat(MatDisolveValue, x), 0f, durationShowSymbol).SetUpdate(true);
                })
                .AppendInterval(durationShowTitle)
                .AppendCallback(() =>
                {
                    for (var i = 0; i < listBossIcons.Count; i++)
                    {
                        var delay = i * 0.03f;
                        listBossIcons[i].DOFade(1f, durationShowBossIcon).SetDelay(delay).SetEase(Ease.OutQuad).SetUpdate(true);
                    }
                })
                .AppendInterval(0.2f)
                .AppendCallback(() =>
                {
                    groupBack.alpha = 0f;
                    groupSave.alpha = 0f;
                    groupBack.gameObject.SetActive(true);
                    groupSave.gameObject.SetActive(true);
                    groupBack.DOFade(1f, 0.2f).SetEase(Ease.OutQuad).SetDelay(0.2f)
                        .OnComplete(() =>
                        {
                            if (btnBack) btnBack.interactable = true;
                        }).SetUpdate(true);
                    groupSave.DOFade(1f, 0.2f).SetEase(Ease.OutQuad).SetDelay(0.2f)
                        .OnComplete(() =>
                        {
                            if (btnSave) btnSave.interactable = true;
                        }).SetUpdate(true);
                })
                .Append(groupSettings.DOFade(1f, durationShowTitle));
            
            return seq;
        }

        public void ClosePopup()
        {
            if (btnBack) btnBack.interactable = false;
            if (btnSave) btnSave.interactable = false;
            
            DOTween.Kill(this);
            popupCanvasGroup.DOFade(0f, 0.5f).SetTarget(this).SetUpdate(true)
                .OnComplete(() => gameObject.SetActive(false));
        }
    }
}