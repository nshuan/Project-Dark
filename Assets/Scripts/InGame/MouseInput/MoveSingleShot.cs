using System.Linq;
using DefaultNamespace;
using UnityEngine;

namespace InGame.MouseInput
{
    public class MoveSingleShot : IMouseInput
    {
        private Camera Cam { get; set; }
        private RectTransform cursor;
        private Vector3 mousePosition;
        
        public MoveSingleShot(Camera cam, RectTransform cursor)
        {
            Cam = cam;
            this.cursor = cursor;
        }
        
        public void OnMouseClick()
        {
            var mousePos = Cam.ScreenToWorldPoint(mousePosition);
            var hit = Physics2D.BoxCast(mousePos, WorldUtility.GetWorldSize(Cam, cursor), 0, Vector2.zero, 0f, LayerMask.GetMask("Entity"));

            if (hit.collider != null && hit.transform.TryGetComponent<EnemyEntity>(out var enemyEntity))
            {
                enemyEntity.OnHit();
            }
        }

        public void OnUpdate()
        {
            mousePosition = Input.mousePosition;
            mousePosition.z = 0; // Set z to 0 for 2D
            cursor.position = mousePosition;
            
#if UNITY_EDITOR
            var corners = new Vector3[4];
            cursor.GetWorldCorners(corners);
            corners = corners.Select((corner) => Cam.ScreenToWorldPoint(corner)).ToArray();
            
            // Draw lines between corners to visualize the box
            Debug.DrawLine(corners[0], corners[1], Color.red); // Bottom Left -> Top Left
            Debug.DrawLine(corners[1], corners[2], Color.red); // Top Left -> Top Right
            Debug.DrawLine(corners[2], corners[3], Color.red); // Top Right -> Bottom Right
            Debug.DrawLine(corners[3], corners[0], Color.red); // Bottom Right -> Bottom Left
            
            var ray = Cam.ScreenPointToRay(mousePosition);

            // Default: draw a ray forward
            // Raydistance = 100f
            var rayEnd = ray.origin + ray.direction * 100f;

            // Draw the ray in Scene view
            Debug.DrawLine(ray.origin, rayEnd, Color.green);
#endif
        }

        public void OnDrawGizmos()
        {
            DrawBoxGizmo(Cam.ScreenToWorldPoint(mousePosition), WorldUtility.GetWorldSize(Cam, cursor), 0f, Color.cyan);
        }
        
        private void DrawBoxGizmo(Vector2 center, Vector2 size, float angle, Color color)
        {
            var half = size / 2f;

            var rot = Quaternion.Euler(0, 0, angle);
            var corners = new Vector2[4]
            {
                center + new Vector2(-half.x, -half.y),
                center + new Vector2(-half.x,  half.y),
                center + new Vector2( half.x,  half.y),
                center + new Vector2( half.x, -half.y)
            };

            Debug.DrawLine(corners[0], corners[1], color, 1f);
            Debug.DrawLine(corners[1], corners[2], color, 1f);
            Debug.DrawLine(corners[2], corners[3], color, 1f);
            Debug.DrawLine(corners[3], corners[0], color, 1f);
        }
    }
}