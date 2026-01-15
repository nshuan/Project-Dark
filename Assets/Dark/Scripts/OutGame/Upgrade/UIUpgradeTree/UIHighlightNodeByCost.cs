using System;
using System.Collections.Generic;
using Economic;
using Economic.UI;
using InGame.Upgrade;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIHighlightNodeByCost : MonoBehaviour
    {
        private Dictionary<WealthType, UIUpgradeNode[]> nodesMapByCose;

        private WealthType hoveringType;
        
        public void InitNodesByCost(UIUpgradeTree tree)
        {
            nodesMapByCose ??= new Dictionary<WealthType, UIUpgradeNode[]>();
            
            var listVestige = new List<UIUpgradeNode>();
            var listEchoes = new List<UIUpgradeNode>();
            var listSigils = new List<UIUpgradeNode>();

            foreach (var nodeList in tree.NodesMap.Values)
            {
                foreach (var node in nodeList)
                {
                    foreach (var cost in node.config.costInfo)
                    {
                        if (cost.costType == WealthType.Vestige) listVestige.Add(node);
                        if (cost.costType == WealthType.Echoes) listEchoes.Add(node);
                        if (cost.costType == WealthType.Sigils) listSigils.Add(node);
                    }
                }
            }
            
            nodesMapByCose.Add(WealthType.Vestige, listVestige.ToArray());
            nodesMapByCose.Add(WealthType.Echoes, listEchoes.ToArray());
            nodesMapByCose.Add(WealthType.Sigils, listSigils.ToArray());
            
            UIEconomic.OnEconomicIconHoverIn += OnEconomicIconHoverIn;
            UIEconomic.OnEconomicIconHoverOut += OnEconomicIconHoverOut;
        }

        private void OnDestroy()
        {
            UIEconomic.OnEconomicIconHoverIn -= OnEconomicIconHoverIn;
            UIEconomic.OnEconomicIconHoverOut -= OnEconomicIconHoverOut;
        }

        private void OnEconomicIconHoverIn(WealthType wealthType)
        {
            if (nodesMapByCose == null) return;

            hoveringType = wealthType;

            foreach (WealthType type in Enum.GetValues(typeof(WealthType)))
            {
                if (nodesMapByCose.TryGetValue(type, out var nodeListHide))
                {
                    foreach (var node in nodeListHide)
                    {
                        node.Highlight(false);
                    }
                }
            }
            
            if (nodesMapByCose.TryGetValue(hoveringType, out var nodeList))
            {
                foreach (var node in nodeList)
                {
                    node.Highlight(true);
                }
            }
        }

        private void OnEconomicIconHoverOut()
        {
            foreach (WealthType type in Enum.GetValues(typeof(WealthType)))
            {
                if (nodesMapByCose.TryGetValue(type, out var nodeList))
                {
                    foreach (var node in nodeList)
                    {
                        node.HideHighlight();
                    }
                }
            }
        }
    }
}