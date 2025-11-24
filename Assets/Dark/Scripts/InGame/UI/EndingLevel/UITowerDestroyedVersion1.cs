using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace InGame.UI.EndingLevel
{
    public class UITowerDestroyedVersion1 : MonoBehaviour
    {
        private static readonly int MatBreakTime = Shader.PropertyToID("_BreakTime");
        
        [SerializeField] private CanvasGroup groupEffect;
        [SerializeField] private UIEndingTowerAnim[] towerAnims;
        [SerializeField] private string animationName;

        public void Play()
        {
            groupEffect.alpha = 0f;
            for (var i = 0; i < towerAnims.Length; i++)
            {
                towerAnims[i].PlayIdle(LevelManager.Instance.Towers[i].CurrentState);
            }
            groupEffect.gameObject.SetActive(true);

            var destroyedId = 0;
            for (var i = 0; i < towerAnims.Length; i++)
            {
                if (LevelManager.Instance.Towers[i].IsDestroyed)
                {
                    destroyedId = i;
                    break;
                }
            }
            DOTween.Kill(this);
            DOTween.Sequence(this)
                .Append(groupEffect.DOFade(1f, 0.1f))
                .AppendCallback(() =>
                {
                    towerAnims[destroyedId].BreakFrom30();
                })
                .AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    for (var i = 0; i < towerAnims.Length; i++)
                    {
                        if (i != destroyedId)
                            towerAnims[i].BreakFromState(LevelManager.Instance.Towers[i].CurrentState);
                    }
                });
        }
    }
}