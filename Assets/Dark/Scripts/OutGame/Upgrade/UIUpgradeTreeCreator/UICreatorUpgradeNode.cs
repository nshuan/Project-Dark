using System.Collections.Generic;
using InGame.Upgrade;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade.UIUpgradeTreeCreator
{
    public class UICreatorUpgradeNode : MonoBehaviour
    {
        public UICreatorManager manager;
        public UpgradeNodeConfig config;
        public List<UICreatorUpgradeNode> childrenNodes;
        
        [Space]
        [Header("UI")]
        [SerializeField] private UICreatorUpgradeNodeHover hoverField;

        public void InitNode()
        {
            if (config.preRequire != null)
            {
                foreach (var nodeConfig in config.preRequire)
                {
                    var preRequireNode = manager.GetNodeById(nodeConfig.nodeId);
                    if (!preRequireNode) continue;
                    manager.ShowPreRequiredLine(config.nodeId, transform.position, preRequireNode.config.nodeId, preRequireNode.transform.position);
                    if (!preRequireNode.childrenNodes.Contains(this)) preRequireNode.childrenNodes.Add(this);
                }
            }
            
            childrenNodes = new List<UICreatorUpgradeNode>();
            
            hoverField.rectTransform = (RectTransform)transform;
            hoverField.onDrag = () =>
            {
                foreach (var nodeConfig in config.preRequire)
                {
                    var preRequireNode = manager.GetNodeById(nodeConfig.nodeId);
                    if (!preRequireNode) continue;
                    manager.ShowPreRequiredLine(config.nodeId, transform.position, preRequireNode.config.nodeId,
                        preRequireNode.transform.position);
                }

                foreach (var child in childrenNodes)
                {
                    manager.ShowPreRequiredLine(child.config.nodeId, child.transform.position,
                        config.nodeId, transform.position);
                }
            };
        }
    }
}