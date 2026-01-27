using Coffee.UIExtensions;
using DG.Tweening;
using InGame;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeLine : MonoBehaviour
    {
        [SerializeField] private GameObject lineAvailable;
        [SerializeField] private GameObject lineLocked;
        [SerializeField] private Gradient2 lineActivated;
        [SerializeField] private GameObject lineGlow;
        [SerializeField] private UIParticle vfxUnlock;
        [SerializeField] private CanvasGroup groupLine;
        public float activateDuration = 0.3f;

        private UIUpgradeNodeState currentState = UIUpgradeNodeState.Activated;
        
        public void UpdateLineState(UIUpgradeNodeState state)
        {
            // if (currentState == UIUpgradeNodeState.Locked && state == UIUpgradeNodeState.Available)
                // vfxUnlock.Play();
            
            currentState = state;
            lineAvailable.SetActive(state == UIUpgradeNodeState.Available);
            lineLocked.SetActive(GameConst.HideLockedNode == false && state == UIUpgradeNodeState.Locked);
            lineActivated.gameObject.SetActive(state == UIUpgradeNodeState.Activated);
            lineGlow.SetActive(state == UIUpgradeNodeState.Activated);
            if (state == UIUpgradeNodeState.Locked) groupLine.alpha = GameConst.HideLockedNode ? 0f : 1f;
            else groupLine.alpha = 1f;
        }
        
        public Tween DoActivate()
        {
            DOTween.Kill(this, true);
            lineActivated.Offset = -1f;
            lineActivated.gameObject.SetActive(true);
            return DOTween.Sequence(this)
                .AppendCallback(() =>
                {
                    groupLine.alpha = 1f;
                })
                .Append(DOTween.To(() => -1f, x =>
                {
                    lineActivated.Offset = x;
                }, 1f, activateDuration).SetEase(Ease.OutQuad));
        }

        public Tween DoSpawn()
        {
            groupLine.alpha = 0f;
            return groupLine.DOFade(1f, 0.1f);
        }
    }
}