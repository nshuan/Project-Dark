using System;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;

namespace InGame.Upgrade.UI
{
    public class UIUpgradeNode : SerializedMonoBehaviour
    {
        [ReadOnly, NonSerialized, OdinSerialize]
        public UpgradeNodeConfig nodeConfig;
        
        [SerializeField] private TextMeshProUGUI txtId;
        [SerializeField] private TextMeshProUGUI txtName;
        [SerializeField] private TextMeshProUGUI txtPreRequiredIds;
        [SerializeField] private TextMeshProUGUI txtLevel;

        public virtual void UpdateUI(UpgradeNodeData data)
        {
            txtId.SetText($"ID: {nodeConfig.nodeId}");
            txtName.SetText($"Name: {nodeConfig.name}");
            var txtPreRequire = nodeConfig.preRequire.Aggregate("", (current, node) => current + (node.nodeId + ", "));
            txtPreRequire = txtPreRequire.Remove(txtPreRequire.Length - 2);
            txtPreRequiredIds.SetText($"Pre Require: {txtPreRequire}");
            txtLevel.SetText($"{data.level}/{nodeConfig.levelNum}");
        }
    }
}