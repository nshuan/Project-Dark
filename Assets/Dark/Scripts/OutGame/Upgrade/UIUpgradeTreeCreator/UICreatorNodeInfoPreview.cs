using Core;
using InGame.Upgrade;
using TMPro;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade.UIUpgradeTreeCreator
{
    public class UICreatorNodeInfoPreview : MonoSingleton<UICreatorNodeInfoPreview>
    {
        [SerializeField] private RectTransform rectInfoFrame;
        [SerializeField] private TextMeshProUGUI txtNodeName;
        [SerializeField] private TextMeshProUGUI txtNodeLore;
        [SerializeField] private TextMeshProUGUI txtNodeLevel;
        [SerializeField] private TextMeshProUGUI txtNodePrice;
        [SerializeField] private TextMeshProUGUI[] txtNodeBonus;
        
        public void UpdateUI(UpgradeNodeData data, UpgradeNodeConfig config)
        {
            txtNodeName.SetText(config.nodeName);
            txtNodeLore.SetText(config.description);
            if (data != null)
            {
                txtNodeLevel.SetText($"{data.level}/{config.levelNum}");
                txtNodePrice.SetText($"{0}/{1}");
            }
            else
            {
                txtNodeLevel.SetText($"{0}/{config.levelNum}");
                txtNodePrice.gameObject.SetActive(false);
            }
            
            for (var i = 0; i < config.nodeLogic.Length; i++)
            {
                txtNodeBonus[i].SetText(config.nodeLogic[i].GetDisplayValue(data?.level ?? 0));
                txtNodeBonus[i].gameObject.SetActive(true);
            }
            for (var i = config.nodeLogic.Length; i < txtNodeBonus.Length; i++)
            {
                txtNodeBonus[i].gameObject.SetActive(false);
            }
        }
        
        public void Show(Vector2 position, Vector2 padding)
        {
            rectInfoFrame.position = position + padding;
            rectInfoFrame.gameObject.SetActive(true);
        }

        public void Hide()
        {
            rectInfoFrame.gameObject.SetActive(false);
        }
    }
}