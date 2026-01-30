using System;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIExtensions;
using Dark.Scripts.Analytics;
using Dark.Scripts.AudioV2;
using DG.Tweening;
using Economic;
using InGame.Upgrade;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Upgrade
{
    public enum UpgradeNodeType
    {
        NodeClass,
        NodeSkill,
        NodeEffect,
        NodeStat,
        NodeStat2
    }
    
    public class UIUpgradeNode : MonoBehaviour
    {
        public UIUpgradeTree treeRef;
        public UpgradeNodeConfig config;
        public List<UIUpgradePreRequireInfo> preRequires;
        
        [Space]
        [Header("UI")]
        public UpgradeNodeType nodeType;
        [SerializeField] public RectTransform nodeContent;
        [SerializeField] protected UIUpgradeNodeHoverField hoverField;
        [SerializeField] protected UIUpgradeNodeSpawnAnimation spawnAnimation;

        [SerializeField] protected CanvasGroup groupNode;
        [SerializeField] protected Image nodeVisual;
        [SerializeField] protected Image nodeLockVisual;
        
        [SerializeField] protected GameObject imgActivatedGlow;
        [SerializeField] protected GameObject imgActivatedMaxGlow;
        [SerializeField] protected Transform rectActivatedMaxOutline;
        [SerializeField] protected GameObject imgAvailable;
        [SerializeField] protected Image imgLock;
        [SerializeField] protected GameObject[] imgAvailableDecor;
        [SerializeField] protected GameObject[] imgLockedDecor;
        [SerializeField] protected Image imgIconLock;
        [SerializeField] protected AudioPlayComponentV2 sfxUnlockSuccess;
        [SerializeField] protected AudioPlayComponentV2 sfxUnlockFailure;
        [SerializeField] protected AudioPlayComponentV2 sfxHover;
        [SerializeField] protected TextMeshProUGUI txtNodeLevel;
        [SerializeField] protected GameObject txtNodeMaxLevel;
        public float lineAnchorOffsetRadius;

        protected UIUpgradeNodeState currentState = UIUpgradeNodeState.Locked;
        public UIUpgradeNodeState CurrentState => currentState;

        protected Vector3 defaultScale;

        protected virtual void Awake()
        {
            defaultScale = nodeContent.localScale;
            defaultScale.z = 1f;
        }

        // This function must be called in layer-order
        // Nodes layer 0 should be updated before nodes layer 1,...
        public virtual void UpdateState()
        {
            var data = UpgradeManager.Instance.GetData(config.nodeId);
            if (data == null || data.level == 0) // Not activated yet
            {
                // Always available or all pre-required nodes are activated
                if (preRequires == null || preRequires.Count == 0 || preRequires.Any((preRequire) => preRequire.node.CurrentState == UIUpgradeNodeState.Activated))
                {
                    currentState = UIUpgradeNodeState.Available;
                }
                else
                {
                    // Locked
                    currentState = UIUpgradeNodeState.Locked;
                }
            }
            else 
            {
                // Locked
                if (preRequires is { Count: > 0 } && preRequires.All((preRequire) => preRequire.node.CurrentState != UIUpgradeNodeState.Activated))
                {
                    currentState = UIUpgradeNodeState.Locked;
                }
                else
                {
                    // Activated
                    currentState = UIUpgradeNodeState.Activated;
                }
            }
        }
        
        public virtual void UpdateUI()
        {
            var groupUnlockOrder =
                config.groupId.Min((info) => UpgradeManager.Instance.GetGroupUnlockOrder(info.groupId, false));
            foreach (var logicV2 in config.nodeLogic)
            {
                if (logicV2 is INodeDynamicBonusValueV2 { IsDynamic: true } dynamicLogic)
                {
                    dynamicLogic.OverrideBonusValue(groupUnlockOrder);
                }
            }
            
            var data = UpgradeManager.Instance.GetData(config.nodeId);
            if (data == null || data.level == 0) // Not activated yet
            {
                // Always available or all pre-required nodes are activated
                if (preRequires == null || preRequires.Count == 0 || preRequires.Any((preRequire) => preRequire.node.CurrentState == UIUpgradeNodeState.Activated))
                {
                    currentState = UIUpgradeNodeState.Available;
                    
                    if (config.hideLevelInNode)
                        txtNodeLevel.transform.parent.gameObject.SetActive(false);
                    else
                    {
                        txtNodeLevel.SetText($"0/{config.MaxLevel}");
                        txtNodeLevel.transform.parent.gameObject.SetActive(true);
                    }
                    SetAvailable();
                    imgActivatedGlow.SetActive(false);
                    imgActivatedMaxGlow.SetActive(false);
                    rectActivatedMaxOutline.gameObject.SetActive(false);
                    groupNode.alpha = 1f;

                    if (preRequires != null)
                    {
                        foreach (var lineInfo in preRequires)
                        {
                            lineInfo.line.UpdateLineState(
                                lineInfo.node.CurrentState == UIUpgradeNodeState.Activated
                                ? UIUpgradeNodeState.Available
                                : UIUpgradeNodeState.Locked);
                        }
                    }
                }
                else
                {
                    // Locked
                    currentState = UIUpgradeNodeState.Locked;
                    txtNodeLevel.transform.parent.gameObject.SetActive(false);
                    SetLocked();
                    imgActivatedGlow.SetActive(false);
                    imgActivatedMaxGlow.SetActive(false);
                    rectActivatedMaxOutline.gameObject.SetActive(false);
                    groupNode.alpha = GameConst.HideLockedNode ? 0f : 1f;
                    
                    foreach (var lineInfo in preRequires)
                    {
                        lineInfo.line.UpdateLineState(
                            lineInfo.node.CurrentState != UIUpgradeNodeState.Activated
                            ? UIUpgradeNodeState.Locked
                            : UIUpgradeNodeState.Available);
                    }
                }
            }
            else 
            {
                // Locked
                if (preRequires is { Count: > 0 } && preRequires.All((preRequire) => preRequire.node.CurrentState != UIUpgradeNodeState.Activated))
                {
                    currentState = UIUpgradeNodeState.Locked;
                    txtNodeLevel.transform.parent.gameObject.SetActive(false);
                    SetLocked();
                    imgActivatedGlow.SetActive(false);
                    imgActivatedMaxGlow.SetActive(false);
                    rectActivatedMaxOutline.gameObject.SetActive(false);
                    groupNode.alpha = GameConst.HideLockedNode ? 0f : 1f;
                    
                    foreach (var lineInfo in preRequires)
                    {
                        lineInfo.line.UpdateLineState(
                            lineInfo.node.CurrentState != UIUpgradeNodeState.Activated
                                ? UIUpgradeNodeState.Locked
                                : UIUpgradeNodeState.Available);
                    }
                }
                else
                {
                    // Activated
                    currentState = UIUpgradeNodeState.Activated;
                    
                    if (config.hideLevelInNode)
                        txtNodeLevel.transform.parent.gameObject.SetActive(false);
                    else
                    {
                        txtNodeLevel.SetText($"{data.level}/{config.MaxLevel}");
                        txtNodeMaxLevel.gameObject.SetActive(data.level == config.MaxLevel);
                        txtNodeLevel.transform.parent.gameObject.SetActive(true);
                    }
                    SetAvailable();
                    imgActivatedGlow.SetActive(data.level < config.MaxLevel);
                    if (data.level >= config.MaxLevel)
                    {
                        imgActivatedMaxGlow.SetActive(true);
                        rectActivatedMaxOutline.gameObject.SetActive(true);
                        // imgBorder.gameObject.SetActive(false);
                    }
                    else
                    {
                        imgActivatedMaxGlow.SetActive(false);
                        rectActivatedMaxOutline.gameObject.SetActive(false);
                        // imgBorder.gameObject.SetActive(true);
                    }

                    groupNode.alpha = 1f;
                    
                    foreach (var lineInfo in preRequires)
                    {
                        lineInfo.line.UpdateLineState(
                            lineInfo.node.CurrentState != UIUpgradeNodeState.Activated
                            ? UIUpgradeNodeState.Locked
                            : UIUpgradeNodeState.Activated);
                    }
                }
            }
            
            hoverField.onHover = () =>
            {
                if (GameConst.HideLockedNode && CurrentState == UIUpgradeNodeState.Locked)
                    return;
                if (GameConst.HideLockedAreaByCloud && config.groupId.All((id) =>
                    {
                        if (id.isLockNode) return false;
                        
                        if (UpgradeManager.Instance.TreeConfig &&
                            UpgradeManager.Instance.TreeConfig.nodeGroupsMapById.TryGetValue(id.groupId,
                                out var nodeGroup))
                        {
                            if (UpgradeManager.Instance.GetData(nodeGroup.lockNode.nodeId) is { level: > 0 })
                                return false;
                        }
                        
                        return true;
                    }))
                {
                    return;
                }
                
                DOTween.Kill(transform);
                sfxHover.Play();
                nodeContent.localRotation = Quaternion.identity;
                nodeContent.localScale = defaultScale;
                nodeContent.DOPunchRotation(new Vector3(0f, 0f, 10f), 0.3f, 20, 0.1f).SetTarget(transform);
                nodeContent.DOScale(new Vector3(0.2f, 0.2f, 0), 0.2f).SetRelative().SetEase(Ease.OutQuad);
                UIUpgradeNodeInfoPreview.Instance.Setup(this, config, false);
                UIUpgradeNodeInfoPreview.Instance.Show(transform.position, new Vector2(hoverField.nodeRepresentableRect.sizeDelta.x / 2, 0f), false, () => hoverField.interactable = true);
            };
            hoverField.onHoverExit = () =>
            {
                if (GameConst.HideLockedNode && CurrentState == UIUpgradeNodeState.Locked)
                    return;
                hoverField.interactable = false;
                nodeContent.DOScale(defaultScale, 0.2f).SetEase(Ease.InQuad);
                UIUpgradeNodeInfoPreview.Instance.Hide(false);
            };
            hoverField.onPointerClick = GetActionNodeClick;
        }

        protected virtual void GetActionNodeClick()
        {
            if (GameConst.HideLockedNode && CurrentState == UIUpgradeNodeState.Locked)
                return;

            if (preRequires != null && preRequires.Select((node) => node.preRequireId)
                    .All((id) =>
                        UpgradeManager.Instance.GetData(id) == null ||
                        UpgradeManager.Instance.GetData(id).level == 0))
            {
                UIUpgradeNodeInfoPreview.Instance.Shake();
                UIUpgradeNodeInfoPreview.Instance.PlayVfxUpgrade();;
                sfxUnlockFailure?.Play();
                return;
            }

            if (!UpgradeManager.Instance.CanUpgrade(config.nodeId, config.groupId))
            {
                UIUpgradeNodeInfoPreview.Instance.Shake();
                UIUpgradeNodeInfoPreview.Instance.PlayVfxUpgrade();;
                sfxUnlockFailure?.Play();
                return;
            }
            
            Action actionUpgrade = () =>
            {
                var success = UpgradeManager.Instance.UpgradeNode(config.nodeId, config.groupId);
                if (success)
                {
                    if (treeRef.IsNodeSkill(config.nodeId))
                        LogManager.Log(LogConst.EventLogActivateNode, "skill", config.nodeName);
                    else if (treeRef.IsNodePassive(config.nodeId))
                        LogManager.Log(LogConst.EventLogActivateNode, "passive", config.nodeName);

                    config.ActivateLevel(UpgradeManager.Instance.GetData(config.nodeId).level,
                        ref UIUpgradeNodeInfoPreview.Instance.bonusInfo);
                    UIUpgradeNodeInfoPreview.Instance.Setup(this, config, true);
                    UIUpgradeNodeInfoPreview.Instance.Show(transform.position,
                        new Vector2(hoverField.nodeRepresentableRect.sizeDelta.x / 2, 0f), true,
                        () => hoverField.interactable = true);
                    UIUpgradeNodeInfoPreview.Instance.PlayVfxUpgrade();;

                    treeRef.LastUpgradeNodeId = config.nodeId;
                    treeRef.InvokeNodeUpgraded(this);
                    treeRef.UpgradeAllNodesWithId(config.nodeId);
                    sfxUnlockSuccess?.Play();
                }
            };
            
            // Nếu node có dùng sigil thì phải bật popup confirm
            if (config.costInfo.Any((cost) => cost.costType == WealthType.Sigils))
            {
                var displayCost = UIUpgradeNodeInfoPreview.Instance.GetDisplayCost();
                UIUpgradeScene.Instance.PopupConfirmExchange.Setup(
                    displayCost.Item1,
                    displayCost.Item2,
                    displayCost.Item3,
                    "And you will receive",
                    "Confirm exchange",
                    config.nodeName, 
                    actionUpgrade);
                UIUpgradeScene.Instance.PopupConfirmExchange.DoOpenFadeIn();
            }
            else
            {
                actionUpgrade?.Invoke();   
            }
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
            treeRef.UpdateChildren(config.nodeId, UpgradeManager.Instance.GetData(config.nodeId).level == 1);
        }

        public Tween DoUnlockVfx(int fromId)
        {
            DOTween.Kill(this);
            var seq = DOTween.Sequence(this);
            if (preRequires != null)
            {
                foreach (var lineInfo in preRequires)
                {
                    if (lineInfo.preRequireId != fromId) continue;
                    seq.AppendCallback(() =>
                        {
                            var vfxUnlock = GetVfxUnlock();
                            DOVirtual.DelayedCall(lineInfo.line.activateDuration - 0.1f, () =>
                            {
                                SetAvailable();
                                vfxUnlock?.Play();
                            }).SetTarget(this);

                            DOVirtual.DelayedCall(lineInfo.line.activateDuration + 1f, () =>
                            {
                                ReleaseVfxUnlock(vfxUnlock);
                            }).SetTarget(this);
                        })
                        .Append(lineInfo.line.DoActivate());
                    break;
                }

                if (GameConst.HideLockedNode == false)
                {
                    seq.Append(imgIconLock.transform.DOShakePosition(0.3f, new Vector3(0f, 1f, 0f), vibrato: 30,
                        fadeOut: false, randomnessMode: ShakeRandomnessMode.Harmonic))
                        .Append(imgIconLock.transform.DOLocalMoveY(-5f, 0.5f).SetEase(Ease.OutQuad).SetRelative())
                        .Join(imgIconLock.DOFade(0f, 0.3f))
                        .Join(imgLock.DOFade(0f, 0.3f));
                }
            }

            return seq;
        }
        
        protected virtual Tween DoUpgrade()
        {
            DOTween.Complete(this);
            
            if (UpgradeManager.Instance.GetData(config.nodeId).level >= config.MaxLevel)
            {
                DOTween.Kill(rectActivatedMaxOutline);
                var vfxActivateMax = GetVfxActivateMax();
                vfxActivateMax.Play();
                return DOTween.Sequence(rectActivatedMaxOutline)
                    .Append(rectActivatedMaxOutline.DOLocalRotate(new Vector3(0f, 0f, 180f), 0.4f).SetRelative())
                    .Join(rectActivatedMaxOutline.DOScale(1.2f, 0.4f).SetEase(Ease.OutQuad))
                    .Append(rectActivatedMaxOutline.DOScale(1f, 0.2f).SetEase(Ease.InQuad))
                    .AppendCallback(() => ReleaseVfxActivateMax(vfxActivateMax));
            }

            var vfxActivate = GetVfxActivate();
            vfxActivate.Play();
            return DOTween.Sequence(this)
                .Append(imgAvailable.transform.DOLocalRotate(new Vector3(0f, 0f, 360f), 0.4f).SetRelative())
                .Join(imgAvailable.transform.DOScale(1.2f, 0.4f).SetEase(Ease.OutQuad))
                .Append(imgAvailable.transform.DOScale(1f, 0.2f).SetEase(Ease.InQuad))
                .AppendCallback(() => ReleaseVfxActivate(vfxActivate));
        }
        
        public Tween DoSpawn()
        {
            return spawnAnimation.SpawnLogic.DoSpawn();
        }

        #region Visual

        protected void SetAvailable()
        {
            imgAvailable.SetActive(true);
            imgLock.gameObject.SetActive(false);
            if (imgAvailableDecor != null)
            {
                foreach (var img in imgAvailableDecor)
                {
                    img.SetActive(true);   
                }
            }

            if (imgLockedDecor != null)
            {
                foreach (var img in imgLockedDecor)
                {
                    img.SetActive(false);
                }
            }
        }
        
        protected void SetLocked()
        {
            imgAvailable.SetActive(false);
            imgLock.gameObject.SetActive(true);
            if (imgAvailableDecor != null)
            {
                foreach (var img in imgAvailableDecor)
                {
                    img.SetActive(false);   
                }
            }

            if (imgLockedDecor != null)
            {
                foreach (var img in imgLockedDecor)
                {
                    img.SetActive(true);
                }
            }
        }

        #endregion
        
        #region Vfx
        
        public UIParticle GetVfxUnlock()
        {
            return nodeType switch
            {
                UpgradeNodeType.NodeSkill => UIUpgradeNodeSkillPool.Instance.GetVfxUnlock(transform, true),
                UpgradeNodeType.NodeEffect => UIUpgradeNodeEffectPool.Instance.GetVfxUnlock(transform, true),
                UpgradeNodeType.NodeStat => UIUpgradeNodeStatPool.Instance.GetVfxUnlock(transform, true),
                UpgradeNodeType.NodeStat2 => UIUpgradeNodeStat2Pool.Instance.GetVfxUnlock(transform, true),
                _ => null
            };
        }
        
        public UIParticle GetVfxActivate()
        {
            return nodeType switch
            {
                UpgradeNodeType.NodeSkill => UIUpgradeNodeSkillPool.Instance.GetVfxActivate(transform, true),
                UpgradeNodeType.NodeEffect => UIUpgradeNodeEffectPool.Instance.GetVfxActivate(transform, true),
                UpgradeNodeType.NodeStat => UIUpgradeNodeStatPool.Instance.GetVfxActivate(transform, true),
                UpgradeNodeType.NodeStat2 => UIUpgradeNodeStat2Pool.Instance.GetVfxActivate(transform, true),
                _ => null
            };
        }
        
        public UIParticle GetVfxActivateMax()
        {
            return nodeType switch
            {
                UpgradeNodeType.NodeSkill => UIUpgradeNodeSkillPool.Instance.GetVfxActivateMax(transform, true),
                UpgradeNodeType.NodeEffect => UIUpgradeNodeEffectPool.Instance.GetVfxActivateMax(transform, true),
                UpgradeNodeType.NodeStat => UIUpgradeNodeStatPool.Instance.GetVfxActivateMax(transform, true),
                UpgradeNodeType.NodeStat2 => UIUpgradeNodeStat2Pool.Instance.GetVfxActivateMax(transform, true),
                _ => null
            };
        }

        public void ReleaseVfxUnlock(UIParticle vfx)
        {
            switch (nodeType)
            {
                case UpgradeNodeType.NodeSkill:
                    UIUpgradeNodeSkillPool.Instance.ReleaseVfxUnlock(vfx);
                    break;
                case UpgradeNodeType.NodeEffect:
                    UIUpgradeNodeEffectPool.Instance.ReleaseVfxUnlock(vfx);
                    break;
                case UpgradeNodeType.NodeStat:
                    UIUpgradeNodeStatPool.Instance.ReleaseVfxUnlock(vfx);
                    break;
                case UpgradeNodeType.NodeStat2:
                    UIUpgradeNodeStat2Pool.Instance.ReleaseVfxUnlock(vfx);
                    break;
            }
        }
        
        public void ReleaseVfxActivate(UIParticle vfx)
        {
            switch (nodeType)
            {
                case UpgradeNodeType.NodeSkill:
                    UIUpgradeNodeSkillPool.Instance.ReleaseVfxActivate(vfx);
                    break;
                case UpgradeNodeType.NodeEffect:
                    UIUpgradeNodeEffectPool.Instance.ReleaseVfxActivate(vfx);
                    break;
                case UpgradeNodeType.NodeStat:
                    UIUpgradeNodeStatPool.Instance.ReleaseVfxActivate(vfx);
                    break;
                case UpgradeNodeType.NodeStat2:
                    UIUpgradeNodeStat2Pool.Instance.ReleaseVfxActivate(vfx);
                    break;
            }
        }
        
        public void ReleaseVfxActivateMax(UIParticle vfx)
        {
            switch (nodeType)
            {
                case UpgradeNodeType.NodeSkill:
                    UIUpgradeNodeSkillPool.Instance.ReleaseVfxActivateMax(vfx);
                    break;
                case UpgradeNodeType.NodeEffect:
                    UIUpgradeNodeEffectPool.Instance.ReleaseVfxActivateMax(vfx);
                    break;
                case UpgradeNodeType.NodeStat:
                    UIUpgradeNodeStatPool.Instance.ReleaseVfxActivateMax(vfx);
                    break;
                case UpgradeNodeType.NodeStat2:
                    UIUpgradeNodeStat2Pool.Instance.ReleaseVfxActivateMax(vfx);
                    break;
            }
        }

        #endregion

        #region Highlight

        [Space] [Header("Highlight")] 
        public Image imgHighlight;
        
        public void Highlight(bool highlight)
        {
            if (highlight)
            {
                if (GameConst.HideLockedNode) return;

                groupNode.alpha = 1f;
                imgHighlight.gameObject.SetActive(true);
            }
            else
            {
                groupNode.alpha = 0.02f;
                imgHighlight.gameObject.SetActive(false);
            }
        }

        public void HideHighlight()
        {
            imgHighlight.gameObject.SetActive(false);
            if (GameConst.HideLockedNode) return;

            groupNode.alpha = 1f;
        }

        #endregion
        
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