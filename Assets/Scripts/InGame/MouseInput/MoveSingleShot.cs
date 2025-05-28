using System.Linq;
using UnityEngine;

namespace InGame.MouseInput
{
    public class MoveSingleShot : IMouseInput
    {
        private Camera Cam { get; set; }
        private RectTransform cursor;
        
        public MoveSingleShot(Camera cam, RectTransform cursor)
        {
            Cam = cam;
            this.cursor = cursor;
        }
        
        public void OnMouseClick()
        {
            
        }

        public void OnUpdate()
        {
            var mousePosition = Input.mousePosition;
            mousePosition.z = 0; // Set z to 0 for 2D
            cursor.position = mousePosition;
            
            var corners = new Vector3[4];
            cursor.GetWorldCorners(corners);
            corners = corners.Select((corner) => Cam.ScreenToWorldPoint(corner)).ToArray();
            
#if UNITY_EDITOR
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
        
        
    }
}