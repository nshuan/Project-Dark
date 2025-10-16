using System;
using Core;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade.UIUpgradeTreeCreator.Grid
{
    public class UIAlignManager : MonoSingleton<UIAlignManager>
    {
        [Header("Setup")]
        public RectTransform parentRect;         // The parent container (e.g., Content of a ScrollView)
        
        private Vector2 spacing = new Vector2(8f, 8f);   // distance between NEAREST edges (x,y)

        private void Start()
        {
            SetupGrid();
        }

        public void SetupGrid()
        {
            if (parentRect == null ) return;
            Clear();

            spacing = GridConfig.Instance.spacing;

            // Ensure parent pivot is center so (0,0) is center anchor space.
            // If not, we still place correctly by offsetting from rect center (which is always 0,0 with pivot 0.5,0.5).
            // Recommended: set parentRect.pivot = new Vector2(0.5f, 0.5f);

            Vector2 parentSize = parentRect.rect.size;

            // Step between centers = itemSize + spacing (since spacing is edge-to-edge)
            Vector2 step = spacing;

            // Max indices from the center that still fit fully inside the parent
            int maxIx = Mathf.FloorToInt((parentSize.x * 0.5f) / step.x);
            int maxIy = Mathf.FloorToInt((parentSize.y * 0.5f) / step.y);

            for (int y = -maxIy; y <= maxIy; y++)
            {
                UIMultiLineRenderer.DrawLine(new Vector2(-maxIx * step.x, y * step.y), new Vector2(maxIx * step.x, y * step.y));
            }

            for (int x = -maxIx; x <= maxIx ; x++)
            {
                UIMultiLineRenderer.DrawLine(new Vector2(x * step.x, -maxIy * step.y), new Vector2(x * step.x, maxIy * step.y));
            }
        }

        public void Clear()
        {
            UIMultiLineRenderer.Clear();
        }

        public void Align(RectTransform target)
        {
            var minX = ((int)(target.anchoredPosition.x / spacing.x)) * spacing.x;
            var maxX = minX + spacing.x * Mathf.Sign(target.anchoredPosition.x);
            var minY = ((int)(target.anchoredPosition.y / spacing.y)) * spacing.y;
            var maxY = minY + spacing.y * Mathf.Sign(target.anchoredPosition.y);

            var nearest = new Vector2();

            if (Mathf.Abs(maxX - target.anchoredPosition.x) > Mathf.Abs(target.anchoredPosition.x - minX))
                nearest.x = minX;
            else nearest.x = maxX;
            
            if (Mathf.Abs(maxY - target.anchoredPosition.y) > Mathf.Abs(target.anchoredPosition.y - minY))
                nearest.y = minY;
            else nearest.y = maxY;
            
            target.anchoredPosition = nearest;
        }
    }
}