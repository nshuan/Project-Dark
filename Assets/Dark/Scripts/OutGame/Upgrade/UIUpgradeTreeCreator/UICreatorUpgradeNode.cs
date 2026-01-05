using System;
using System.Collections.Generic;
using InGame.Upgrade;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade.UIUpgradeTreeCreator
{
    public class UICreatorUpgradeNode : MonoBehaviour
    {
        public UICreatorManager manager;
        public UpgradeNodeConfig config;
        public Guid guid;
        public int group;

        [Space] [Header("UI")] 
        [SerializeField] private UICreatorUpgradeNodeHover hoverField;
        [SerializeField] private GameObject glow;
        public float lineAnchorOffsetRadius;
        
        public NodeType CreatorNodeType { get; set; }
        public int PrefabIndex { get; set; }
        
        public void InitNode()
        {
            hoverField.rectTransform = (RectTransform)transform;
            hoverField.onDrag = (anchor, delta) =>
            {
                manager.OnDragNode(anchor, delta);
                manager.UpdateLine(guid);
            };
            hoverField.onClick = () =>
            {
                manager.SelectNode(this);
            };
            hoverField.onEndDrag = () =>
            {
                manager.OnEndDrag();
                manager.UpdateLine(guid);
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
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, lineAnchorOffsetRadius);
        }
    }
}