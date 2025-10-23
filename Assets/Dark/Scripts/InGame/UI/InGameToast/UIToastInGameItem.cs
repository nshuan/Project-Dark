using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI.InGameToast
{
    public class UIToastInGameItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtMessage;
        [SerializeField] private Image imgIcon;
        [SerializeField] private Image imgLight;
        
        public Tween DoShowToast(ToastInGame toast)
        {
            transform.localScale = new Vector3(1f, 0f, 1f);
            txtMessage.SetText(toast.message);
            txtMessage.SetAlpha(0f);
            imgIcon.sprite = toast.icon;
            imgIcon.SetAlpha(0f);
            imgIcon.transform.localScale = 0f * Vector3.one;
            imgIcon.SetNativeSize();
            imgLight.SetAlpha(0f);
            gameObject.SetActive(true);

            var seq =  DOTween.Sequence(this)
                .AppendCallback(() =>
                {
                    txtMessage.DOFade(1f, 0.15f);
                    imgLight.DOFade(0.6f, 0.1f);
                })
                .Append(transform.DOScaleY(1f, 0.2f));

            if (toast.icon)
            {
                seq.AppendCallback(() =>
                    {
                        imgIcon.DOFade(1f, 0.15f);
                    })
                    .Append(imgIcon.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack));
            }
            else seq.AppendInterval(0.5f);
                
            seq.AppendCallback(() =>
                {
                    transform.DOScale(0.85f, 0.2f);
                    imgLight.DOFade(0f, 0.3f);
                })
                .AppendInterval(3f)
                .Append(imgIcon.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack))
                .Append(transform.DOScale(0f, 0.2f))
                .OnUpdate(() =>
                {
                    LayoutRebuilder.MarkLayoutForRebuild((RectTransform)transform.parent);
                });

            return seq;
        }
    }
}