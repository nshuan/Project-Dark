using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class ZoomInOut : MonoBehaviour
    {
        [SerializeField] private Button btnResetZoom;
        [SerializeField] private TextMeshProUGUI txtZoom;
        
        [Space]
        public RectTransform targetRect; // The UI element to zoom
        public float zoomSpeed = 0.1f;
        public float minScale = 0.5f;
        public float maxScale = 1.2f;

        private bool activateKeyHolding = false;

        private void Awake()
        {
            if (btnResetZoom)
            {
                btnResetZoom.onClick.RemoveAllListeners();
                btnResetZoom.onClick.AddListener(ResetZoom);
            }
        }

        private void Update()
        {
            // // Skip if pointer is over another UI element
            // if (EventSystem.current && EventSystem.current.IsPointerOverGameObject() == false)
            //     return;

            if (Input.GetKey(KeyCode.LeftControl))
                activateKeyHolding = true;
            if (Input.GetKeyUp(KeyCode.LeftControl))
                activateKeyHolding = false;

            if (activateKeyHolding == false) return;
            
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                Vector2 mousePosition = Input.mousePosition;

                // Get current scale
                float currentScale = targetRect.localScale.x;
                float newScale = Mathf.Clamp(currentScale + scroll * zoomSpeed, minScale, maxScale);
                float scaleFactor = newScale / currentScale;

                // Convert mouse position to local position in the targetRect
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    targetRect, mousePosition, null, out Vector2 localPointBefore);

                // Apply the scale
                targetRect.localScale = new Vector3(newScale, newScale, 1f);
                txtZoom?.SetText($"x{newScale:F1}");
                
                // Convert mouse position again after scaling
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    targetRect, mousePosition, null, out Vector2 localPointAfter);

                // Calculate difference caused by scaling
                Vector2 delta = localPointAfter - localPointBefore;

                // Adjust anchoredPosition to compensate the shift
                targetRect.anchoredPosition -= delta;
            }
        }

        private void ResetZoom()
        {
            targetRect.localScale = Vector3.one;
            txtZoom.SetText("x1");
        }
    }
}