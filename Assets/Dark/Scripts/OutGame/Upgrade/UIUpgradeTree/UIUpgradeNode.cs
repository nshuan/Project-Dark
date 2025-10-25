using System;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIExtensions;
using Dark.Scripts.Audio;
using DG.Tweening;
using InGame.Upgrade;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeNode : MonoBehaviour
    {
        public UIUpgradeTree treeRef;
        public UpgradeNodeConfig config;
        public List<UIUpgradeLineInfo> preRequireLines;
        
        [Space]
        [Header("UI")]
        [SerializeField] protected UIUpgradeNodeHoverField hoverField;

        [SerializeField] protected Image nodeVisual;
        [SerializeField] protected Image nodeLockVisual;
        [SerializeField] protected Transform imgBorder;
        [SerializeField] protected GameObject imgActivatedGlow;
        [SerializeField] protected GameObject imgActivatedMaxGlow;
        [SerializeField] protected Transform rectActivatedMaxOutline;
        [SerializeField] protected GameObject imgAvailable;
        [SerializeField] protected GameObject imgLock;
        [SerializeField] protected UIParticle vfxActivate;
        [SerializeField] protected AudioComponent sfxUnlockSuccess;
        [SerializeField] protected AudioComponent sfxUnlockFailure;
        [SerializeField] protected TextMeshProUGUI txtNodeLevel;
        public float lineAnchorOffsetRadius;

        protected virtual void OnEnable()
        {
            UpdateUI();
        }

        public void SetVisual(Sprite sprite, Sprite lockSprite)
        {
            nodeVisual.sprite = sprite;
            nodeLockVisual.sprite = lockSprite;
            nodeVisual.SetNativeSize();
            nodeLockVisual.SetNativeSize();
        }
        
        public virtual void UpdateUI()
        {
            var data = UpgradeManager.Instance.GetData(config.nodeId);
            if (data == null || data.level == 0) // Not activated yet
            {
                // Always available or all pre-required nodes are activated
                if (config.preRequire == null || config.preRequire.Length == 0 || config.preRequire.All((preRequire) => UpgradeManager.Instance.GetData(preRequire.nodeId) != null && UpgradeManager.Instance.GetData(preRequire.nodeId).level > 0))
                {
                    txtNodeLevel.SetText($"0/{config.MaxLevel}");
                    txtNodeLevel.transform.parent.gameObject.SetActive(true);
                    if (config.preRequire == null || config.preRequire.Length == 0)
                        txtNodeLevel.transform.parent.gameObject.SetActive(false);
                    imgAvailable.SetActive(true);
                    imgLock.SetActive(false);
                    imgActivatedGlow.SetActive(false);
                    imgActivatedMaxGlow.SetActive(false);
                    rectActivatedMaxOutline.gameObject.SetActive(false);
                    imgBorder.gameObject.SetActive(true);

                    foreach (var lineInfo in preRequireLines)
                    {
                        lineInfo.line.UpdateLineState(UIUpgradeNodeState.Available);
                    }
                }
                else
                {
                    txtNodeLevel.transform.parent.gameObject.SetActive(false);
                    imgAvailable.SetActive(false);
                    imgLock.SetActive(true);
                    imgActivatedGlow.SetActive(false);
                    imgActivatedMaxGlow.SetActive(false);
                    rectActivatedMaxOutline.gameObject.SetActive(false);
                    imgBorder.gameObject.SetActive(true);
                    
                    foreach (var lineInfo in preRequireLines)
                    {
                        lineInfo.line.UpdateLineState(
                            UpgradeManager.Instance.GetData(lineInfo.preRequireId) == null
                            ? UIUpgradeNodeState.Locked
                            : UIUpgradeNodeState.Available);
                    }
                }
            }
            else // Activated
            {
                txtNodeLevel.SetText($"{data.level}/{config.MaxLevel}");
                txtNodeLevel.transform.parent.gameObject.SetActive(true);
                if (config.preRequire == null || config.preRequire.Length == 0)
                    txtNodeLevel.transform.parent.gameObject.SetActive(false);
                imgAvailable.SetActive(true);
                imgLock.SetActive(false);
                vfxActivate?.Play();
                imgActivatedGlow.SetActive(data.level < config.MaxLevel);
                if (data.level >= config.MaxLevel)
                {
                    imgActivatedMaxGlow.SetActive(true);
                    rectActivatedMaxOutline.gameObject.SetActive(true);
                    imgBorder.gameObject.SetActive(false);
                    DOTween.Kill(rectActivatedMaxOutline);
                    DOTween.Sequence(rectActivatedMaxOutline)
                        .Append(rectActivatedMaxOutline.DOLocalRotate(new Vector3(0f, 0f, 180f), 0.4f).SetRelative())
                        .Join(rectActivatedMaxOutline.DOScale(1.2f, 0.4f).SetEase(Ease.OutQuad))
                        .Append(rectActivatedMaxOutline.DOScale(1f, 0.2f).SetEase(Ease.InQuad));
                }
                else
                {
                    imgActivatedMaxGlow.SetActive(false);
                    rectActivatedMaxOutline.gameObject.SetActive(false);
                    imgBorder.gameObject.SetActive(true);
                }
                
                
                foreach (var lineInfo in preRequireLines)
                {
                    lineInfo.line.UpdateLineState(UIUpgradeNodeState.Activated);
                }
            }
            
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
                // treeRef.SelectNode(this);
                if (config.preRequire != null && config.preRequire.Select((node) => node.nodeId)
                        .Any((id) =>
                            UpgradeManager.Instance.GetData(id) == null ||
                            UpgradeManager.Instance.GetData(id).level == 0))
                {
                    UIUpgradeNodeInfoPreview.Instance.Shake();
                    sfxUnlockFailure?.Play();
                    return;
                }
                
                var success = UpgradeManager.Instance.UpgradeNode(config.nodeId);
                if (success)
                {
                    UIUpgradeNodeInfoPreview.Instance.Setup(config, true);
                    UIUpgradeNodeInfoPreview.Instance.Show(transform.position, new Vector2(hoverField.rectTransform.sizeDelta.x / 2, 0f), true, () => hoverField.interactable = true);
                    UpdateUI();
                    DoUpgrade().Play();
                    treeRef.UpdateChildren(config.nodeId);
                    sfxUnlockSuccess?.Play();
                }
                else
                {
                    UIUpgradeNodeInfoPreview.Instance.Shake();
                    sfxUnlockFailure?.Play();
                }
            };
        }

        private Tween DoUpgrade()
        {
            DOTween.Complete(this);
            return DOTween.Sequence(this)
                .Append(imgBorder.DOLocalRotate(new Vector3(0f, 0f, 180f), 0.4f).SetRelative())
                .Join(imgBorder.DOScale(1.2f, 0.4f).SetEase(Ease.OutQuad))
                .Append(imgBorder.DOScale(1f, 0.2f).SetEase(Ease.InQuad));
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, lineAnchorOffsetRadius);
        }

#if UNITY_EDITOR
        public void SetIconNormal(Sprite sprite)
        {
            nodeVisual.sprite = sprite;
            nodeVisual.SetNativeSize();
            EditorUtility.SetDirty(nodeVisual);
        }

        public void SetIconLocked(Sprite sprite)
        {
            nodeLockVisual.sprite = sprite;
            nodeLockVisual.SetNativeSize();
            EditorUtility.SetDirty(nodeLockVisual);
        }
#endif
    }

    [Serializable]
    public class UIUpgradeLineInfo
    {
        public int preRequireId;
        public UIUpgradeLine line;
    }

    public enum UIUpgradeNodeState
    {
        Locked,
        Available,
        Activated
    }
}