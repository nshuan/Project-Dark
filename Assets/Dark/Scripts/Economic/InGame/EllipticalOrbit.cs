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

        [Header("Gizmos")]
        [SerializeField] private bool showGizmos = true;
        [SerializeField] private Color gizmoColor = Color.yellow;
        [SerializeField] private int gizmoSegments = 64;

        private Vector3 _basePosition;
        private float _angle;

        private void Awake()
        {
            CacheBasePosition();
            _angle = 0f;
        }

        private void OnEnable()
        {
            CacheBasePosition();
        }

        private void Update()
        {
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
            if (useLocalSpace)
                transform.localPosition = _basePosition + centerOffset;
            else
                transform.position = _basePosition + centerOffset;
            _angle = 0f;
        }

        public void ResetOrbit()
        {
            CacheBasePosition();
            _angle = 0f;
        }

        private void CacheBasePosition()
        {
            _basePosition = useLocalSpace ? transform.localPosition : transform.position;
        }

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
    }
}

