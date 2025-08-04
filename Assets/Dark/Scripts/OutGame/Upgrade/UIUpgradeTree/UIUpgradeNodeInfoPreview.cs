using System;
using System.Linq;
using Core;
using InGame.Upgrade;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeNodeInfoPreview : MonoSingleton<UIUpgradeNodeInfoPreview>
    {
        [SerializeField] private RectTransform rectInfoFrame;
        [SerializeField] private TextMeshProUGUI txtNodeName;
        [SerializeField] private TextMeshProUGUI txtNodeLore;
        [SerializeField] private TextMeshProUGUI txtNodeLevel;
        [SerializeField] private TextMeshProUGUI txtNodePrice;
        [SerializeField] private TextMeshProUGUI[] txtNodeBonus;
        [SerializeField] private Button btnUpgrade;

        public bool CanAutoShowHide { get; set; } = true;
        private UpgradeNodeData cacheData;
        private UpgradeNodeConfig cacheConfig;
        
        private Action OnUpgradeSuccessCallback { get; set; }
        private Action OnUpgradeFailedCallback { get; set; }

        private void Start()
        {
            btnUpgrade.onClick.RemoveAllListeners();
            btnUpgrade.onClick.AddListener(() =>
            {
                if (cacheConfig.preRequire.Select((node) => node.nodeId)
                    .Any((id) => UpgradeManager.Instance.GetData(id) == null || UpgradeManager.Instance.GetData(id).level == 0))
                    return;
                var success = UpgradeManager.Instance.UpgradeNode(cacheConfig.nodeId);
                if (success)
                    OnUpgradeSuccessCallback?.Invoke();
                else
                    OnUpgradeFailedCallback?.Invoke();
                
                UpdateUI();
            });
        }

        public void Setup(UpgradeNodeConfig config, bool forceUpdate, Action onUpgradeSuccessCallback, Action onUpgradeFailedCallback)
        {
            if (CanAutoShowHide == false && forceUpdate == false) return;
            
            cacheConfig = config;
            cacheData = UpgradeManager.Instance.GetData(config.nodeId);
            OnUpgradeSuccessCallback = onUpgradeSuccessCallback;
            OnUpgradeFailedCallback = onUpgradeFailedCallback;
            UpdateUI();
        }

        public void UpdateUI()
        {
            if (cacheData == null) return;
            if (cacheConfig == null) return;
            txtNodeName.SetText(cacheConfig.nodeName);
            txtNodeLore.SetText(cacheConfig.description);
            if (cacheData != null)
            {
                txtNodeLevel.SetText($"{cacheData.level}/{cacheConfig.levelNum}");
                txtNodePrice.SetText($"{0}/{1}");
            }
            else
            {
                txtNodeLevel.SetText($"Locked");
                txtNodePrice.SetText($"Locked");
            }
            
            for (var i = 0; i < cacheConfig.nodeLogic.Length; i++)
            {
                txtNodeBonus[i].SetText(cacheConfig.nodeLogic[i].GetDescription(cacheData?.level ?? 0));
                txtNodeBonus[i].gameObject.SetActive(true);
            }
            for (var i = cacheConfig.nodeLogic.Length; i < txtNodeBonus.Length; i++)
            {
                txtNodeBonus[i].gameObject.SetActive(false);
            }
        }
        
        public void Show(Vector2 position, Vector2 padding, bool forceShow)
        {
            if (CanAutoShowHide == false && forceShow == false) return;
            rectInfoFrame.position = position + padding;
            rectInfoFrame.gameObject.SetActive(true);
        }

        public void Hide(bool forceHide)
        {
            if (CanAutoShowHide == false && forceHide == false) return;
            rectInfoFrame.gameObject.SetActive(false);
        }
    }
}