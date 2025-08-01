using System;
using InGame.Upgrade;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeNode : MonoBehaviour
    {
        public UIUpgradeTree treeRef;
        public UpgradeNodeConfig config;
        
        [Space]
        [Header("UI")]
        [SerializeField] private UIUpgradeNodeHoverField hoverField;

        private void OnEnable()
        {
            UpdateUI();
        }

        public void UpdateUI()
        {
            hoverField.onHover = () =>
            {
                UIUpgradeNodeInfoPreview.Instance.UpdateUI(UpgradeManager.Instance.GetData(config.nodeId), config);
                UIUpgradeNodeInfoPreview.Instance.Show(transform.position,
                    ((RectTransform)hoverField.transform).rect.size / 2 + new Vector2(10f, 0f));
            };
            hoverField.onHoverExit = () => UIUpgradeNodeInfoPreview.Instance.Hide();
            
            if (config.preRequire == null || config.preRequire.Length == 0) return;
            foreach (var nodeConfig in config.preRequire)
            {
                treeRef.ShowPreRequiredLine(transform.position, treeRef.GetNodeById(nodeConfig.nodeId).transform.position);
            }
        }
    }
}