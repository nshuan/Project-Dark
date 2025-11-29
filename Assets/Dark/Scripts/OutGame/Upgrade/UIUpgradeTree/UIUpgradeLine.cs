using Coffee.UIExtensions;
using DG.Tweening;
using InGame;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeLine : MonoBehaviour
    {
        [SerializeField] private GameObject lineAvailable;
        [SerializeField] private GameObject lineLocked;
        [SerializeField] private GameObject lineActivated;
        [SerializeField] private Image lineActivatedMask;
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
            lineActivated.SetActive(state == UIUpgradeNodeState.Activated);
            lineGlow.SetActive(state == UIUpgradeNodeState.Activated);
            if (state == UIUpgradeNodeState.Locked) groupLine.alpha = GameConst.HideLockedNode ? 0f : 1f;
            else groupLine.alpha = 1f;
        }
        
        public Tween DoActivate()
        {
            DOTween.Kill(this, true);
            lineActivatedMask.gameObject.SetActive(false);
            lineActivated.SetActive(true);
            return DOTween.Sequence(this)
                .AppendCallback(() =>
                {
                    lineActivatedMask.fillAmount = 0f;
                    lineActivatedMask.gameObject.SetActive(true);
                    groupLine.alpha = 1f;
                })
                .Append(DOTween.To(() => 0f, x =>
                {
                    lineActivatedMask.fillAmount = x;
                }, 1f, activateDuration).SetEase(Ease.OutQuad));
        }

        public Tween DoSpawn()
        {
            groupLine.alpha = 0f;
            return groupLine.DOFade(1f, 0.1f);
        }
    }
}