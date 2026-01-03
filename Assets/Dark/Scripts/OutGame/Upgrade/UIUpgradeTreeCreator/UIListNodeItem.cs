using InGame.Upgrade;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Upgrade.UIUpgradeTreeCreator
{
    public class UIListNodeItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtNodeId;
        [SerializeField] private TextMeshProUGUI txtNodeName;
        [SerializeField] private TextMeshProUGUI txtNodeDescription;
        [SerializeField] public Button btnSelect;
        [SerializeField] private GameObject objSelected;

        public void Setup(UpgradeNodeConfig nodeConfig)
        {
            if (nodeConfig == null)
            {
                Debug.LogError("NodeConfig is null!");
                return;
            }

            if (txtNodeId != null)
            {
                txtNodeId.SetText($"{nodeConfig.nodeId}");
            }

            if (txtNodeName != null)
            {
                txtNodeName.SetText(nodeConfig.nodeName);
            }

            if (txtNodeDescription != null)
            {
                txtNodeDescription.SetText(nodeConfig.description);
            }
        }

        public void SetSelected(bool selected)
        {
            objSelected.SetActive(selected);
        }
    }
}