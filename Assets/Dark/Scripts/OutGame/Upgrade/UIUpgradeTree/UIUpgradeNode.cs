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
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeNode : MonoBehaviour
    {
        public UIUpgradeTree treeRef;
        public UpgradeNodeConfig config;
        public List<UIUpgradePreRequireInfo> preRequires;
        
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
        [SerializeField] protected UIParticle vfxActivateMax;
        [SerializeField] protected AudioComponent sfxUnlockSuccess;
        [SerializeField] protected AudioComponent sfxUnlockFailure;
        [SerializeField] protected TextMeshProUGUI txtNodeLevel;
        [SerializeField] protected GameObject txtNodeMaxLevel;
        public float lineAnchorOffsetRadius;

        protected virtual void OnEnable()
        {
            UpdateUI();
        }

        public virtual void UpdateUI()
        {
            var data = UpgradeManager.Instance.GetData(config.nodeId);
            if (data == null || data.level == 0) // Not activated yet
            {
                // Always available or all pre-required nodes are activated
                if (preRequires == null || preRequires.Count == 0 || preRequires.Any((preRequire) => UpgradeManager.Instance.GetData(preRequire.preRequireId) != null && UpgradeManager.Instance.GetData(preRequire.preRequireId).level > 0))
                {
                    if (config.hideLevelInNode)
                        txtNodeLevel.transform.parent.gameObject.SetActive(false);
                    else
                    {
                        txtNodeLevel.SetText($"0/{config.MaxLevel}");
                        txtNodeLevel.transform.parent.gameObject.SetActive(true);
                    }
                    imgAvailable.SetActive(true);
                    imgLock.SetActive(false);
                    imgActivatedGlow.SetActive(false);
                    imgActivatedMaxGlow.SetActive(false);
                    rectActivatedMaxOutline.gameObject.SetActive(false);
                    imgBorder.gameObject.SetActive(true);

                    foreach (var lineInfo in preRequires)
                    {
                        lineInfo.line.UpdateLineState(
                            UpgradeManager.Instance.GetData(lineInfo.preRequireId) != null && UpgradeManager.Instance.GetData(lineInfo.preRequireId).level > 0
                            ? UIUpgradeNodeState.Available
                            : UIUpgradeNodeState.Locked);
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
                    
                    foreach (var lineInfo in preRequires)
                    {
                        lineInfo.line.UpdateLineState(
                            UpgradeManager.Instance.GetData(lineInfo.preRequireId) == null || UpgradeManager.Instance.GetData(lineInfo.preRequireId).level == 0
                            ? UIUpgradeNodeState.Locked
                            : UIUpgradeNodeState.Available);
                    }
                }
            }
            else // Activated
            {
                if (preRequires is { Count: > 0 } && preRequires.All((preRequire) =>
                        UpgradeManager.Instance.GetData(preRequire.preRequireId) == null ||
                        UpgradeManager.Instance.GetData(preRequire.preRequireId).level == 0))
                {
                    txtNodeLevel.transform.parent.gameObject.SetActive(false);
                    imgAvailable.SetActive(false);
                    imgLock.SetActive(true);
                    imgActivatedGlow.SetActive(false);
                    imgActivatedMaxGlow.SetActive(false);
                    rectActivatedMaxOutline.gameObject.SetActive(false);
                    imgBorder.gameObject.SetActive(true);
                    
                    foreach (var lineInfo in preRequires)
                    {
                        lineInfo.line.UpdateLineState(
                            UpgradeManager.Instance.GetData(lineInfo.preRequireId) == null || UpgradeManager.Instance.GetData(lineInfo.preRequireId).level == 0
                                ? UIUpgradeNodeState.Locked
                                : UIUpgradeNodeState.Available);
                    }
                }
                else
                {
                    if (config.hideLevelInNode)
                        txtNodeLevel.transform.parent.gameObject.SetActive(false);
                    else
                    {
                        txtNodeLevel.SetText($"{data.level}/{config.MaxLevel}");
                        txtNodeMaxLevel.gameObject.SetActive(data.level == config.MaxLevel);
                        txtNodeLevel.transform.parent.gameObject.SetActive(true);
                    }
                    imgAvailable.SetActive(true);
                    imgLock.SetActive(false);
                    imgActivatedGlow.SetActive(data.level < config.MaxLevel);
                    if (data.level >= config.MaxLevel)
                    {
                        imgActivatedMaxGlow.SetActive(true);
                        rectActivatedMaxOutline.gameObject.SetActive(true);
                        imgBorder.gameObject.SetActive(false);
                    }
                    else
                    {
                        imgActivatedMaxGlow.SetActive(false);
                        rectActivatedMaxOutline.gameObject.SetActive(false);
                        imgBorder.gameObject.SetActive(true);
                    }
                    
                    foreach (var lineInfo in preRequires)
                    {
                        lineInfo.line.UpdateLineState(
                            UpgradeManager.Instance.GetData(lineInfo.preRequireId) == null || UpgradeManager.Instance.GetData(lineInfo.preRequireId).level == 0
                            ? UIUpgradeNodeState.Locked
                            : UIUpgradeNodeState.Activated);
                    }
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
                if (preRequires != null && preRequires.Select((node) => node.preRequireId)
                        .All((id) =>
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
                    treeRef.UpgradeAllNodesWithId(config.nodeId);
                    sfxUnlockSuccess?.Play();
                }
                else
                {
                    UIUpgradeNodeInfoPreview.Instance.Shake();
                    sfxUnlockFailure?.Play();
                }
            };
        }

        public void Upgrade()
        {
            UpdateUI();
            // Node chua du dieu kien unlock thi bo qua
            if (preRequires is { Count: > 0 } && preRequires.All((preRequire) =>
                    UpgradeManager.Instance.GetData(preRequire.preRequireId) == null ||
                    UpgradeManager.Instance.GetData(preRequire.preRequireId).level == 0))
                return;
            DoUpgrade().Play();
            treeRef.UpdateChildren(config.nodeId);
        }

        private Tween DoUpgrade()
        {
            DOTween.Complete(this);
            
            if (UpgradeManager.Instance.GetData(config.nodeId).level >= config.MaxLevel)
            {
                DOTween.Kill(rectActivatedMaxOutline);
                vfxActivateMax.Play();
                return DOTween.Sequence(rectActivatedMaxOutline)
                    .Append(rectActivatedMaxOutline.DOLocalRotate(new Vector3(0f, 0f, 180f), 0.4f).SetRelative())
                    .Join(rectActivatedMaxOutline.DOScale(1.2f, 0.4f).SetEase(Ease.OutQuad))
                    .Append(rectActivatedMaxOutline.DOScale(1f, 0.2f).SetEase(Ease.InQuad));
            }

            vfxActivate?.Play();
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
    public class UIUpgradePreRequireInfo
    {
        public int preRequireId;
        public UIUpgradeNode node;
        public UIUpgradeLine line;
    }

    public enum UIUpgradeNodeState
    {
        Locked,
        Available,
        Activated
    }
}