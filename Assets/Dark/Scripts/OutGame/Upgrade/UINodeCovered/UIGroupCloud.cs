using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Upgrade.UINodeCovered
{
    public class UIGroupCloud : MonoBehaviour
    {
        public int[] nodeIds;
        
        private UIUpgradeTree upgradeTree;
        private Image[] allClouds;
        private CanvasGroup cvgCloud;

        private bool initialzed;
        
        private void Awake()
        {
            if (!GameConst.HideLockedAreaByCloud)
            {
                gameObject.SetActive(false);
                return;
            }
            
            upgradeTree = GetComponentInParent<UIUpgradeTree>();
            allClouds = GetComponentsInChildren<Image>();
            if (!TryGetComponent<CanvasGroup>(out cvgCloud))
                cvgCloud = gameObject.AddComponent<CanvasGroup>();

            if (upgradeTree)
            {
                upgradeTree.OnNodeUpgraded += OnNodeUpgraded;
                upgradeTree.OnTreeSpawned += Init;
                Init();
            }
        }

        private void OnNodeUpgraded(UIUpgradeNode node)
        {
            if (nodeIds == null) return;
            if (!nodeIds.Contains(node.config.nodeId)) return;
            if (allClouds == null) return;
            if (!cvgCloud) return;

            DOTween.Kill(this);
            cvgCloud.DOFade(0f, 1f).SetEase(Ease.OutQuad).SetTarget(this);
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

            foreach (var id in nodeIds)
            {
                if (upgradeTree.NodesMap.TryGetValue(id, out var nodes) &&
                    nodes.Any((node) => node.CurrentState == UIUpgradeNodeState.Activated))
                {
                    cvgCloud.alpha = 0f;
                    break;
                }
            }
        }
    }
}