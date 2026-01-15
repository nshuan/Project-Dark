using System.Collections.Generic;
using Core;
using UnityEngine;

namespace InGame
{
    /// <summary>
    /// Extended version of EnemyBoidManager with obstacle tracking support.
    /// This script does not modify the original EnemyBoidManager.
    /// </summary>
    public class EnemyBoidManagerWithObstacles : MonoSingleton<EnemyBoidManagerWithObstacles>
    {
        public EnemySpatialGridWithObstacles grid;
        
        private List<EnemyBoidObstacle> allObstacles = new List<EnemyBoidObstacle>();
        private List<EnemyBoidObstacle> tempObstacleList = new List<EnemyBoidObstacle>();

        protected override void Awake()
        {
            base.Awake();
            gameObject.name = "Enemy Boid Manager With Obstacles";
            
            grid = new EnemySpatialGridWithObstacles(100, 100, 5);
            LevelManager.Instance.OnLevelPreLoaded += (level) =>
            {
                grid.Clear();
                allObstacles.Clear();
            };
        }

        /// <summary>
        /// Register an obstacle to be tracked by the manager.
        /// </summary>
        public void RegisterObstacle(EnemyBoidObstacle obstacle)
        {
            if (obstacle != null && !allObstacles.Contains(obstacle))
            {
                allObstacles.Add(obstacle);
            }
        }

        /// <summary>
        /// Unregister an obstacle from the manager.
        /// </summary>
        public void UnregisterObstacle(EnemyBoidObstacle obstacle)
        {
            if (obstacle != null)
            {
                allObstacles.Remove(obstacle);
            }
        }

        /// <summary>
        /// Get all obstacles within a radius of a position (non-allocating).
        /// </summary>
        public void GetNearbyObstaclesNonAlloc(Vector3 position, float radius, ref List<EnemyBoidObstacle> result)
        {
            result.Clear();
            
            for (int i = 0; i < allObstacles.Count; i++)
            {
                var obstacle = allObstacles[i];
                if (!obstacle || !obstacle.gameObject.activeInHierarchy)
                    continue;

                float distSquared = (position - obstacle.transform.position).sqrMagnitude;
                float effectiveRadius = obstacle.obstacleRadius + radius;
                
                if (distSquared <= effectiveRadius * effectiveRadius)
                {
                    result.Add(obstacle);
                }
            }
        }

        /// <summary>
        /// Get all registered obstacles.
        /// </summary>
        public List<EnemyBoidObstacle> GetAllObstacles()
        {
            return allObstacles;
        }
    }
}

