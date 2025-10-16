using System;
using Dark.Scripts.OutGame.Upgrade.UIUpgradeTreeCreator.Grid;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dark.Scripts.OutGame.Upgrade.UIUpgradeTreeCreator
{
    public class UICreatorUpgradeNodeHover : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IAutoAlign
    {
        public RectTransform rectTransform;

        public Action onDrag;
        public Action onClick;

        private bool isDrag;

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDrag = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!rectTransform) return;
            rectTransform.position += (Vector3)eventData.delta;
            onDrag?.Invoke();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDrag = false;
            Align();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (isDrag) return;
            onClick?.Invoke();
        }
        
        private RaycastHit2D[] hits;
        public void Align()
        {
            // hits ??= new RaycastHit2D[10];
            // var count = Physics2D.CircleCastNonAlloc(rectTransform.position,
            //     UIAlignManager.Instance.anchorDistance / Mathf.Sqrt(2), Vector2.right, hits, 0f, UIAlignManager.Instance.anchorLayer);
            //
            // var minDistance = UIAlignManager.Instance.anchorDistance;
            // Transform nearestHit = null;
            // for (var i = 0; i < Math.Min(count, hits.Length); i++)
            // {
            //     var tempDistance = Vector2.Distance(hits[i].transform.position, rectTransform.position);
            //     if (tempDistance <= minDistance)
            //     {
            //         minDistance = tempDistance;
            //         nearestHit = hits[i].transform;
            //     }
            // }
            //
            // if (nearestHit)
            //     rectTransform.position = nearestHit.position;
            
            UIAlignManager.Instance.Align(rectTransform);
        }
    }
}