using UnityEngine;

namespace InGame.MapBoundary
{
    public class MapBoundaryBox : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D box;
        
        public bool ContainPoint(Vector2 point)
        {
            if (!box.bounds.Contains(point)) return false;
            
            Transform t = box.transform;

            // Translate point into local space manually
            var diffX = point.x - t.position.x;
            var diffY = point.y - t.position.y;

            // Undo rotation
            float angle = -t.eulerAngles.z * Mathf.Deg2Rad;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);
            float localPointX = diffX * cos - diffY * sin;
            float localPointY = diffX * sin + diffY * cos;

            // Undo scale
            localPointX /= t.lossyScale.x;
            localPointY /= t.lossyScale.y;

            // Now compare with collider local box
            float halfSizeX = box.size.x * 0.5f;
            float halfSizeY = box.size.y * 0.5f;

            if (Mathf.Abs(localPointX - box.offset.x) <= halfSizeX &&
                Mathf.Abs(localPointY - box.offset.y) <= halfSizeY)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}