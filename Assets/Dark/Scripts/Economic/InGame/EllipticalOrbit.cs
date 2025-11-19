using System;
using System.Collections;
using UnityEngine;

namespace Economic.InGame
{
    public class EllipticalOrbit : MonoBehaviour
    {
        [Header("Ellipse Settings")]
        [SerializeField] private float width = 1f;
        [SerializeField] private float height = 1f;
        [SerializeField] private float orbitSpeed = 1f;
        [SerializeField] private Vector3 centerOffset = Vector3.zero;

        [Header("Axes")]
        [SerializeField] private Vector3 ellipseAxisX = Vector3.right;
        [SerializeField] private Vector3 ellipseAxisY = Vector3.forward;

        [Header("Space Settings")]
        [SerializeField] private bool useLocalSpace = true;

        [Header("Initial Transition")]
        [SerializeField] private float transitionDuration = 1f; // if < 0, no transition, just pick a random angle and start orbiting

        [Header("Gizmos")]
        [SerializeField] private bool showGizmos = true;
        [SerializeField] private Color gizmoColor = Color.yellow;
        [SerializeField] private int gizmoSegments = 64;

        private Vector3 _basePosition;
        private float _angle;
        private bool _isTransitioning = false;
        private bool _activated = false;

        // private void Awake()
        // {
        //     CacheBasePosition();
        //     _angle = 0f;
        // }
        //
        // private void OnEnable()
        // {
        //     CacheBasePosition();
        //     StartInitialTransition();
        // }

        private void OnDestroy()
        {
            _activated = false;
        }

        private void StartInitialTransition()
        {
            if (transitionDuration <= 0f)
            {
                // No transition, just pick a random angle and start orbiting
                _angle = RandomUtil.Range(0f, Mathf.PI * 2f);
                _isTransitioning = false;
                return;
            }

            // Store the starting position
            Vector3 startPosition = useLocalSpace ? transform.localPosition : transform.position;
            
            // Calculate the target position on the orbit
            var target = GetRandomPositionOnOrbit();

            // Start the transition coroutine
            StartCoroutine(TransitionToOrbitPosition(startPosition, target.Item2, target.Item1));
        }

        private IEnumerator TransitionToOrbitPosition(Vector3 startPosition, Vector3 targetPosition, float targetAngle)
        {
            _isTransitioning = true;
            float elapsedTime = 0f;

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / transitionDuration);
                
                // Use smooth interpolation
                t = t * t * (3f - 2f * t); // Smoothstep

                Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);

                if (useLocalSpace)
                    transform.localPosition = currentPosition;
                else
                    transform.position = currentPosition;

                yield return null;
            }

            // Ensure we're exactly at the target position
            if (useLocalSpace)
                transform.localPosition = targetPosition;
            else
                transform.position = targetPosition;

            // Set the angle to start orbiting from the target position
            _angle = targetAngle;
            _isTransitioning = false;
        }

        private void Update()
        {
            if (!_activated) return;
            if (_isTransitioning)
                return;

            _angle += Time.deltaTime * orbitSpeed;

            var axisX = ellipseAxisX.sqrMagnitude < Mathf.Epsilon ? Vector3.right : ellipseAxisX.normalized;
            var axisY = ellipseAxisY.sqrMagnitude < Mathf.Epsilon ? Vector3.forward : ellipseAxisY.normalized;

            var offset = axisX * (Mathf.Cos(_angle) * width) +
                         axisY * (Mathf.Sin(_angle) * height);

            if (useLocalSpace)
                transform.localPosition = _basePosition + centerOffset + offset;
            else
                transform.position = _basePosition + centerOffset + offset;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            _isTransitioning = false;
            
            if (useLocalSpace)
                transform.localPosition = _basePosition + centerOffset;
            else
                transform.position = _basePosition + centerOffset;
            _angle = 0f;
        }

        public void ResetOrbit()
        {
            StopAllCoroutines();
            _isTransitioning = false;
            CacheBasePosition();
            _angle = 0f;
            _activated = false;
        }

        public void StartOrbit()
        {
            StartInitialTransition();
            _activated = true;
        }

        private void CacheBasePosition()
        {
            _basePosition = useLocalSpace ? transform.localPosition : transform.position;
        }

        // Be careful that the value will ignore useLocalSpace and do not auto handle all cases
        public void OverrideBasePosition(Vector3 newBasePosition)
        {
            _basePosition = newBasePosition;
        }

        /// <summary>
        /// Return (target angle, target position)
        /// </summary>
        /// <returns></returns>
        public (float, Vector3) GetRandomPositionOnOrbit()
        {
            // Pick a random angle for the target orbit position
            float targetAngle = RandomUtil.Range(0f, Mathf.PI * 2f);

            // Calculate the target position on the orbit
            var axisX = ellipseAxisX.sqrMagnitude < Mathf.Epsilon ? Vector3.right : ellipseAxisX.normalized;
            var axisY = ellipseAxisY.sqrMagnitude < Mathf.Epsilon ? Vector3.forward : ellipseAxisY.normalized;
            var targetOffset = axisX * (Mathf.Cos(targetAngle) * width) +
                               axisY * (Mathf.Sin(targetAngle) * height);
            return (targetAngle, _basePosition + centerOffset + targetOffset);
        }
        
        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            if (!showGizmos) return;

            DrawEllipsePath();
        }

        private void DrawEllipsePath()
        {
            Gizmos.color = gizmoColor;

            // Get current base position (works in both play and edit mode)
            Vector3 currentBasePos = useLocalSpace ? transform.localPosition : transform.position;
            
            // Calculate center position
            Vector3 center;
            Vector3 axisX;
            Vector3 axisY;

            if (useLocalSpace)
            {
                // For local space, calculate center in local space first
                Vector3 localCenter = currentBasePos + centerOffset;
                
                // Transform to world space for gizmo drawing
                if (transform.parent != null)
                {
                    center = transform.parent.TransformPoint(localCenter);
                    axisX = transform.parent.TransformDirection(ellipseAxisX.normalized);
                    axisY = transform.parent.TransformDirection(ellipseAxisY.normalized);
                }
                else
                {
                    center = localCenter;
                    axisX = ellipseAxisX.sqrMagnitude < Mathf.Epsilon ? Vector3.right : ellipseAxisX.normalized;
                    axisY = ellipseAxisY.sqrMagnitude < Mathf.Epsilon ? Vector3.forward : ellipseAxisY.normalized;
                }
            }
            else
            {
                center = currentBasePos + centerOffset;
                axisX = ellipseAxisX.sqrMagnitude < Mathf.Epsilon ? Vector3.right : ellipseAxisX.normalized;
                axisY = ellipseAxisY.sqrMagnitude < Mathf.Epsilon ? Vector3.forward : ellipseAxisY.normalized;
            }

            // Ensure axes are normalized
            if (axisX.sqrMagnitude < Mathf.Epsilon) axisX = Vector3.right;
            if (axisY.sqrMagnitude < Mathf.Epsilon) axisY = Vector3.forward;
            axisX = axisX.normalized;
            axisY = axisY.normalized;

            // Draw ellipse outline
            Vector3 previousPoint = center + axisX * width;
            for (int i = 1; i <= gizmoSegments; i++)
            {
                float angle = (float)i / gizmoSegments * Mathf.PI * 2f;
                Vector3 offset = axisX * (Mathf.Cos(angle) * width) + 
                                axisY * (Mathf.Sin(angle) * height);
                Vector3 currentPoint = center + offset;
                
                Gizmos.DrawLine(previousPoint, currentPoint);
                previousPoint = currentPoint;
            }

            // Draw center point
            Gizmos.color = gizmoColor * 0.5f;
            Gizmos.DrawWireSphere(center, 0.1f);
        }

        #endregion
    }
}

