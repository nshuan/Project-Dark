using System.Collections.Generic;
using UnityEngine;

namespace InGame
{
    public class EnemyBoidAgent : MonoBehaviour
    {
        public bool IsActive { get; set; }
        public float separationRadius = 2f;
        public float alignmentRadius = 4f;
        public float cohesionRadius = 4f;

        public float separationWeight = 1.5f;
        public float alignmentWeight = 1f;
        public float cohesionWeight = 1f;

        public Vector2 GetBoidAddition()
        {
            var grid = EnemyBoidManager.Instance.grid;

            if (transform.GetSiblingIndex() == 0)
                grid.Clear();
            grid.Register(this);

            List<EnemyBoidAgent> neighbors = grid.GetNearby(this, Mathf.Max(separationRadius, alignmentRadius, cohesionRadius));

            Vector3 separation = Vector3.zero;
            Vector3 alignment = Vector3.zero;
            Vector3 cohesion = Vector3.zero;
            int alignmentCount = 0, cohesionCount = 0;

            foreach (var neighbor in neighbors)
            {
                Vector3 offset = transform.position - neighbor.transform.position;
                float dist = offset.magnitude;

                if (dist < separationRadius)
                    separation += offset.normalized / dist;

                if (dist < alignmentRadius)
                {
                    alignment += neighbor.transform.forward;
                    alignmentCount++;
                }

                if (dist < cohesionRadius)
                {
                    cohesion += neighbor.transform.position;
                    cohesionCount++;
                }
            }

            if (alignmentCount > 0)
                alignment /= alignmentCount;

            if (cohesionCount > 0)
            {
                cohesion /= cohesionCount;
                cohesion = (cohesion - transform.position).normalized;
            }

            Vector3 directionAdder =
                separation * separationWeight +
                alignment.normalized * alignmentWeight +
                cohesion * cohesionWeight;

            return directionAdder.normalized;
        }   
    }
}