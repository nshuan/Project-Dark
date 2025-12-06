using System;
using System.Collections;
using DG.Tweening;
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
        public KeyCode keyHold = KeyCode.None;
        public float zoomSpeed = 0.1f;
        public float minScale = 0.5f;
        public float maxScale = 1.2f;
        public float defaultScale = 0.5f;

        private bool activateKeyHolding = false;
        private bool blockZoom = false;
        private Coroutine coroutineZoom;

        public static float CurrentScale;
        
        private void Awake()
        {
            if (btnResetZoom)
            {
                btnResetZoom.onClick.RemoveAllListeners();
                btnResetZoom.onClick.AddListener(ResetZoom);
            }
        }

        private void Start()
        {
            targetRect.localScale = Vector3.one * defaultScale;
            CurrentScale = defaultScale;
            if (keyHold == KeyCode.None)
                activateKeyHolding = true;
        }

        private void Update()
        {
            if (blockZoom) return;
            // // Skip if pointer is over another UI element
            // if (EventSystem.current && EventSystem.current.IsPointerOverGameObject() == false)
            //     return;

            if (keyHold != KeyCode.None)
            {
                if (Input.GetKey(keyHold))
                    activateKeyHolding = true;
                if (Input.GetKeyUp(keyHold))
                    activateKeyHolding = false;
            }

            if (activateKeyHolding == false) return;
            
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                Vector2 mousePosition = Input.mousePosition;
                UpdatePivot(targetRect, mousePosition);

                // Get current scale
                float currentScale = targetRect.localScale.x;
                float newScale = Mathf.Clamp(currentScale + scroll * zoomSpeed, minScale, maxScale);

                CurrentScale = newScale;

                // // Convert mouse position to local position in the targetRect
                // RectTransformUtility.ScreenPointToLocalPointInRectangle(
                //     targetRect, mousePosition, null, out Vector2 localPointBefore);

                // Apply the scale
                targetRect.localScale = new Vector3(newScale, newScale, 1f);
                txtZoom?.SetText($"x{newScale:F1}");
                
                // // Convert mouse position again after scaling
                // RectTransformUtility.ScreenPointToLocalPointInRectangle(
                //     targetRect, mousePosition, null, out Vector2 localPointAfter);

                // // Calculate difference caused by scaling
                // Vector2 delta = localPointAfter - localPointBefore;
                //
                // // Adjust anchoredPosition to compensate the shift
                // targetRect.anchoredPosition += delta;
            }
        }

        private void ResetZoom()
        {
            targetRect.localScale = Vector3.one;
            txtZoom.SetText("x1");
        }

        public void SetZoom(float scale)
        {
            targetRect.localScale = new Vector3(scale, scale, 1f);
        }
        
        public void ZoomTo(float scale, Vector2 position, float duration, float delay)
        {
            if (coroutineZoom != null) StopCoroutine(coroutineZoom);
            coroutineZoom = StartCoroutine(IEZoomTo(scale, position, duration, delay));
        }

        private IEnumerator IEZoomTo(float scale, Vector2 position, float duration, float delay)
        {
            blockZoom = true;
            yield return new WaitForSecondsRealtime(delay);
            UpdatePivot(targetRect, position);
            scale = Mathf.Clamp(scale, minScale, maxScale);
            var originalScale = targetRect.localScale.x;
            var originalPosition = targetRect.position;
            var scaleValue = scale - originalScale;
            var moveValue = transform.position - originalPosition;

            yield return DOTween.To(() => 0, x =>
            {
                targetRect.localScale = new Vector3(originalScale + x * scaleValue, originalScale + x * scaleValue, 1f);
                targetRect.position = originalPosition + x * moveValue;
            }, 1f, duration).SetUpdate(true).WaitForCompletion();

            CurrentScale = scale;
            // yield return targetRect.DOScale(new Vector3(scale, scale, 1f), duration).SetUpdate(true).WaitForCompletion();
            blockZoom = false;
        }
        
        private void UpdatePivot(RectTransform target, Vector2 pivotPosition)
        {
            var targetSizeScaled = new Vector2(target.rect.width * target.localScale.x,
                target.rect.height * target.localScale.y);
            var offset = (0.5f * Vector2.one - target.pivot) * targetSizeScaled;
            target.pivot = 0.5f * Vector2.one;
            target.position += (Vector3)offset;
            
            var pivotVector = -(Vector2)target.position + pivotPosition + 0.5f * targetSizeScaled;
            target.pivot = new Vector2(pivotVector.x / targetSizeScaled.x, pivotVector.y / targetSizeScaled.y);
            offset.x = (target.pivot.x - 0.5f) * targetSizeScaled.x;
            offset.y = (target.pivot.y - 0.5f) * targetSizeScaled.y;
            target.position += (Vector3)offset;
        }
    }
}