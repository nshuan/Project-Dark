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
        [SerializeField] private GameObject imgActivatedMaxGlow;
        [SerializeField] private GameObject imgAvailable;
        [SerializeField] private GameObject imgLock;
        public float lineAnchorOffsetRadius;

        private void OnEnable()
        {
            UpdateUI();
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
                    imgActivatedGlow.SetActive(false);
                    imgActivatedMaxGlow.SetActive(false);

                    foreach (var lineInfo in preRequireLines)
                    {
                        lineInfo.line.UpdateLineState(UIUpgradeNodeState.Available);
                    }
                }
                else
                {
                    imgAvailable.SetActive(false);
                    imgLock.SetActive(true);
                    imgActivatedGlow.SetActive(false);
                    imgActivatedMaxGlow.SetActive(false);
                    
                    foreach (var lineInfo in preRequireLines)
                    {
                        lineInfo.line.UpdateLineState(
                            UpgradeManager.Instance.GetData(lineInfo.preRequireId) == null
                            ? UIUpgradeNodeState.Locked
                            : UIUpgradeNodeState.Available);
                    }
                }
            }
            else // Activated
            {
                imgAvailable.SetActive(true);
                imgLock.SetActive(false);
                imgActivatedGlow.SetActive(data.level < config.levelNum);
                imgActivatedMaxGlow.SetActive(data.level >= config.levelNum);
                
                foreach (var lineInfo in preRequireLines)
                {
                    lineInfo.line.UpdateLineState(UIUpgradeNodeState.Activated);
                }
            }
            
            hoverField.onHover = () =>
            {
                UIUpgradeNodeInfoPreview.Instance.Setup(config, false);
                UIUpgradeNodeInfoPreview.Instance.Show(transform.position, new Vector2(lineAnchorOffsetRadius, 0f), false);
            };
            hoverField.onHoverExit = () => UIUpgradeNodeInfoPreview.Instance.Hide(false);
            hoverField.onPointerClick = () =>
            {
                // treeRef.SelectNode(this);
                if (config.preRequire == null || config.preRequire.Select((node) => node.nodeId)
                    .Any((id) => UpgradeManager.Instance.GetData(id) == null || UpgradeManager.Instance.GetData(id).level == 0))
                    return;
                
                var success = UpgradeManager.Instance.UpgradeNode(config.nodeId);
                if (success)
                {
                    UIUpgradeNodeInfoPreview.Instance.Setup(config, true);
                    UIUpgradeNodeInfoPreview.Instance.Show(transform.position, new Vector2(lineAnchorOffsetRadius, 0f), true);
                    UpdateUI();
                    treeRef.UpdateChildren(config.nodeId);
                }
                else
                {
                    // TODO not success
                }
            };
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