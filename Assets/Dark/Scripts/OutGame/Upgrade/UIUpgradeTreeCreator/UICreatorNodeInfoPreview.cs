using Core;
using InGame.Upgrade;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Upgrade.UIUpgradeTreeCreator
{
    public class UICreatorNodeInfoPreview : MonoSingleton<UICreatorNodeInfoPreview>
    {
        [SerializeField] private RectTransform rectInfoFrame;
        [SerializeField] private TextMeshProUGUI txtNodeName;
        [SerializeField] private TextMeshProUGUI txtNodeLore;
        [SerializeField] private TextMeshProUGUI txtNodeLevel;
        // [SerializeField] private TextMeshProUGUI txtNodePrice;
        [SerializeField] private TextMeshProUGUI[] txtNodeBonus;
        [SerializeField] private TMP_InputField inpGroup;
        public Button btnSelectGroup;
        public Button btnSetLockNode;
        
        public void UpdateUI(UICreatorManager manager, UICreatorUpgradeNode node, UpgradeNodeConfig config)
        {
            txtNodeName.SetText($"{config.nodeId} - {config.nodeName}");
            txtNodeLore.SetText(config.description);
            txtNodeLevel.SetText($"Max level: {config.MaxLevel}");
            
            if (config.nodeLogic.Length > 0)
            {
                var str = "";
                for (var i = 0; i < config.nodeLogic[0].MaxLevel; i++)
                {
                    str += config.nodeLogic[0].GetDisplayValue(i);
                    if (i != config.nodeLogic[0].MaxLevel - 1) str += ", ";
                }

                txtNodeBonus[0].SetText(str);
                txtNodeBonus[0].gameObject.SetActive(true);
            }

            inpGroup.onValueChanged.RemoveAllListeners();
            inpGroup.text = node.group.ToString();
            inpGroup.onValueChanged.AddListener((value) =>
            {
                if (int.TryParse(value, out var intValue))
                {
                    node.group = intValue;
                    manager.RefreshGroupNodes();
                }
            });
            
            btnSelectGroup.onClick.RemoveAllListeners();
            btnSelectGroup.onClick.AddListener(() =>
            {
                manager.SelectGroupNodes(node.group);
            });
            
            btnSetLockNode.onClick.RemoveAllListeners();
            btnSetLockNode.onClick.AddListener(() =>
            {
                manager.SetGroupLockNode(node);
            });
        }
        
        public void Show()
        {
            rectInfoFrame.gameObject.SetActive(true);
        }

        public void Hide()
        {
            rectInfoFrame.gameObject.SetActive(false);
        }
    }
}