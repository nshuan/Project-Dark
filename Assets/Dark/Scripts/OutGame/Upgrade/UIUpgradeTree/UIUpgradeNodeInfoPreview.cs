using System;
using System.Linq;
using Core;
using Dark.Scripts.InGame.Upgrade;
using Dark.Scripts.Utils.Camera;
using DG.Tweening;
using Economic;
using InGame.Upgrade;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeNodeInfoPreview : MonoSingleton<UIUpgradeNodeInfoPreview>
    {
        [SerializeField] private Vector2 rectInfoFramePadding;
        [SerializeField] private RectTransform rectInfoFrame;
        [SerializeField] private RectTransform rectInfoFrameContent;
        [SerializeField] private UIInfoPreviewContentFitter contentFitter;
        [SerializeField] private TextMeshProUGUI txtNodeName;
        [SerializeField] private TextMeshProUGUI txtNodeLore;
        [SerializeField] private TextMeshProUGUI txtNodeLevel;
        [SerializeField] private TextMeshProUGUI txtNodeBonus;

        [Space] [Header("Requirement")] 
        [SerializeField] private RequirementInfo infoReqVestige;
        [SerializeField] private RequirementInfo infoReqEchoes;
        [SerializeField] private RequirementInfo infoReqSigils;
        [SerializeField] private CanvasGroup groupStillAvailable;
        [SerializeField] private GameObject groupMax;
        [SerializeField] private Color colorEnoughResource;
        [SerializeField] private Color colorNotEnoughResource;

        [Serializable]
        public class RequirementInfo
        {
            public GameObject groupReq;
            public TextMeshProUGUI txtReq;
        }

        public bool CanAutoShowHide { get; set; } = true;
        private UpgradeNodeData cacheData;
        private UpgradeNodeConfig cacheConfig;
        private bool isVisible;
        private Vector2 mousePos = Vector2.zero;

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
            txtNodeLevel.SetText($"{cacheData?.level ?? 0}/{cacheConfig.levelNum}");

            var descriptionStr = "";
            var descriptions = cacheConfig.description.Split("\n");
            for (var i = 0; i < cacheConfig.nodeLogic.Length; i++)
            {
                if (i < descriptions.Length && descriptions[i].Contains("[X]"))
                {
                    descriptions[i] = descriptions[i].Replace("[X]",
                        cacheConfig.nodeLogic[i].GetDisplayValue(cacheData?.level ?? 0));
                    descriptionStr += descriptions[i] + "\n";
                }
                else
                    descriptionStr += cacheConfig.nodeLogic[i].GetDisplayValue(cacheData?.level ?? 0) + "\n";

            }
            txtNodeBonus.SetText(descriptionStr);
            txtNodeBonus.gameObject.SetActive(true);

            // Setup requirement
            if (cacheData != null && cacheData.level >= cacheConfig.levelNum)
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
                infoReqVestige.txtReq.SetText(costVestige.ToString()); 
                infoReqVestige.txtReq.color = WealthManager.Instance.CanSpend(WealthType.Vestige, costVestige) ? colorEnoughResource : colorNotEnoughResource;
                infoReqEchoes.txtReq.SetText(costEchoes.ToString());
                infoReqEchoes.txtReq.color = WealthManager.Instance.CanSpend(WealthType.Echoes, costEchoes) ? colorEnoughResource : colorNotEnoughResource;
                infoReqSigils.txtReq.SetText(costSigils.ToString());
                infoReqSigils.txtReq.color = WealthManager.Instance.CanSpend(WealthType.Sigils, costSigils) ? colorEnoughResource : colorNotEnoughResource;
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
            // Check if the panel is outside the screen
            var framePos = mousePos;
            var framePivot = new Vector2(0f, 0.5f);
            if (mousePos.x + rectInfoFrame.sizeDelta.x - rectInfoFramePadding.x > SafeScaler.ScreenWidth)
            {
                framePivot.x = 1f;
            }
            else
            {
                framePivot.x = 0f;
            }

            if (mousePos.y + rectInfoFrame.sizeDelta.y / 2 - rectInfoFramePadding.y > SafeScaler.ScreenHeight)
                framePivot.y = 1f;
            else if (mousePos.y - rectInfoFrame.sizeDelta.y / 2 + rectInfoFramePadding.y < 0)
                framePivot.y = 0f;
            else
                framePivot.y = 1f;

            rectInfoFrame.position = framePos;
            rectInfoFrame.pivot = framePivot;
        }

        public void Show(Vector2 position, Vector2 padding, bool forceShow, Action onShow)
        {
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
                .Append(rectInfoFrameContent.DOShakePosition(0.3f, new Vector3(0f, 8f, 0f), vibrato: 30, fadeOut: false, randomnessMode: ShakeRandomnessMode.Harmonic));
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
                .Append(rectInfoFrame.DOScale(0f, 0.2f).SetEase(Ease.InBack))
                .AppendCallback(() =>
                {
                    rectInfoFrame.gameObject.SetActive(false);
                });
        }
    }
}