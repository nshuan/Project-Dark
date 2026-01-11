using System;
using System.Collections.Generic;
using System.Linq;
using InGame.Upgrade;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Upgrade.UIUpgradeTreeCreator
{
    public class UICreatorUpgradeNode : MonoBehaviour
    {
        public UICreatorManager manager;
        public UpgradeNodeConfig config;
        public Guid guid;
        public List<int> group;
        public Dictionary<int, bool> isGroupLockNode;

        [Space] [Header("UI")] 
        [SerializeField] private UICreatorUpgradeNodeHover hoverField;
        [SerializeField] private GameObject glow;
        public float lineAnchorOffsetRadius;
        public Image nodeVisual;
        public GameObject objAreaLock;
        
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
            hoverField.onClickRight = () =>
            {
                if (manager.isLinkMode)
                {
                    manager.SelectNode(this);
                    manager.ChangeMode();
                }
                else
                {
                    manager.ChangeMode();
                    manager.SelectNode(this);
                }
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
        
        public void SetIcon(Sprite sprite)
        {
            nodeVisual.sprite = sprite;
            nodeVisual.SetNativeSize();
        }

        public void SetAreaLock()
        {
            objAreaLock.SetActive(isGroupLockNode != null && isGroupLockNode.Any((pair) => pair.Value));
        }
    }
}