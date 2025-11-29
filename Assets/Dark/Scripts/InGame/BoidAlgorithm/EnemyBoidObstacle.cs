using UnityEngine;

namespace InGame
{
    /// <summary>
    /// Component that marks an object as an obstacle for boid agents.
    /// Any object with this component will be considered an obstacle.
    /// </summary>
    public class EnemyBoidObstacle : MonoBehaviour
    {
        [Tooltip("Radius of the obstacle. Agents will avoid this area.")]
        public float obstacleRadius = 1f;
        
        [Tooltip("Additional avoidance force multiplier.")]
        public float avoidanceStrength = 1f;

        private void OnEnable()
        {
            // Register this obstacle with the manager when enabled
            if (EnemyBoidManagerWithObstacles.Instance != null)
            {
                EnemyBoidManagerWithObstacles.Instance.RegisterObstacle(this);
            }
        }

        private void OnDisable()
        {
            // Unregister this obstacle when disabled
            if (EnemyBoidManagerWithObstacles.Initialized)
            {
                EnemyBoidManagerWithObstacles.Instance.UnregisterObstacle(this);
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Visualize obstacle radius in editor
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, obstacleRadius);
        }
    }
}

