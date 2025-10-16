using System;
using Core;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade.UIUpgradeTreeCreator.Grid
{
    public class UIAlignManager : MonoSingleton<UIAlignManager>
    {
        public float anchorDistance;
        public LayerMask anchorLayer;

        [Header("Setup")]
        public RectTransform parentRect;         // The parent container (e.g., Content of a ScrollView)
        public RectTransform prefab;             // Prefab with a RectTransform

        [Header("Layout")]
        public Vector2 spacing = new Vector2(8f, 8f);   // distance between NEAREST edges (x,y)
        public bool autoClearOnSpawn = true;

        [Header("Item Size (leave zero to read from prefab)")]
        public Vector2 overrideItemSize = Vector2.zero; // if (0,0) we use prefab size

        private void Start()
        {
            SetupGrid();
        }

        public void SetupGrid()
        {
            if (parentRect == null || prefab == null) return;
            if (autoClearOnSpawn) Clear();

            // Ensure parent pivot is center so (0,0) is center anchor space.
            // If not, we still place correctly by offsetting from rect center (which is always 0,0 with pivot 0.5,0.5).
            // Recommended: set parentRect.pivot = new Vector2(0.5f, 0.5f);

            Vector2 parentSize = parentRect.rect.size;

            // Step between centers = itemSize + spacing (since spacing is edge-to-edge)
            Vector2 step = spacing;

            // Max indices from the center that still fit fully inside the parent
            int maxIx = Mathf.FloorToInt((parentSize.x * 0.5f) / step.x);
            int maxIy = Mathf.FloorToInt((parentSize.y * 0.5f) / step.y);

            // Instantiate grid, centered: (0,0) cell exists
            for (int y = -maxIy; y <= maxIy; y++)
            {
                for (int x = -maxIx; x <= maxIx; x++)
                {
                    var item = Instantiate(prefab, parentRect);
                    var rt = item;
                    rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f); // place by anchoredPosition from center
                    rt.pivot = new Vector2(0.5f, 0.5f);

                    // Position (centered grid)
                    rt.anchoredPosition = new Vector2(x * step.x, y * step.y);
                }
            }
        }
        
        public void Clear()
        {
            if (parentRect == null) return;
            for (int i = parentRect.childCount - 1; i >= 0; i--)
            {
                var c = parentRect.GetChild(i);
    #if UNITY_EDITOR
                if (!Application.isPlaying) DestroyImmediate(c.gameObject);
                else Destroy(c.gameObject);
    #else
                Destroy(c.gameObject);
    #endif
            }
        }
    }
}