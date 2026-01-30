using System.Collections;
using DG.Tweening;
using InGame.Upgrade;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeNodeClass : UIUpgradeNode
    {
        private bool canSpawnVfx;
        private float cooldownClickSpawnVfx = 2f;

        protected override void Awake()
        {
            defaultScale = nodeVisual.transform.localScale;
            defaultScale.z = 1f;
        }

        public override void UpdateUI()
        {
            currentState = UIUpgradeNodeState.Activated;
            
            txtNodeLevel.transform.parent.gameObject.SetActive(false);
            SetAvailable();
            imgActivatedGlow.SetActive(false);
            imgActivatedMaxGlow.gameObject.SetActive(false);
            rectActivatedMaxOutline.gameObject.SetActive(false);

            hoverField.onHover = () =>
            {
                UIUpgradeNodeInfoPreview.Instance.Setup(this, config, false);
                UIUpgradeNodeInfoPreview.Instance.Show(transform.position, new Vector2(hoverField.nodeRepresentableRect.sizeDelta.x / 2, 0f), false, () => hoverField.interactable = true);
                nodeVisual.transform.localScale = defaultScale;
                nodeVisual.transform.DOScale(new Vector3(0.08f, 0.08f, 0), 0.2f).SetRelative().SetEase(Ease.OutQuad);
            };
            hoverField.onHoverExit = () =>
            {
                hoverField.interactable = false;
                UIUpgradeNodeInfoPreview.Instance.Hide(false);
                nodeVisual.transform.DOScale(defaultScale, 0.2f).SetEase(Ease.InQuad);            
            };
            hoverField.onPointerClick = () =>
            {
                UIUpgradeNodeInfoPreview.Instance.Shake();
                if (!canSpawnVfx) return;
                sfxUnlockSuccess?.Play();
                StartCoroutine(IECooldownClick(cooldownClickSpawnVfx));
                spawnAnimation.SpawnLogic.DoSpawn();
            };

            StartCoroutine(IECooldownClick(5f));
        }

        private IEnumerator IECooldownClick(float duration)
        {
            canSpawnVfx = false;
            yield return new WaitForSeconds(duration);
            canSpawnVfx = true;
        }

        protected override Tween DoUpgrade()
        {
            return DOTween.Sequence();
        }
    }
}