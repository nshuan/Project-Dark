using System.Collections;
using Core;
using DG.Tweening;
using UnityEngine;

namespace InGame.CameraController
{
    public class InGameCameraController : MonoSingleton<InGameCameraController>
    {
        [Header("Camera Reference")]
        [SerializeField] private Camera targetCamera;
        
        [Header("Zoom Settings")]
        [SerializeField] private float zoomDuration = 1f;
        [SerializeField] private float zoomAmount = 2.5f;
        [SerializeField] private float delayBeforeZoom = 0f;
        [SerializeField] private float holdDuration = 1f;
        [SerializeField] private float resetDuration = 1f;
        
        [Header("Easing")]
        [SerializeField] private Ease zoomInEase = Ease.OutQuad;
        [SerializeField] private Ease zoomOutEase = Ease.InQuad;
        
        // Private fields
        private float originalOrthoSize;
        private Vector3 originalPosition;
        private Sequence currentZoomSequence;
        private bool isZooming = false;
        
        private void Awake()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }
            
            if (targetCamera != null)
            {
                originalOrthoSize = targetCamera.orthographicSize;
                originalPosition = targetCamera.transform.position;
            }
        }
        
        /// <summary>
        /// Zooms the camera to a specific transform with the configured settings
        /// </summary>
        /// <param name="targetTransform">The transform to zoom to</param>
        public void ZoomToTransform(Transform targetTransform)
        {
            if (targetTransform == null || targetCamera == null)
            {
                Debug.LogWarning("InGameCameraController: Target transform or camera is null!");
                return;
            }
            
            if (isZooming)
            {
                StopCurrentZoom();
            }
            
            StartCoroutine(ZoomToTransformCoroutine(targetTransform));
        }
        
        /// <summary>
        /// Zooms the camera to a specific transform with custom parameters
        /// </summary>
        /// <param name="targetTransform">The transform to zoom to</param>
        /// <param name="customZoomDuration">Custom zoom in duration</param>
        /// <param name="customZoomAmount">Custom zoom amount (orthographic size)</param>
        /// <param name="customDelay">Custom delay before zoom starts</param>
        /// <param name="customHoldDuration">Custom time to hold at zoomed state</param>
        /// <param name="customResetDuration">Custom duration to reset back</param>
        public void ZoomToTransform(Transform targetTransform, float customZoomDuration, 
            float customZoomAmount, float customDelay, float customHoldDuration, float customResetDuration)
        {
            if (targetTransform == null || targetCamera == null)
            {
                Debug.LogWarning("InGameCameraController: Target transform or camera is null!");
                return;
            }
            
            if (isZooming)
            {
                StopCurrentZoom();
            }
            
            StartCoroutine(ZoomToTransformCoroutine(targetTransform, customZoomDuration, 
                customZoomAmount, customDelay, customHoldDuration, customResetDuration));
        }
        
        /// <summary>
        /// Zooms the camera to a specific world position
        /// </summary>
        /// <param name="targetPosition">The world position to zoom to</param>
        public void ZoomToPosition(Vector3 targetPosition)
        {
            if (!targetCamera)
            {
                Debug.LogWarning("InGameCameraController: Camera is null!");
                return;
            }
            
            if (isZooming)
            {
                StopCurrentZoom();
            }
            
            StartCoroutine(ZoomToPositionCoroutine(targetPosition));
        }
        
        /// <summary>
        /// Stops the current zoom operation and resets camera immediately
        /// </summary>
        public void StopZoomAndReset()
        {
            StopCurrentZoom();
            ResetCamera();
        }
        
        /// <summary>
        /// Immediately resets the camera to its original state
        /// </summary>
        public void ResetCamera()
        {
            if (targetCamera == null) return;
            
            if (currentZoomSequence != null && currentZoomSequence.IsActive())
            {
                currentZoomSequence.Kill();
            }
            
            targetCamera.orthographicSize = originalOrthoSize;
            targetCamera.transform.position = originalPosition;
            isZooming = false;
        }
        
        private IEnumerator ZoomToTransformCoroutine(Transform targetTransform)
        {
            yield return ZoomToTransformCoroutine(targetTransform, zoomDuration, zoomAmount, 
                delayBeforeZoom, holdDuration, resetDuration);
        }
        
        private IEnumerator ZoomToTransformCoroutine(Transform targetTransform, float duration, 
            float zoomSize, float delay, float holdTime, float resetTime)
        {
            isZooming = true;
            
            // Wait for delay
            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }
            
            // Store current state if not already stored
            if (originalOrthoSize <= 0f)
            {
                originalOrthoSize = targetCamera.orthographicSize;
                originalPosition = targetCamera.transform.position;
            }
            
            Vector3 targetPosition = targetTransform.position;
            targetPosition.z = targetCamera.transform.position.z; // Preserve camera Z
            
            // Calculate the position adjustment needed when zooming
            // When zooming in, we need to move the camera to keep the target in view
            float currentOrthoSize = targetCamera.orthographicSize;
            Vector3 currentPosition = targetCamera.transform.position;
            
            // Calculate how much the camera needs to move to center on the target
            Vector3 positionOffset = targetPosition - currentPosition;
            
            // Create the zoom sequence
            currentZoomSequence = DOTween.Sequence();
            
            // Zoom in phase
            currentZoomSequence.Append(targetCamera.DOOrthoSize(zoomSize, duration).SetEase(zoomInEase));
            currentZoomSequence.Join(targetCamera.transform.DOMove(targetPosition, duration).SetEase(zoomInEase));
            
            // Hold at zoomed state
            if (holdTime > 0f)
            {
                currentZoomSequence.AppendInterval(holdTime);
            }
            
            // Zoom out phase (reset)
            currentZoomSequence.Append(targetCamera.DOOrthoSize(originalOrthoSize, resetTime).SetEase(zoomOutEase));
            currentZoomSequence.Join(targetCamera.transform.DOMove(originalPosition, resetTime).SetEase(zoomOutEase));
            
            // Wait for sequence to complete
            yield return currentZoomSequence.WaitForCompletion();
            
            isZooming = false;
        }
        
        private IEnumerator ZoomToPositionCoroutine(Vector3 targetPosition)
        {
            isZooming = true;
            
            // Wait for delay
            if (delayBeforeZoom > 0f)
            {
                yield return new WaitForSeconds(delayBeforeZoom);
            }
            
            // Store current state if not already stored
            if (originalOrthoSize <= 0f)
            {
                originalOrthoSize = targetCamera.orthographicSize;
                originalPosition = targetCamera.transform.position;
            }
            
            targetPosition.z = targetCamera.transform.position.z; // Preserve camera Z
            
            // Create the zoom sequence
            currentZoomSequence = DOTween.Sequence();
            
            // Zoom in phase
            currentZoomSequence.Append(targetCamera.DOOrthoSize(zoomAmount, zoomDuration).SetEase(zoomInEase));
            currentZoomSequence.Join(targetCamera.transform.DOMove(targetPosition, zoomDuration).SetEase(zoomInEase));
            
            // Hold at zoomed state
            if (holdDuration > 0f)
            {
                currentZoomSequence.AppendInterval(holdDuration);
            }
            
            // Zoom out phase (reset)
            currentZoomSequence.Append(targetCamera.DOOrthoSize(originalOrthoSize, resetDuration).SetEase(zoomOutEase));
            currentZoomSequence.Join(targetCamera.transform.DOMove(originalPosition, resetDuration).SetEase(zoomOutEase));
            
            // Wait for sequence to complete
            yield return currentZoomSequence.WaitForCompletion();
            
            isZooming = false;
        }
        
        private void StopCurrentZoom()
        {
            if (currentZoomSequence != null && currentZoomSequence.IsActive())
            {
                currentZoomSequence.Kill();
            }
            isZooming = false;
        }
        
        private void OnDestroy()
        {
            StopCurrentZoom();
        }
        
        // Public getters for current state
        public bool IsZooming => isZooming;
        public float OriginalOrthoSize => originalOrthoSize;
        public Vector3 OriginalPosition => originalPosition;
    }
}


