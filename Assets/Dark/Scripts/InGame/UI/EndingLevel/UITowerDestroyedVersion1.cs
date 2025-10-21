using DG.Tweening;
using UnityEngine;

namespace InGame.UI.EndingLevel
{
    public class UITowerDestroyedVersion1 : MonoBehaviour
    {
        private static readonly int MatBreakTime = Shader.PropertyToID("_BreakTime");
        
        [SerializeField] private CanvasGroup groupEffect;
        [SerializeField] private Material towerSilhouetteMat;

        public void Play()
        {
            groupEffect.alpha = 0f;
            towerSilhouetteMat.SetFloat(MatBreakTime, 0);
            groupEffect.gameObject.SetActive(true);

            DOTween.Kill(this);
            DOTween.Sequence(this)
                .Append(groupEffect.DOFade(1f, 0.1f))
                .Append(DOTween.To(() => 0f, x => towerSilhouetteMat.SetFloat(MatBreakTime, x), 1f, 2f).SetEase(Ease.InQuad));
        }
    }
}