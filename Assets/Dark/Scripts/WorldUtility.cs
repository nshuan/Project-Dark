using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class WorldUtility
    {
        public static Vector2 GetWorldSize(Camera cam, RectTransform rectTransform)
        {
            var worldCorners = new Vector3[4];
            rectTransform.GetWorldCorners(worldCorners);
            worldCorners = worldCorners.Select(cam.ScreenToWorldPoint).ToArray();

            // Width = distance between bottom-left and bottom-right
            var width = Vector3.Distance(worldCorners[0], worldCorners[3]); // or [0] and [1] depending on orientation
            var height = Vector3.Distance(worldCorners[0], worldCorners[1]);

            return new Vector2(width, height);
        }
    }
}