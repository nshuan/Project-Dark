using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIInfoPreviewContentFitter : MonoBehaviour
    {
        [SerializeField] private RectOffset padding = new RectOffset();
        [SerializeField] private float spacing;
        
        public RectTransform rectTransform;
        
        private void Awake()
        {
            rectTransform ??= GetComponent<RectTransform>();
        }
        
        public Vector2 GetSize()
        {
            if (!rectTransform) return Vector2.zero;
            
            var size = Vector2.zero;
            
            if (rectTransform.childCount == 0)
            {
                size = new Vector2(rectTransform.sizeDelta.x, padding.top + padding.bottom);
                return size;
            }

            size.x = rectTransform.sizeDelta.x;

            size.y += padding.top;
            
            foreach (RectTransform child in rectTransform)
            {
                if (child.TryGetComponent<LayoutElement>(out var childLayout) && childLayout.ignoreLayout)
                    continue;

                if (child.TryGetComponent<TextMeshProUGUI>(out var textMesh))
                {
                    textMesh.ForceMeshUpdate();
                    child.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textMesh.preferredHeight);
                }
                
                size.y += child.sizeDelta.y;
                size.y += spacing;
            }

            size.y -= spacing;
            size.y += padding.bottom;

            return size;
        }
    }
}