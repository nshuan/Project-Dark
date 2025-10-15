using Core;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace InGame.UI.WarningWave
{
    public class UIWarningWaveManager : MonoSingleton<UIWarningWaveManager>
    {
        [Header("Normal Wave")]
        [SerializeField] private RectTransform panelWarningNormalWave;
        [SerializeField] private TextMeshProUGUI txtWarningNormalWave;
        [SerializeField] private CanvasGroup infoWarningNormalWave;
        
        [Space] [Header("Boss Wave")]
        [SerializeField] private RectTransform panelWarningBossWave;
        [SerializeField] private TextMeshProUGUI txtWarningBossWave;

        protected override void Awake()
        {
            base.Awake();

            LevelManager.Instance.OnWaveStart += OnWaveStart;
        }

        private void OnWaveStart(int waveIndex, float timeToEnd)
        {
            WarnWave(waveIndex);
        }
        
        public void WarnWave(int wave)
        {
            txtWarningNormalWave.SetText($"wave {wave}");
            DoWarning(panelWarningNormalWave, infoWarningNormalWave);
        }

        public Tween DoWarning(RectTransform panel, CanvasGroup info)
        {
            const float delayHide = 0.8f;
            
            DOTween.Kill(panel);
            
            panel.gameObject.SetActive(false);
            panel.localScale = new Vector3(0f, 0.02f, 0f);
            info.alpha = 0f;
            info.transform.localScale = new Vector3(1f, 0.8f, 1f);

            var seq = DOTween.Sequence(panel)
                .AppendCallback(() =>
                {
                    panel.gameObject.SetActive(true);
                })
                .Append(panel.DOScaleX(1f, 0.2f).SetEase(Ease.InQuad))
                .Append(panel.DOScaleY(1f, 0.2f).SetEase(Ease.OutQuad))
                .Append(info.transform.DOScaleY(1f, 0.2f).SetEase(Ease.OutQuad))
                .Join(info.DOFade(1f, 0.2f).SetEase(Ease.OutQuad))
                .AppendInterval(delayHide)
                .Append(panel.DOScaleY(0f, 0.2f).SetEase(Ease.InQuad));
            
            return seq;
        }
    }
}