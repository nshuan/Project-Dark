using System;
using System.Collections;
using Coffee.UIExtensions;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace InGame.UI.LevelIntro
{
    public class UILevelQuickIntro : MonoBehaviour
    {
        [SerializeField] private CanvasGroup cvgContent;
        [SerializeField] private CanvasGroup cvgLevelFrame;
        [SerializeField] private CanvasGroup cvgTxtLevel;
        [SerializeField] private RectTransform rectBossNameFrame;
        [SerializeField] private UIParticle vfxLevelFrame;

        [Space]
        [SerializeField] private TextMeshProUGUI txtLevel;
        [SerializeField] private TextMeshProUGUI txtBossName;
        
        private Coroutine coroutineQuickIntro;
        
        private void Awake()
        {
            LevelManager.Instance.OnLevelPreLoaded += OnLevelPreLoaded;
        }

        private void OnLevelPreLoaded(LevelConfig level)
        {
            txtLevel.SetText(level.level.ToString());
            txtBossName.SetText(LevelManager.Instance.LevelBossName);
            
            if (coroutineQuickIntro != null) StopCoroutine(coroutineQuickIntro);
            coroutineQuickIntro = StartCoroutine(IEQuickIntro());
        }

        private IEnumerator IEQuickIntro()
        {
            var targetRotation =
                new Vector3(0f, 0f, (Mathf.RoundToInt(cvgLevelFrame.transform.rotation.z / 180f) + 1) * 180f);
            var bossNameFrameLength = rectBossNameFrame.sizeDelta;
            cvgLevelFrame.transform.localScale = Vector3.one;
            cvgLevelFrame.alpha = 0f;
            cvgTxtLevel.alpha = 0f;
            rectBossNameFrame.sizeDelta = new Vector2(0f, rectBossNameFrame.sizeDelta.y);
            
            cvgContent.gameObject.SetActive(true);
            cvgContent.alpha = 1f;
            yield return DOTween.Sequence(this)
                .AppendCallback(() =>
                {
                    vfxLevelFrame.gameObject.SetActive(true);
                    vfxLevelFrame.Play();
                })
                .Append(cvgLevelFrame.transform.DOLocalRotate(targetRotation, 0.4f).SetRelative())
                .Join(cvgLevelFrame.transform.DOScale(1.1f, 0.4f).SetEase(Ease.OutQuad))
                .Join(cvgLevelFrame.DOFade(1f, 0.4f).SetEase(Ease.OutQuad))
                .Join(cvgTxtLevel.DOFade(1f, 0.4f))
                .Append(cvgLevelFrame.transform.DOScale(1f, 0.25f).SetEase(Ease.InQuad))
                .Append(rectBossNameFrame.DOSizeDelta(bossNameFrameLength, 0.5f).SetEase(Ease.OutQuad))
                .AppendInterval(0.7f)
                .Append(cvgContent.DOFade(0f, 0.5f)).WaitForCompletion();
            cvgContent.gameObject.SetActive(false);
            vfxLevelFrame.gameObject.SetActive(false);
        }
    }
}