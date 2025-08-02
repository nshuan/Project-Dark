using System.Collections.Generic;
using InGame.Upgrade;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade.UIUpgradeTreeCreator
{
    public class UICreatorUpgradeNode : MonoBehaviour
    {
        public UICreatorManager manager;
        public UpgradeNodeConfig config;
        
        [Space]
        [Header("UI")]
        [SerializeField] private UICreatorUpgradeNodeHover hoverField;
        [SerializeField] private GameObject glow;

        public void InitNode()
        {
            hoverField.rectTransform = (RectTransform)transform;
            hoverField.onDrag = () =>
            {
                manager.UpdateLine(config.nodeId);
            };
            hoverField.onClick = () =>
            {
                manager.SelectNode(this);
            };
        }

        public void SelectThis()
        {
            glow.SetActive(true);
        }

        public void DeselectThis()
        {
            glow.SetActive(false);
        }
    }
}