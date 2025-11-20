using System;
using System.Collections.Generic;
using UnityEngine;

namespace InGame
{
    /// <summary>
    /// Extended version of EnemyBoidAgent with obstacle avoidance support.
    /// This script does not modify the original EnemyBoidAgent.
    /// </summary>
    public class EnemyBoidAgentWithObstacles : MonoBehaviour
    {
        public bool IsActive { get; set; }
        public float separationRadius = 2f;
        public float alignmentRadius = 4f;
        public float cohesionRadius = 4f;

        public float separationWeight = 1.5f;
        public float alignmentWeight = 1f;
        public float cohesionWeight = 1f;
        
        [Header("Obstacle Avoidance")]
        [Tooltip("Radius to detect obstacles")]
        public float obstacleDetectionRadius = 3f;
        
        [Tooltip("Weight of obstacle avoidance force")]
        public float obstacleAvoidanceWeight = 2f;
        
        private List<EnemyBoidAgentWithObstacles> neighbors = new List<EnemyBoidAgentWithObstacles>();
        private Vector3 separation = new Vector3();
        private Vector3 alignment = new Vector3();
        private Vector3 cohesion = new Vector3();
        private Vector3 obstacleAvoidance = new Vector3();
        private int alignmentCount = 0;
        private int cohesionCount = 0;
        private Vector2Int currentCell = new Vector2Int();
        private Vector3 position = new Vector3();
        
        // neighbor temp
        private int neighborCount = 0;
        private Vector3 offset = new Vector3();
        private float dist;
        private float cohesionMag;
        private float alignmentMag;
        
        // cooldown between 2 boid 
        public int boidCooldownStep = 2;
        private int boidCdCounter;

        // Obstacle detection temp variables
        private List<EnemyBoidObstacle> nearbyObstacles = new List<EnemyBoidObstacle>();
        private Vector3 obstacleOffset = new Vector3();
        private float obstacleDist;
        private float obstacleAvoidanceMag;

        public void GetBoidAdditionNonAlloc(ref Vector2 addition)
        {
            if (boidCdCounter > 0)
            {
                boidCdCounter -= 1;
                addition.x = 0;
                addition.y = 0;
                return;
            }
            
            EnemyBoidManagerWithObstacles.Instance.grid.Register(this, ref currentCell);
            
            neighborCount = EnemyBoidManagerWithObstacles.Instance.grid.GetNearbyNonAlloc(this, Mathf.Max(separationRadius, alignmentRadius, cohesionRadius), ref currentCell, ref neighbors);

            separation.x = 0; separation.y = 0; separation.z = 0;
            alignment.x = 0; alignment.y = 0; alignment.z = 0;
            cohesion.x = 0; cohesion.y = 0; cohesion.z = 0;
            obstacleAvoidance.x = 0; obstacleAvoidance.y = 0; obstacleAvoidance.z = 0;
            alignmentCount = 0;
            cohesionCount = 0;
            position.x = transform.position.x;
            position.y = transform.position.y;
            position.z = transform.position.z;

            // Standard boid behaviors
            for (var i = 0; i < neighborCount; i++)
            {
                if (neighbors[i] == this || !neighbors[i].IsActive && Vector3.Distance(transform.position, neighbors[i].transform.position) >= Mathf.Max(separationRadius, alignmentRadius, cohesionRadius)) 
                    continue;
                offset.x = position.x - neighbors[i].transform.position.x;
                offset.y = position.y - neighbors[i].transform.position.y;
                offset.z = position.z - neighbors[i].transform.position.z;
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
                alignmentMag = alignment.magnitude;
            }
            else alignmentMag = 1;

            if (cohesionCount > 0)
            {
                cohesion.x = cohesion.x / cohesionCount - position.x;
                cohesion.y = cohesion.y / cohesionCount - position.y;
                cohesion.z = cohesion.z / cohesionCount - position.z;
                cohesionMag = cohesion.magnitude;
                cohesion.x /= cohesionMag;
                cohesion.y /= cohesionMag;
                cohesion.z /= cohesionMag;
            }

            // Obstacle avoidance
            CalculateObstacleAvoidance();

            addition.x = separation.x * separationWeight + alignment.x / alignmentMag * alignmentWeight +
                             cohesion.x * cohesionWeight + obstacleAvoidance.x * obstacleAvoidanceWeight;
            addition.y = separation.y * separationWeight + alignment.y / alignmentMag * alignmentWeight +
                             cohesion.y * cohesionWeight + obstacleAvoidance.y * obstacleAvoidanceWeight;
            
            boidCdCounter = boidCooldownStep;
        }

        private void CalculateObstacleAvoidance()
        {
            if (EnemyBoidManagerWithObstacles.Instance == null)
                return;

            // Get nearby obstacles from manager
            EnemyBoidManagerWithObstacles.Instance.GetNearbyObstaclesNonAlloc(
                transform.position, 
                obstacleDetectionRadius, 
                ref nearbyObstacles
            );

            // Calculate avoidance force from each nearby obstacle
            for (int i = 0; i < nearbyObstacles.Count; i++)
            {
                var obstacle = nearbyObstacles[i];
                if (!obstacle || !obstacle.gameObject.activeInHierarchy)
                    continue;

                obstacleOffset.x = position.x - obstacle.transform.position.x;
                obstacleOffset.y = position.y - obstacle.transform.position.y;
                obstacleOffset.z = position.z - obstacle.transform.position.z;
                obstacleDist = obstacleOffset.magnitude;

                // Calculate effective radius (obstacle radius + detection radius)
                float effectiveRadius = obstacle.obstacleRadius + obstacleDetectionRadius;

                if (obstacleDist < effectiveRadius && obstacleDist > 0.01f)
                {
                    // Normalize and apply inverse square law for stronger avoidance when closer
                    float avoidanceFactor = 1f / (obstacleDist * obstacleDist);
                    float strength = obstacle.avoidanceStrength * avoidanceFactor;
                    
                    obstacleAvoidance.x += obstacleOffset.x / obstacleDist * strength;
                    obstacleAvoidance.y += obstacleOffset.y / obstacleDist * strength;
                    obstacleAvoidance.z += obstacleOffset.z / obstacleDist * strength;
                }
            }

            // Normalize obstacle avoidance vector
            obstacleAvoidanceMag = obstacleAvoidance.magnitude;
            if (obstacleAvoidanceMag > 0.01f)
            {
                obstacleAvoidance.x /= obstacleAvoidanceMag;
                obstacleAvoidance.y /= obstacleAvoidanceMag;
            }
        }

        private void OnDrawGizmos()
        {
            if (nearbyObstacles == null || nearbyObstacles.Count == 0) return;

            foreach (var obstacle in nearbyObstacles)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, obstacle.transform.position);
            }
        }
    }
}

