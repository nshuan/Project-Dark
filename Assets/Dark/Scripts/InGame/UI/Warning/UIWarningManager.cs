using Core;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI
{
    public class UIWarningManager : MonoSingleton<UIWarningManager>
    {
        [SerializeField] private Image imgVignette;

        public void WarnOnce(bool overrideCurrent)
        {
            if (!overrideCurrent && DOTween.IsTweening(imgVignette)) return;
            
            DOTween.Kill(imgVignette);
            imgVignette.SetAlpha(0f);
            imgVignette.gameObject.SetActive(true);
            
            DOTween.Sequence(imgVignette)
                .Append(imgVignette.DOFade(1f, 0.1f))
                .Append(imgVignette.DOFade(0f, 1.5f))
                .AppendCallback(() => imgVignette.gameObject.SetActive(false));
        }
    }
}