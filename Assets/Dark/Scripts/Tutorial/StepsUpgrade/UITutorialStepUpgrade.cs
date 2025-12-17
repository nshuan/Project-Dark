using System;
using System.Linq;
using Dark.Scripts.OutGame.Upgrade;
using Dark.Scripts.Utils;
using InGame.Upgrade;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.Tutorial
{
    public class UITutorialStepUpgrade : UIAbstractTutorialStep
    {
        [SerializeField] private GameObject objPointToResource;
        [SerializeField] private GameObject objInstruction;
        [SerializeField] private GameObject objTreeInstruction;
        [SerializeField] private Button btnCloseTreeInstruction;
        [SerializeField] private UIPanelUpgradeTree panelUpgradeTree;

        [Space] [Header("Config")] 
        [SerializeField] private float delay;

        private Transform targetNode;
        
        public override bool IsValid()
        {
            if (UpgradeManager.Instance.Data.nodes is { Count: > 1 } &&
                UpgradeManager.Instance.Data.nodes.Any((node) => node.id != 2 && node.level > 0)) return false;
            return true;
        }

        public override void Setup()
        {
            objPointToResource.SetActive(false);
            objInstruction.SetActive(false);
            btnCloseTreeInstruction.gameObject.SetActive(false);
            
            if (!panelUpgradeTree)
            {
                return;
            }

            if (!panelUpgradeTree.Tree)
            {
                return;
            }

            targetNode = panelUpgradeTree.Tree.nodesMapByLayer[1][0].transform;
            this.DelayCall(delay, () =>
            {
                if (targetNode) 
                    objInstruction.transform.position = targetNode.position;
                objPointToResource.SetActive(true);
                objInstruction.SetActive(true);
                
                panelUpgradeTree.Tree.OnNodeUpgraded += OnNodeUpgraded;
            });
        }

        public override void Setup(Action<Vector2, Vector2, float, bool, bool> actionUpdateFocus)
        {
            Setup();
        }

        private void OnNodeUpgraded(UIUpgradeNode node)
        {
            panelUpgradeTree.Tree.OnNodeUpgraded -= OnNodeUpgraded;
            btnCloseTreeInstruction.onClick.RemoveAllListeners();
            btnCloseTreeInstruction.onClick.AddListener(() =>
            {
                OnComplete?.Invoke();
            });
            
            objPointToResource.SetActive(false);
            objInstruction.SetActive(false);
            btnCloseTreeInstruction.gameObject.SetActive(true);
        }

        private void Update()
        {
             if (objInstruction.activeInHierarchy && targetNode) 
                 objInstruction.transform.position = targetNode.position;
             if (objTreeInstruction.activeInHierarchy && panelUpgradeTree.Tree)
                 objTreeInstruction.transform.position = panelUpgradeTree.Tree.transform.position;
        }
    }
}