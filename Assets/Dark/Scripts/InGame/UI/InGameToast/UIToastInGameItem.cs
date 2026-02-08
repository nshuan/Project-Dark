using System;
using System.Collections;
using Dark.Tools.Language.Runtime;
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

        private Coroutine coroutineShowToast;
        private bool inProgress;
        
        public void ShowToast(ToastInGame toast, Action callbackComplete)
        {
            if (coroutineShowToast != null) StopCoroutine(coroutineShowToast);
            coroutineShowToast = StartCoroutine(IEShowToast(toast, callbackComplete));
        }
        
        private IEnumerator IEShowToast(ToastInGame toast, Action callbackComplete)
        {
            transform.localScale = new Vector3(1f, 0f, 1f);
            txtMessage.SetTextValueLanguage(toast.message);
            txtMessage.SetAlpha(0f);
            imgIcon.sprite = toast.icon;
            imgIcon.SetAlpha(0f);
            imgIcon.transform.localScale = 0f * Vector3.one;
            imgIcon.SetNativeSize();
            imgLight.SetAlpha(0f);
            gameObject.SetActive(true);

            inProgress = true;
            
            txtMessage.DOFade(1f, 0.15f).SetUpdate(true);
            imgLight.DOFade(0.6f, 0.1f).SetUpdate(true);

            yield return transform.DOScaleY(1f, 0.2f).WaitForCompletion();

            if (toast.icon)
            {
                imgIcon.DOFade(1f, 0.15f).SetUpdate(true);
                yield return imgIcon.transform.DOScale(1f, 0.2f).SetUpdate(true).SetEase(Ease.OutBack).WaitForCompletion();
            }
            else
            {
                yield return new WaitForSecondsRealtime(0.5f);
            }
                
            transform.DOScale(0.85f, 0.2f).SetUpdate(true);
            imgLight.DOFade(0f, 0.3f).SetUpdate(true);

            yield return new WaitForSecondsRealtime(3f);
            yield return imgIcon.transform.DOScale(0f, 0.3f).SetUpdate(true).SetEase(Ease.InBack).WaitForCompletion();
            yield return transform.DOScale(0f, 0.2f).SetUpdate(true).WaitForCompletion();
            inProgress = false;
        }

        private void Update()
        {
            if (inProgress) LayoutRebuilder.MarkLayoutForRebuild((RectTransform)transform.parent);
        }
    }
}