using System;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.Upgrade.UI
{
    public class UIUpgradeNode : SerializedMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtId;
        [SerializeField] private TextMeshProUGUI txtName;
        [SerializeField] private TextMeshProUGUI txtPreRequiredIds;
        [SerializeField] private TextMeshProUGUI txtLevel;
        [SerializeField] private Button btnUpgrade;

        public virtual void UpdateUI(UpgradeNodeData data, UpgradeNodeConfig nodeConfig, Action onUpgradeSuccessCallback = null, Action onUpgradeFailureCallback = null)
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
            txtLevel.SetText($"{data.level}/{nodeConfig.levelNum}");
            
            btnUpgrade.onClick.RemoveAllListeners();
            btnUpgrade.onClick.AddListener(() =>
            {
                if (nodeConfig.preRequire.Select((node) => node.nodeId).Any((id) => UpgradeManager.Instance.GetData(id).level == 0))
                    return;
                
                var success = UpgradeManager.Instance.UpgradeNode(nodeConfig.nodeId);
                if (success)    
                    onUpgradeSuccessCallback?.Invoke();
                else
                    onUpgradeFailureCallback?.Invoke();
            });
        }
    }
}