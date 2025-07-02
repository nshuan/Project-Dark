using System;
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

        private Vector2 boidAddition;
        private List<EnemyBoidAgent> neighbors = new List<EnemyBoidAgent>();
        private Vector3 separation;
        private Vector3 alignment;
        private Vector3 cohesion;
        private int alignmentCount = 0;
        private int cohesionCount = 0;
        private Vector2Int currentCell;
        
        // neighbor temp
        private int neighborCount = 0;
        private Vector3 offset;
        private float dist;
        private float cohesionMag;
        private float alignmentMag;

        public Vector2 GetBoidAddition()
        { 
            // if (transform.GetSiblingIndex() == 0)
            //     grid.Clear();
            EnemyBoidManager.Instance.grid.Register(this, ref currentCell);
            
            neighborCount = EnemyBoidManager.Instance.grid.GetNearbyNonAlloc(this, Mathf.Max(separationRadius, alignmentRadius, cohesionRadius), ref currentCell, ref neighbors);

            separation.x = 0; separation.y = 0; separation.z = 0;
            alignment.x = 0; alignment.y = 0; alignment.z = 0;
            cohesion.x = 0; cohesion.y = 0; cohesion.z = 0;
            alignmentCount = 0;
            cohesionCount = 0;

            for (var i = 0; i < neighborCount; i++)
            {
                if (neighbors[i] == this || !neighbors[i].IsActive && Vector3.Distance(transform.position, neighbors[i].transform.position) >= Mathf.Max(separationRadius, alignmentRadius, cohesionRadius)) 
                    continue;
                offset.x = transform.position.x - neighbors[i].transform.position.x;
                offset.y = transform.position.y - neighbors[i].transform.position.y;
                offset.z = transform.position.z - neighbors[i].transform.position.z;
                dist = offset.magnitude;

                if (dist < separationRadius)
                {
                    separation.x += offset.x / dist / dist;
                    separation.y += offset.y / dist / dist;
                    separation.z += offset.z / dist / dist;
                }

                if (dist < alignmentRadius)
                {
                    alignment.x += neighbors[i].transform.forward.x;
                    alignment.y += neighbors[i].transform.forward.y;
                    alignment.z += neighbors[i].transform.forward.z;
                    alignmentCount++;
                }

                if (dist < cohesionRadius)
                {
                    cohesion.x += neighbors[i].transform.position.x;
                    cohesion.y += neighbors[i].transform.position.y;
                    cohesion.z += neighbors[i].transform.position.z;
                    cohesionCount++;
                }
            }

            if (alignmentCount > 0)
            {
                alignment.x /= alignmentCount;
                alignment.y /= alignmentCount;
                alignment.z /= alignmentCount;
            }
            alignmentMag = alignment.magnitude;

            if (cohesionCount > 0)
            {
                cohesion.x = cohesion.x / cohesionCount - transform.position.x;
                cohesion.y = cohesion.y / cohesionCount - transform.position.y;
                cohesion.z = cohesion.z / cohesionCount - transform.position.z;
                cohesionMag = cohesion.magnitude;
                cohesion.x /= cohesionMag;
                cohesion.y /= cohesionMag;
                cohesion.z /= cohesionMag;
            }

            boidAddition.x = separation.x * separationWeight + alignment.x / alignmentMag * alignmentWeight +
                             cohesion.x * cohesionWeight;
            boidAddition.y = separation.y * separationWeight + alignment.y / alignmentMag * alignmentWeight +
                             cohesion.y * cohesionWeight;
            
            return boidAddition.normalized;
        }   
    }
}