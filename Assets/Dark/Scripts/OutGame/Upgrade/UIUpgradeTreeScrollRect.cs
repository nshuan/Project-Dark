using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeTreeScrollRect : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private UIPanelUpgradeTree panelUpgradeTree;
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (panelUpgradeTree.Tree) panelUpgradeTree.TreeCvg.blocksRaycasts = false;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (panelUpgradeTree.Tree) panelUpgradeTree.TreeCvg.blocksRaycasts = true;
        }
    }
}