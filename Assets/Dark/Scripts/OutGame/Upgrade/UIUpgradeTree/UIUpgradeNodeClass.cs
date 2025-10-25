using InGame.Upgrade;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeNodeClass : UIUpgradeNode
    {
        protected override void OnEnable()
        {
            UpdateUI();
            
            if (config.preRequire == null || config.preRequire.Length == 0)
            {
                UpgradeManager.Instance.UpgradeNode(config.nodeId);
                UpdateUI();
                treeRef.UpdateChildren(config.nodeId);
            }
        }

        public override void UpdateUI()
        {
            txtNodeLevel.transform.parent.gameObject.SetActive(false);
            imgAvailable.SetActive(true);
            imgLock.SetActive(false);
            imgActivatedGlow.SetActive(false);
            imgBorder.gameObject.SetActive(true);
            imgActivatedMaxGlow.gameObject.SetActive(false);
            rectActivatedMaxOutline.gameObject.SetActive(false);

            hoverField.onHover = () =>
            {
                UIUpgradeNodeInfoPreview.Instance.Setup(config, false);
                UIUpgradeNodeInfoPreview.Instance.Show(transform.position, new Vector2(hoverField.rectTransform.sizeDelta.x / 2, 0f), false, () => hoverField.interactable = true);
            };
            hoverField.onHoverExit = () =>
            {
                hoverField.interactable = false;
                UIUpgradeNodeInfoPreview.Instance.Hide(false);
            };
            hoverField.onPointerClick = () =>
            {
                UIUpgradeNodeInfoPreview.Instance.Shake();
                sfxUnlockFailure?.Play();
            };
        }
    }
}