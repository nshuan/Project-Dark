using System;
using System.Linq;
using InGame.Upgrade.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.Upgrade.UIDummy
{
    public class DummyUpgradeNode : MonoBehaviour
    {
        public UpgradeNodeConfig nodeConfig;
        public UIUpgradeNode nodeUI;

        [SerializeField] private GameObject nodeEditor;
        [SerializeField] private Image nodeImage;
        [SerializeField] private TextMeshProUGUI txtId;
        [SerializeField] private TextMeshProUGUI txtName;
        [SerializeField] private TextMeshProUGUI txtPreRequiredIds;
        [SerializeField] private TextMeshProUGUI txtLevel;

        public void UpdateUI()
        {
            txtId.SetText($"ID: {nodeConfig.nodeId}");
            txtName.SetText($"Name: {nodeConfig.nodeName}");
            if (nodeConfig.preRequire != null && nodeConfig.preRequire.Length > 0)
            {
                var txtPreRequire = nodeConfig.preRequire.Aggregate("", (current, node) => current + (node.nodeId + ", "));
                txtPreRequire = txtPreRequire.Remove(txtPreRequire.Length - 2);
                txtPreRequiredIds.SetText($"Pre Require: {txtPreRequire}");
            }
            else
            {
                txtPreRequiredIds.SetText($"Pre Require:");
            }
            txtLevel.SetText($"{0}/{nodeConfig.levelNum}");
        }

        public void HidePlaceHolder()
        {
            nodeImage.enabled = false;
        }
    }
}