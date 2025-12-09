using System;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIExtensions;
using Dark.Scripts.Audio;
using Dark.Scripts.AudioV2;
using Dark.Scripts.Utils;
using DG.Tweening;
using InGame;
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
        [SerializeField] protected UIUpgradeNodeSpawnAnimation spawnAnimation;

        [SerializeField] protected CanvasGroup groupNode;
        [SerializeField] protected Image nodeVisual;
        [SerializeField] protected Image nodeLockVisual;

        [SerializeField] protected Transform imgBorder;
        [SerializeField] protected GameObject imgActivatedGlow;
        [SerializeField] protected GameObject imgActivatedMaxGlow;
        [SerializeField] protected Transform rectActivatedMaxOutline;
        [SerializeField] protected GameObject imgAvailable;
        [SerializeField] protected Image imgLock;
        [SerializeField] protected Image imgIconLock;
        [SerializeField] protected UIParticle vfxUnlock;
        [SerializeField] protected UIParticle vfxActivate;
        [SerializeField] protected UIParticle vfxActivateMax;
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
            defaultScale = transform.localScale;
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
                    imgAvailable.SetActive(true);
                    imgLock.gameObject.SetActive(false);
                    imgActivatedGlow.SetActive(false);
                    imgActivatedMaxGlow.SetActive(false);
                    rectActivatedMaxOutline.gameObject.SetActive(false);
                    imgBorder.gameObject.SetActive(true);
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
                    imgAvailable.SetActive(false);
                    imgLock.gameObject.SetActive(true);
                    imgActivatedGlow.SetActive(false);
                    imgActivatedMaxGlow.SetActive(false);
                    rectActivatedMaxOutline.gameObject.SetActive(false);
                    imgBorder.gameObject.SetActive(true);
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
                    imgAvailable.SetActive(false);
                    imgLock.gameObject.SetActive(true);
                    imgActivatedGlow.SetActive(false);
                    imgActivatedMaxGlow.SetActive(false);
                    rectActivatedMaxOutline.gameObject.SetActive(false);
                    imgBorder.gameObject.SetActive(true);
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
                    imgAvailable.SetActive(true);
                    imgLock.gameObject.SetActive(false);
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
                DOTween.Kill(transform);
                sfxHover.Play();
                transform.localRotation = Quaternion.identity;
                transform.localScale = defaultScale;
                transform.DOPunchRotation(new Vector3(0f, 0f, 10f), 0.3f, 20, 0.1f).SetTarget(transform);
                transform.DOScale(new Vector3(0.2f, 0.2f, 0), 0.2f).SetRelative().SetEase(Ease.OutQuad);
                UIUpgradeNodeInfoPreview.Instance.Setup(config, false);
                UIUpgradeNodeInfoPreview.Instance.Show(transform.position, new Vector2(hoverField.nodeRepresentableRect.sizeDelta.x / 2, 0f), false, () => hoverField.interactable = true);
            };
            hoverField.onHoverExit = () =>
            {
                if (GameConst.HideLockedNode && CurrentState == UIUpgradeNodeState.Locked)
                    return;
                hoverField.interactable = false;
                transform.DOScale(defaultScale, 0.2f).SetEase(Ease.InQuad);
                UIUpgradeNodeInfoPreview.Instance.Hide(false);
            };
            hoverField.onPointerClick = () =>
            {
                if (GameConst.HideLockedNode && CurrentState == UIUpgradeNodeState.Locked)
                    return;
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
                    config.ActivateLevel(UpgradeManager.Instance.GetData(config.nodeId).level, ref UIUpgradeNodeInfoPreview.Instance.bonusInfo);
                    // UIUpgradeNodeInfoPreview.Instance.Setup(config, true);
                    // UIUpgradeNodeInfoPreview.Instance.Show(transform.position, new Vector2(hoverField.nodeRepresentableRect.sizeDelta.x / 2, 0f), true, () => hoverField.interactable = true);
                    UIUpgradeNodeInfoPreview.Instance.Hide(true);
                    UIUpgradeNodeInfoPreview.Instance.CanAutoShowHide = false;
                    this.DelayCall(0.5f, () =>
                    {
                        UIUpgradeNodeInfoPreview.Instance.Setup(config, true);
                        UIUpgradeNodeInfoPreview.Instance.Show(transform.position, new Vector2(hoverField.nodeRepresentableRect.sizeDelta.x / 2, 0f), true, () =>
                        {
                            hoverField.interactable = true;
                            UIUpgradeNodeInfoPreview.Instance.CanAutoShowHide = true;
                        });
                        // UIUpgradeNodeInfoPreview.Instance.ShowImmediately(transform.position,
                        //     new Vector2(hoverField.nodeRepresentableRect.sizeDelta.x / 2, 0f), true,
                        //     () => UIUpgradeNodeInfoPreview.Instance.CanAutoShowHide = true);
                    });
                    treeRef.LastUpgradeNodeId = config.nodeId;
                    treeRef.InvokeNodeUpgraded(this);
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
            treeRef.UpdateChildren(config.nodeId, UpgradeManager.Instance.GetData(config.nodeId).level == 1);
        }

        public Tween DoUnlockVfx(int fromId)
        {
            var seq = DOTween.Sequence(this);
            if (preRequires != null)
            {
                foreach (var lineInfo in preRequires)
                {
                    if (lineInfo.preRequireId != fromId) continue;
                    seq.AppendCallback(() =>
                        {
                            DOVirtual.DelayedCall(lineInfo.line.activateDuration - 0.1f, () =>
                            {
                                imgAvailable.gameObject.SetActive(true);
                                vfxUnlock?.Play();
                            });
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
        
        public Tween DoSpawn()
        {
            return spawnAnimation.SpawnLogic.DoSpawn();
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