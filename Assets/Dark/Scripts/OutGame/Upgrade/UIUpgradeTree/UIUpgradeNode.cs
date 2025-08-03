using System;
using System.Collections.Generic;
using System.Linq;
using InGame.Upgrade;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeNode : MonoBehaviour
    {
        public UIUpgradeTree treeRef;
        public UpgradeNodeConfig config;
        public List<UIUpgradeLineInfo> preRequireLines;
        
        [Space]
        [Header("UI")]
        [SerializeField] private UIUpgradeNodeHoverField hoverField;
        [SerializeField] private GameObject imgActivatedGlow;
        [SerializeField] private GameObject imgAvailable;
        [SerializeField] private GameObject imgLock;
        public float lineAnchorOffsetRadius;

        private void OnEnable()
        {
            UpdateUI();
            
            DeselectThis();
        }

        public void UpdateUI()
        {
            var data = UpgradeManager.Instance.GetData(config.nodeId);
            if (data == null) // Not activated yet
            {
                // Always available or all pre-required nodes are activated
                if (config.preRequire == null || config.preRequire.All((preRequire) => UpgradeManager.Instance.GetData(preRequire.nodeId) != null))
                {
                    imgAvailable.SetActive(true);
                    imgLock.SetActive(false);
                }
                else
                {
                    imgAvailable.SetActive(false);
                    imgLock.SetActive(true);
                }
            }
            else // Activated
            {
                imgAvailable.SetActive(true);
                imgLock.SetActive(false);
            }
            
            imgActivatedGlow.SetActive(false);
            
            hoverField.onHover = () =>
            {
                UIUpgradeNodeInfoPreview.Instance.UpdateUI(data, config, false);
                UIUpgradeNodeInfoPreview.Instance.Show(transform.position, new Vector2(lineAnchorOffsetRadius, 0f), false);
            };
            hoverField.onHoverExit = () => UIUpgradeNodeInfoPreview.Instance.Hide(false);
            hoverField.onPointerClick = () =>
            {
                treeRef.SelectNode(this);
                UIUpgradeNodeInfoPreview.Instance.UpdateUI(data, config, true);
                UIUpgradeNodeInfoPreview.Instance.Show(transform.position, new Vector2(lineAnchorOffsetRadius, 0f), true);
            };
        }

        public void SelectThis()
        {
            imgActivatedGlow.SetActive(true);
            if (preRequireLines is { Count: > 0 })
            {
                foreach (var preRequireLine in preRequireLines)
                {
                    preRequireLine.line.UpdateLineState(UIUpgradeNodeState.Activated);
                }
            }
        }

        public void DeselectThis()
        {
            imgActivatedGlow.SetActive(false);
            if (preRequireLines is { Count: > 0 })
            {
                foreach (var preRequireLine in preRequireLines)
                {
                    preRequireLine.line.UpdateLineState(UIUpgradeNodeState.Available);
                }
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, lineAnchorOffsetRadius);
        }
    }

    [Serializable]
    public class UIUpgradeLineInfo
    {
        public int preRequireId;
        public UIUpgradeLine line;
    }

    public enum UIUpgradeNodeState
    {
        Locked,
        Available,
        Activated
    }
}