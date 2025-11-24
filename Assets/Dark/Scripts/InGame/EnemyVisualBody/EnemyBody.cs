using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace InGame.EnemyVisualBody
{
    public class EnemyBody : MonoBehaviour
    {
        [SerializeField] private List<EnemyBodyHitSpot> hitSpots;
        
        public void SetupProjectileHit(Transform projectile, Vector2 direction)
        {
            var minDistance = 100f;
            var nearestSpot = hitSpots[0];

            foreach (var spot in hitSpots)
            {
                var distance = DistancePointToVector(spot.transform.position, projectile.position, direction);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestSpot = spot;
                }
            }
            
            if ((projectile.position.x - nearestSpot.transform.position.x) * (-direction.x) < 0) direction = -direction;
            var targetPosition = projectile.position + (Vector3)direction.normalized * Mathf.Sqrt(
                Mathf.Pow((nearestSpot.transform.position - projectile.transform.position).magnitude, 2) - Mathf.Pow(minDistance, 2));
            if (minDistance > nearestSpot.range)
                targetPosition = nearestSpot.transform.position +
                                 (targetPosition - nearestSpot.transform.position).normalized * nearestSpot.range;

            projectile.position = targetPosition;
        }

        #region Math

        private float DistancePointToVector(Vector3 point, Vector3 lineOrigin, Vector3 lineDir)
        {
            return Vector3.Cross(point - lineOrigin, lineDir).magnitude / lineDir.magnitude;
        }

        #endregion
        
        [Button]
        private void AddNewSpot()
        {
            hitSpots ??= new List<EnemyBodyHitSpot>();
            var newSpot = new GameObject() { name = $"HitSpot_{hitSpots.Count}" };
            newSpot.transform.SetParent(transform);
            newSpot.transform.localPosition = Vector3.zero;
            var newSpotComponent = newSpot.AddComponent<EnemyBodyHitSpot>();
            hitSpots.Add(newSpotComponent);
        }

        [Button]
        private void Refresh()
        {
            hitSpots = GetComponentsInChildren<EnemyBodyHitSpot>().ToList();
            for (var i = 0; i < hitSpots.Count; i++)
            {
                hitSpots[i].gameObject.name = $"HitSpot_{i}";
            }
        }
        
        [SerializeField] private bool showGizmos;
        private void OnDrawGizmos()
        {
            if (hitSpots == null) return;
            if (showGizmos)
            {
                foreach (var spot in hitSpots)
                {
                    spot.DrawGizmos();
                }
            }
        }
    }
}