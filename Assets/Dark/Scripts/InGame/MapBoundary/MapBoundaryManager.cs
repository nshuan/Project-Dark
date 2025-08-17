using System.Linq;
using Core;
using UnityEngine;

namespace InGame.MapBoundary
{
    public class MapBoundaryManager : MonoSingleton<MapBoundaryManager>
    {
        [SerializeField] private MapBoundaryBox[] allBox;

        public bool ContainPoint(Vector2 point)
        {
            return allBox.Any((box) => box.ContainPoint(point));
        }
    }
}