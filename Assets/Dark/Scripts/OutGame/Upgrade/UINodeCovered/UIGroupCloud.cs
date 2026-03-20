using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Upgrade.UINodeCovered
{
    public class UIGroupCloud : MonoBehaviour
    {
        public int[] nodeIds;
        public int hideLayer = -1; // -1 là ẩn hết layer dưới
        public UIUpgradeNode hideNodeFrom;
        
        private UIUpgradeTree upgradeTree;
        private Image[] allClouds;
        private CanvasGroup cvgCloud;

        private bool initialzed;
        private UICloudFloat[] allCloudFloat;
        
        private void Awake()
        {
            if (!GameConst.HideLockedAreaByCloud)
            {
                gameObject.SetActive(false);
                return;
            }
            
            upgradeTree = GetComponentInParent<UIUpgradeTree>();
            allClouds = GetComponentsInChildren<Image>();
            allCloudFloat = new UICloudFloat[allClouds.Length];
            for (var i = 0; i < allClouds.Length; i++)
            {
                var cloud = allClouds[i];
                if (!cloud.TryGetComponent<UICloudFloat>(out var cloudFloat))
                {
                    cloudFloat = cloud.gameObject.AddComponent<UICloudFloat>();
                }

                cloudFloat.preset = CloudTweenPreset.Instance;
                allCloudFloat[i] = cloudFloat;
            }

            if (!TryGetComponent<CanvasGroup>(out cvgCloud))
                cvgCloud = gameObject.AddComponent<CanvasGroup>();

            if (upgradeTree)
            {
                upgradeTree.OnNodeUpgraded += OnNodeUpgraded;
                upgradeTree.OnTreeSpawned += Init;
                Init();
            }
        }

        private void OnNodeUpgraded(UIUpgradeNode upgradedNode)
        {
            if (nodeIds == null) return;
            if (!nodeIds.Contains(upgradedNode.config.nodeId)) return;
            if (allClouds == null) return;
            if (!cvgCloud) return;

            DOTween.Kill(this);
            cvgCloud.DOFade(0f, 1f).SetEase(Ease.OutQuad).SetTarget(this);
            foreach (var cloud in allCloudFloat)
            {
                cloud.TriggerSpeedScale(10f);
            }
            
            // Show nodes
            if (!hideNodeFrom) return;

            ShowNode(hideNodeFrom);
                        
            var queueCheck = new Queue<UIUpgradeNode>();
            if (upgradeTree.nodeChildrenMap.TryGetValue(hideNodeFrom.config.nodeId, out var childrenLayer1))
            {
                foreach (var child in childrenLayer1)
                {
                    ShowNode(child);
                    if (!queueCheck.Contains(child)) queueCheck.Enqueue(child);
                }
            }

            if (hideLayer == -1)
            {
                while (queueCheck.Count > 0)
                {
                    var node = queueCheck.Dequeue();
                    if (upgradeTree.nodeChildrenMap.TryGetValue(node.config.nodeId, out var children))
                    {
                        foreach (var child in children)
                        {
                            ShowNode(child);
                            if (!queueCheck.Contains(child)) queueCheck.Enqueue(child);
                        }
                    }
                }
            }
        }

        private void Init()
        {
            if (initialzed) return;
            initialzed = true;
            if (nodeIds == null) return;
            if (!cvgCloud)
            {
                if (!TryGetComponent<CanvasGroup>(out cvgCloud))
                    cvgCloud = gameObject.AddComponent<CanvasGroup>();
            }

            var shouldShow = true;
            foreach (var id in nodeIds)
            {
                if (upgradeTree.NodesMap.TryGetValue(id, out var nodes))
                {
                    if (nodes.Any((node) => node.CurrentState == UIUpgradeNodeState.Activated))
                    {
                        cvgCloud.alpha = 0f;
                        shouldShow = false;
                        break;
                    }
                }
            }
            
            if (!shouldShow) return;
            upgradeTree.OnNodeUpgraded += OnNodeUpgraded;
            
            // Hide nodes
            if (!hideNodeFrom) return;

            HideNode(hideNodeFrom);
                        
            var queueCheck = new Queue<UIUpgradeNode>();
            if (upgradeTree.nodeChildrenMap.TryGetValue(hideNodeFrom.config.nodeId, out var childrenLayer1))
            {
                foreach (var child in childrenLayer1)
                {
                    HideNode(child);
                    if (!queueCheck.Contains(child)) queueCheck.Enqueue(child);
                }
            }

            if (hideLayer == -1)
            {
                while (queueCheck.Count > 0)
                {
                    var node = queueCheck.Dequeue();
                    if (upgradeTree.nodeChildrenMap.TryGetValue(node.config.nodeId, out var children))
                    {
                        foreach (var child in children)
                        {
                            HideNode(child);
                            if (!queueCheck.Contains(child)) queueCheck.Enqueue(child);
                        }
                    }
                }
            }
        }
        
        void HideNode(UIUpgradeNode node)
        {
            node.groupNode.alpha = 0f;
            node.flagHideOnSpawn = true;
            node.NodeAlphaOnHidden = 0;

            if (node.preRequires != null)
            {
                foreach (var pre in node.preRequires)
                {
                    pre.line.groupLine.alpha = 0f;
                    pre.line.flagHideOnSpawn = true;
                }
            }
        }

        void ShowNode(UIUpgradeNode node)
        {
            node.groupNode.alpha = 1f;
            node.NodeAlphaOnHidden = 1f;

            if (node.preRequires != null)
            {
                foreach (var pre in node.preRequires)
                {
                    pre.line.groupLine.alpha = 1f;
                }
            }
        }

#if UNITY_EDITOR
        [Button]
        private void FindNodeToHideFrom()
        {
            var tree = GetComponentInParent<UIUpgradeTree>();
            if (nodeIds == null || nodeIds.Length == 0) return;
            if (!tree) return;
            if (tree.nodeChildrenMap == null || !tree.nodeChildrenMap.TryGetValue(nodeIds[0], out var children)) return;
            if (children == null || children.Count == 0) return;
            hideNodeFrom = children[0];
        }
#endif
    }
}