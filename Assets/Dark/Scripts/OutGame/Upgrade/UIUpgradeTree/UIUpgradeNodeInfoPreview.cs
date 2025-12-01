using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Core;
using Dark.Scripts.InGame.Upgrade;
using Dark.Scripts.Utils.Camera;
using Data;
using DG.Tweening;
using Economic;
using InGame;
using InGame.ChargeConfig;
using InGame.ConfigManager;
using InGame.Upgrade;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        [Space] [Header("Requirement")] 
        [SerializeField] private RequirementInfo infoReqVestige;
        [SerializeField] private RequirementInfo infoReqEchoes;
        [SerializeField] private RequirementInfo infoReqSigils;
        [SerializeField] private CanvasGroup groupStillAvailable;
        [SerializeField] private GameObject groupMax;
        [SerializeField] private Color colorEnoughResource;
        [SerializeField] private Color colorNotEnoughResource;

        [Space] [Header("Base stats config")] 
        [SerializeField] private PlayerStats playerStatsConfig;
        [SerializeField] private MoveTowersConfig teleConfig;
        [SerializeField] private MoveTowersConfig flashConfig;
        [SerializeField] private MoveTowersConfig dashConfig;
        public UpgradeBonusInfo bonusInfo = new UpgradeBonusInfo();
        
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
            LevelUtility.BonusInfo = bonusInfo;
            LevelUtility.PlayerStats = playerStatsConfig;
            LevelUtility.CurrentSkill = ClassConfigManifest.GetConfig(PlayerDataManager.Instance.Data.characterClass);
            LevelUtility.ChargeConfigMap = new Dictionary<ChargeType, PlayerChargeConfig>()
            {
                { ChargeType.Bullet, PlayerChargeManifest.Get(ChargeType.Bullet) },
                { ChargeType.Size, PlayerChargeManifest.Get(ChargeType.Size) }
            };
            LevelUtility.DashConfig = dashConfig;
            LevelUtility.FlashConfig = flashConfig;
            LevelUtility.TeleConfig = teleConfig; 
        }

        public void Setup(UpgradeNodeConfig config, bool forceUpdate)
        {
            if (CanAutoShowHide == false && forceUpdate == false) return;
            
            cacheConfig = config;
            cacheData = UpgradeManager.Instance.GetData(config.nodeId);
            UpdateUI();
        }

        public void UpdateUI()
        {
            if (cacheConfig == null) return;
            txtNodeName.SetText(cacheConfig.nodeName);
            txtNodeLore.SetText(cacheConfig.description);
            txtNodeLevel.SetText($"{cacheData?.level ?? 0}/{cacheConfig.MaxLevel}");

            var descriptionStr = "";
            var descriptions = cacheConfig.description.Split("\n");
            for (var i = 0; i < cacheConfig.nodeLogic.Length; i++)
            {
                if (i < descriptions.Length)
                {
                    descriptions[i] = descriptions[i].Replace("[X]",
                        cacheConfig.nodeLogic[i].GetDisplayValue(cacheData?.level ?? 0));
                    descriptionStr += descriptions[i];
                    if (i < descriptions.Length - 1) descriptionStr += "\n";
                }
            }
            txtNodeBonus.SetText(descriptionStr);
            txtNodeBonus.gameObject.SetActive(true);

            var bonusBeforeStr = "";
            var bonusAfterStr = "";
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
            }
            else
            {
                groupStillAvailable.alpha = 1f;
                groupStillAvailable.gameObject.SetActive(true);
                groupMax.SetActive(false);
                
                var costVestige = 0;
                var costEchoes = 0;
                var costSigils = 0;
                foreach (var req in cacheConfig.costInfo)
                {
                    if (req.costType == WealthType.Vestige) 
                        costVestige = UpgradeRequirementConfig.Instance.GetRequirement(WealthType.Vestige, UpgradeManager.Instance.GetRequirementIndex(WealthType.Vestige));
                    else if (req.costType == WealthType.Echoes) 
                        costEchoes = UpgradeRequirementConfig.Instance.GetRequirement(WealthType.Echoes, UpgradeManager.Instance.GetRequirementIndex(WealthType.Echoes));
                    else if (req.costType == WealthType.Sigils) 
                        costSigils = UpgradeRequirementConfig.Instance.GetRequirement(WealthType.Sigils, UpgradeManager.Instance.GetRequirementIndex(WealthType.Sigils));
                }

                var canSpend = WealthManager.Instance.CanSpend(WealthType.Vestige, costVestige);
                infoReqVestige.txtReq.SetText(costVestige.ToString()); 
                infoReqVestige.txtReq.color = canSpend ? colorEnoughResource : colorNotEnoughResource;
                infoReqVestige.imgIconNotEnough.gameObject.SetActive(!canSpend);
                canSpend = WealthManager.Instance.CanSpend(WealthType.Echoes, costEchoes);
                infoReqEchoes.txtReq.SetText(costEchoes.ToString());
                infoReqEchoes.txtReq.color = canSpend ? colorEnoughResource : colorNotEnoughResource;
                infoReqEchoes.imgIconNotEnough.gameObject.SetActive(!canSpend);
                canSpend = WealthManager.Instance.CanSpend(WealthType.Sigils, costSigils);
                infoReqSigils.txtReq.SetText(costSigils.ToString());
                infoReqSigils.txtReq.color = canSpend ? colorEnoughResource : colorNotEnoughResource;
                infoReqSigils.imgIconNotEnough.gameObject.SetActive(!canSpend);
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
            DoShow().OnComplete(() => onShow?.Invoke());
        }

        public void Hide(bool forceHide)
        {
            if (CanAutoShowHide == false && forceHide == false) return;
            isVisible = false;
            DoHide();
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
    }
}