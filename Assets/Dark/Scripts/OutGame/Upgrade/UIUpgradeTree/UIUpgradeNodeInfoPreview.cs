using System;
using System.Linq;
using System.Text.RegularExpressions;
using Coffee.UIExtensions;
using Core;
using InGame.Upgrade.DynamicCost;
using Data;
using DG.Tweening;
using Economic;
using InGame;
using InGame.AttackNormalConfig;
using InGame.ChargeConfig;
using InGame.ConfigManager;
using InGame.CounterConfig;
using InGame.Upgrade;
using OutGame.Upgrade.Tooltip;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeNodeInfoPreview : SerializedMonoSingleton<UIUpgradeNodeInfoPreview>
    {
        [OdinSerialize, NonSerialized] private INodePreviewPositionLogic nodePreviewPositionLogic;
        [SerializeField] private Vector2 rectInfoFramePadding;
        [SerializeField] private RectTransform rectInfoFrame;
        [SerializeField] private RectTransform rectInfoFrameContent;
        [SerializeField] private UIInfoPreviewContentFitter contentFitter;
        [SerializeField] private TextMeshProUGUI txtNodeName;
        [SerializeField] private TextMeshProUGUI txtNodeLore;
        [SerializeField] private TextMeshProUGUI txtNodeLevel;
        [SerializeField] private TextMeshProUGUI txtNodeBonus;
        [SerializeField] private TextMeshProUGUI txtNodeBonusBefore;
        [SerializeField] private TextMeshProUGUI txtNodeBonusAfter;
        [SerializeField] private RectTransform rectInfoBonusChanged;
        [SerializeField] private UITooltip tooltip;
        [SerializeField] private UITooltipSkillVideo tooltipSkillVideo;
        [SerializeField] private Image imgLevelProgress;
        [SerializeField] private UIParticle vfxBackgroundNotUpgrade;
        [SerializeField] private UIParticle vfxBackgroundUpgraded;
        [SerializeField] private UIParticle vfxBackgroundMax;
        [SerializeField] private List<UIParticle> vfxClaimNotEnoughs;
        [SerializeField] private List<UIParticle> vfxClaimEnoughs;
        [SerializeField] private List<UIParticle> vfxClaimMaxs;



        [Space] [Header("Requirement")] 
        [SerializeField] private RequirementInfo infoReqVestige;
        [SerializeField] private RequirementInfo infoReqEchoes;
        [SerializeField] private RequirementInfo infoReqSigils;
        [SerializeField] private CanvasGroup groupStillAvailable;
        [SerializeField] private GameObject groupMax;
        [SerializeField] private Color colorEnoughResource;
        [SerializeField] private Color colorNotEnoughResource;
        [SerializeField] private Image imgNotEnoughResource;
        [SerializeField] private Image imgEnoughResource;
        [SerializeField] private Image imgMax;

        [Space] [Header("Base stats config")] 
        [SerializeField] private PlayerStats playerStatsConfig;
        [SerializeField] private MoveTowersConfig teleConfig;
        [SerializeField] private MoveTowersConfig flashConfig;
        [SerializeField] private MoveTowersConfig dashConfig;
        public UpgradeBonusInfoV2 bonusInfo = new UpgradeBonusInfoV2();
        
        [Serializable]
        public class RequirementInfo
        {
            public GameObject groupReq;
            public TextMeshProUGUI txtReq;
            public Image imgIconNotEnough;
        }

        public bool CanAutoShowHide { get; set; } = true;
        private UpgradeNodeData cacheData;
        private UpgradeNodeConfig cacheConfig;
        private bool isVisible;
        private Vector2 mousePos = Vector2.zero;
        private Vector2 cacheHoverNodePosition = new Vector2(0, 0);
        private Vector2 cacheHoverNodePadding = new Vector2(0, 0);

        private void Start()
        {
            UpgradeManager.Instance.ActivateTree(ref bonusInfo);
            LevelUtilityV2.BonusInfo = bonusInfo;
            LevelUtilityV2.StatsBase = playerStatsConfig;
            LevelUtilityV2.StatsNormalAttack = ClassConfigManifest.GetConfig(PlayerDataManager.Instance.Data.characterClass);
            LevelUtilityV2.StatsNormalPiercing = PlayerSkillNormalManifest.Get(NormalType.Piercing);
            LevelUtilityV2.StatsNormalBullet = PlayerSkillNormalManifest.Get(NormalType.Bullet);
            LevelUtilityV2.StatsChargeBullet = PlayerChargeManifest.Get(ChargeType.Bullet);
            LevelUtilityV2.StatsChargeSize = PlayerChargeManifest.Get(ChargeType.Size);
            LevelUtilityV2.StatsDash = dashConfig;
            LevelUtilityV2.StatsFlash = flashConfig;
            LevelUtilityV2.StatsTele = teleConfig; 
            LevelUtilityV2.StatsCounterPiercing = TowerCounterManifest.Get(NodeTowerCounter.CounterType.Pierce);
            LevelUtilityV2.StatsCounterSlash = TowerCounterManifest.Get(NodeTowerCounter.CounterType.Slash);
        }

        public void Setup(UIUpgradeNode node, UpgradeNodeConfig config, bool forceUpdate)
        {
            if (CanAutoShowHide == false && forceUpdate == false) return;
            
            cacheConfig = config;
            cacheData = UpgradeManager.Instance.GetData(config.nodeId);
            var isNodeLocked = node.preRequires is { Count: > 0 } && node.preRequires.All((preRequire) =>
                preRequire.node.CurrentState != UIUpgradeNodeState.Activated);
            tooltipSkillVideo.gameObject.gameObject.SetActive(false);
            tooltip.gameObject.SetActive(false);
            UpdateUI(isNodeLocked);
        }

        public void UpdateUI(bool isLocked)
        {
            if (cacheConfig == null) return;

            var descriptionStr = "";
            var descriptions = cacheConfig.description.Split("\n");
            if (cacheConfig.nodeLogic != null)
            {
                for (var i = 0; i < cacheConfig.nodeLogic.Length; i++)
                {
                    if (i < descriptions.Length)
                    {
                        if (cacheConfig.nodeLogic[i] is INodeDynamicBonusValueV2 { IsDynamic: true } dynamicLogic)
                        {
                            dynamicLogic.OverrideBonusValue(cacheConfig.groupId.Min((info) => UpgradeManager.Instance.GetGroupUnlockOrder(info.groupId, false)));
                        }
                        descriptions[i] = descriptions[i].Replace("[X]",
                            cacheConfig.nodeLogic[i].GetDisplayValue(cacheData?.level ?? 0));
                        descriptionStr += descriptions[i];
                        if (i < descriptions.Length - 1) descriptionStr += "\n";
                    }
                }
            }
            txtNodeBonus.SetText(descriptionStr);
            txtNodeBonus.gameObject.SetActive(true);
            
            txtNodeName.SetText(cacheConfig.nodeName);
            txtNodeLore.SetText(cacheConfig.description);
            txtNodeLevel.SetText($"{cacheData?.level ?? 0}/{cacheConfig.MaxLevel}");
            DOTween.Kill(imgLevelProgress);
            imgLevelProgress.DOFillAmount((cacheData?.level ?? 0f) / cacheConfig.MaxLevel, 0.3f).SetEase(Ease.OutQuad)
                .SetTarget(imgLevelProgress);
            if ((cacheData?.level ?? 0) <= 0) SetVfxBackgroundNotUpgraded();
            else if ((cacheData?.level ?? 0) < cacheConfig.MaxLevel) SetVfxBackgroundUpgraded();
            else SetVfxBackgroundMax();

            var bonusBeforeStr = "";
            var bonusAfterStr = "";
            if (cacheConfig.nodeLogic != null)
            {
                for (var i = 0; i < cacheConfig.nodeLogic.Length; i++)
                {
                    var bonusChanged = cacheConfig.nodeLogic[i].GetBeforeAfterValueTotalStat(cacheData?.level + 1 ?? 1, ref bonusInfo);
                    if (string.IsNullOrEmpty(bonusChanged.Item1) && string.IsNullOrEmpty(bonusChanged.Item2))
                        continue;
                    bonusAfterStr += bonusChanged.Item2;
                    if (cacheData != null && cacheData.level >= cacheConfig.MaxLevel)
                        bonusBeforeStr += bonusChanged.Item2;    
                    else bonusBeforeStr += bonusChanged.Item1;
                    if (i < cacheConfig.nodeLogic.Length - 1)
                    {
                        bonusBeforeStr += "\n";
                        bonusAfterStr += "\n";
                    }
                }
            }
            txtNodeBonusBefore.SetText(bonusBeforeStr);
            txtNodeBonusAfter.SetText(bonusAfterStr);
            if (string.IsNullOrEmpty(bonusBeforeStr) && string.IsNullOrEmpty(bonusAfterStr))
                rectInfoBonusChanged.gameObject.SetActive(false);
            else
                rectInfoBonusChanged.gameObject.SetActive(true);

            // Setup requirement
            if (cacheData != null && cacheData.level >= cacheConfig.MaxLevel)
            {
                groupStillAvailable.gameObject.SetActive(false);
                groupMax.SetActive(true);
                SetMax();
            }
            else
            {
                groupMax.SetActive(false);
                if (isLocked)
                {
                    groupStillAvailable.gameObject.SetActive(false);
                }
                else
                {
                    groupStillAvailable.alpha = 1f;
                    groupStillAvailable.gameObject.SetActive(true);
                }

                var (costVestige, costEchoes, costSigils) = GetDisplayCost();

                SetEnoughResource();
                var canSpend = WealthManager.Instance.CanSpend(WealthType.Vestige, costVestige);
                infoReqVestige.txtReq.SetText(costVestige.ToString()); 
                infoReqVestige.txtReq.color = canSpend ? colorEnoughResource : colorNotEnoughResource;
                infoReqVestige.imgIconNotEnough.gameObject.SetActive(!canSpend);
                if (!canSpend) SetNotEnoughResource();
                canSpend = WealthManager.Instance.CanSpend(WealthType.Echoes, costEchoes);
                infoReqEchoes.txtReq.SetText(costEchoes.ToString());
                infoReqEchoes.txtReq.color = canSpend ? colorEnoughResource : colorNotEnoughResource;
                infoReqEchoes.imgIconNotEnough.gameObject.SetActive(!canSpend);
                if (!canSpend) SetNotEnoughResource();
                canSpend = WealthManager.Instance.CanSpend(WealthType.Sigils, costSigils);
                infoReqSigils.txtReq.SetText(costSigils.ToString());
                infoReqSigils.txtReq.color = canSpend ? colorEnoughResource : colorNotEnoughResource;
                infoReqSigils.imgIconNotEnough.gameObject.SetActive(!canSpend);
                if (!canSpend) SetNotEnoughResource();
                infoReqVestige.groupReq.SetActive(costVestige > 0);
                infoReqEchoes.groupReq.SetActive(costEchoes > 0);
                infoReqSigils.groupReq.SetActive(costSigils > 0);
            }
            
            rectInfoFrame.sizeDelta = contentFitter.GetSize();
        }

        private void Update()
        {
            if (!isVisible) return;

            mousePos.x = Input.mousePosition.x;
            mousePos.y = Input.mousePosition.y;
            
            nodePreviewPositionLogic.UpdatePosition(
                ref mousePos,
                ref cacheHoverNodePosition,
                ref cacheHoverNodePadding,
                ref rectInfoFramePadding,
                ref rectInfoFrame);
        }

        public void Show(Vector2 position, Vector2 padding, bool forceShow, Action onShow)
        {
            cacheHoverNodePosition.x = position.x;
            cacheHoverNodePosition.y = position.y;
            cacheHoverNodePadding.x = padding.x;
            cacheHoverNodePadding.y = padding.y;
            if (CanAutoShowHide == false && forceShow == false) return;
            isVisible = true;
            DoShow().OnComplete(() =>
            {
                if (!tooltipSkillVideo.Show(cacheConfig, rectInfoFrameContent, new Vector2(10f, 0f)))
                {
                    tooltip.Show(cacheConfig.description, rectInfoFrameContent, new Vector2(10f, 0f));
                }
                onShow?.Invoke();
            });
        }

        public void ShowImmediately(Vector2 position, Vector2 padding, bool forceShow, Action onShow)
        {
            cacheHoverNodePosition.x = position.x;
            cacheHoverNodePosition.y = position.y;
            cacheHoverNodePadding.x = padding.x;
            cacheHoverNodePadding.y = padding.y;
            if (CanAutoShowHide == false && forceShow == false) return;
            isVisible = true;
            rectInfoFrame.localScale = Vector3.one;
            rectInfoFrame.gameObject.SetActive(true);
            onShow?.Invoke();
        }

        public void Hide(bool forceHide)
        {
            if (CanAutoShowHide == false && forceHide == false) return;
            isVisible = false;
            DoHide();
        }

        public void HideImmediately(bool forceHide)
        {
            if (CanAutoShowHide == false && forceHide == false) return;
            isVisible = false;
            rectInfoFrame.localScale = Vector3.zero;
            rectInfoFrame.gameObject.SetActive(false);
        }

        public void Shake()
        {
            DOTween.Kill(rectInfoFrame);
            DOTween.Sequence(rectInfoFrame)
                .Append(rectInfoFrameContent.DOShakePosition(0.3f, new Vector3(0f, 5f, 0f), vibrato: 30, fadeOut: false, randomnessMode: ShakeRandomnessMode.Harmonic));
        }

        private Tween DoShow()
        {
            DOTween.Kill(rectInfoFrame);

            rectInfoFrame.localScale = 0.8f * Vector3.one;
            rectInfoFrame.gameObject.SetActive(true);
            GetVfxBackground().Play();

            return DOTween.Sequence(rectInfoFrame)
                .Append(rectInfoFrame.DOScale(1f, 0.2f).SetEase(Ease.OutBack));
        }

        private Tween DoHide()
        {
            DOTween.Kill(rectInfoFrame);

            return DOTween.Sequence(rectInfoFrame)
                .Append(rectInfoFrame.DOScale(0f, 0.2f).SetEase(Ease.OutQuad))
                .AppendCallback(() =>
                {
                    GetVfxBackground().Stop();
                    rectInfoFrame.gameObject.SetActive(false);
                });
        }

        private string ExtractValueString(string input)
        {
            const string pattern = @"[+-]?\[X\]%?";
            var match = Regex.Match(input, pattern);

            if (match.Success)
            {
                return match.Value;
            }
            
            return string.Empty;
        }

        #region Visual
        
        private void SetEnoughResource()
        {
            imgEnoughResource.gameObject.SetActive(true);
            imgNotEnoughResource.gameObject.SetActive(false);
            imgMax.gameObject.SetActive(false);
        }

        private void SetNotEnoughResource()
        {
            imgEnoughResource.gameObject.SetActive(false);
            imgNotEnoughResource.gameObject.SetActive(true);
            imgMax.gameObject.SetActive(false);
        }

        private void SetMax()
        {
            imgEnoughResource.gameObject.SetActive(false);
            imgNotEnoughResource.gameObject.SetActive(false);
            imgMax.gameObject.SetActive(true);
        }

        private void SetVfxBackgroundNotUpgraded()
        {
            vfxBackgroundNotUpgrade.gameObject.SetActive(true);
            vfxBackgroundUpgraded.gameObject.SetActive(false);
            vfxBackgroundMax.gameObject.SetActive(false);
        }

        private void SetVfxBackgroundUpgraded()
        {
            vfxBackgroundNotUpgrade.gameObject.SetActive(false);
            vfxBackgroundUpgraded.gameObject.SetActive(true);
            vfxBackgroundMax.gameObject.SetActive(false);
        }

        private void SetVfxBackgroundMax()
        {
            vfxBackgroundNotUpgrade.gameObject.SetActive(false);
            vfxBackgroundUpgraded.gameObject.SetActive(false);
            vfxBackgroundMax.gameObject.SetActive(true);
        }
        
        private UIParticle GetVfxBackground()
        {
            if (vfxBackgroundNotUpgrade.gameObject.activeSelf) return vfxBackgroundNotUpgrade;
            if (vfxBackgroundUpgraded.gameObject.activeSelf) return vfxBackgroundUpgraded;
            if (vfxBackgroundMax.gameObject.activeSelf) return vfxBackgroundMax;
            return vfxBackgroundNotUpgrade;
        }

        // (vestige, echoes, sigils)
        public (int, int, int) GetDisplayCost()
        {
            if (!cacheConfig || cacheConfig.costInfo == null) return (0, 0, 0);
            
            var costVestige = 0;
            var costEchoes = 0;
            var costSigils = 0;
            foreach (var req in cacheConfig.costInfo)
            {
                var unlockLevel = cacheData?.level ?? 0;
                if (req.costType == WealthType.Vestige)
                {
                    if (cacheConfig.dynamicVestige)
                    {
                        if (cacheConfig.MaxLevel == 1)
                            costVestige =
                                DynamicVestigeConfig.Instance.GetCost1Stage(
                                    cacheConfig.groupId.Min((info) => UpgradeManager.Instance.GetGroupUnlockOrder(info.groupId, false)));
                        else
                        {
                            var unlockCost = DynamicVestigeConfig.Instance.GetCost5Stage(
                                cacheConfig.groupId.Min((info) => UpgradeManager.Instance.GetGroupUnlockOrder(info.groupId, false)));
                            unlockLevel = Math.Clamp(unlockLevel, 0, unlockCost.Length - 1);
                            costVestige = unlockCost[unlockLevel];
                        }
                    }
                    else
                    {
                        unlockLevel = Math.Clamp(unlockLevel, 0, req.costValue.Length - 1);
                        costVestige = req.costValue[unlockLevel];
                    }

                    costVestige = Mathf.RoundToInt(costVestige * cacheConfig.vestigeCostRatio);
                }
                else if (req.costType == WealthType.Echoes)
                {
                    if (cacheConfig.dynamicEchoes)
                    {
                        if (cacheConfig.MaxLevel == 1)
                            costEchoes =
                                DynamicVestigeConfig.Instance.GetCost1Echoes(
                                    cacheConfig.groupId.Min((info) => UpgradeManager.Instance.GetGroupUnlockOrder(info.groupId, false)));
                        else
                        {
                            var unlockCost = DynamicVestigeConfig.Instance.GetCost5Echoes(
                                cacheConfig.groupId.Min((info) => UpgradeManager.Instance.GetGroupUnlockOrder(info.groupId, false)));
                            unlockLevel = Math.Clamp(unlockLevel, 0, unlockCost.Length - 1);
                            costEchoes = unlockCost[unlockLevel];
                        }
                    }
                    else
                    {
                        unlockLevel = Math.Clamp(unlockLevel, 0, req.costValue.Length - 1);
                        costEchoes = req.costValue[unlockLevel];
                    }
                }
                else if (req.costType == WealthType.Sigils)
                {
                    unlockLevel = Math.Clamp(unlockLevel, 0, req.costValue.Length - 1);
                    costSigils = req.costValue[unlockLevel];
                }
            }
            
            return (costVestige, costEchoes, costSigils);
        }

        #endregion
    }
}