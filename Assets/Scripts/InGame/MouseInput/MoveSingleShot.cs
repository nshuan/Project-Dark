using UnityEngine;

namespace InGame.MouseInput
{
    public class MoveSingleShot : IMouseInput
    {
        private Camera Cam { get; set; }
        private Transform cursor;
        
        public MoveSingleShot(Camera cam, Transform cursor)
        {
            Cam = cam;
            this.cursor = cursor;
        }
        
        public void OnMouseClick()
        {
            
        }

        public void OnUpdate()
        {
            var mouseWorldPosition = Cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0; // Set z to 0 for 2D
            cursor.position = mouseWorldPosition;
        }
    }
}