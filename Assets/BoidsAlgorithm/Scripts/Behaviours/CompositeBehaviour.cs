using System.Collections.Generic;
using UnityEngine;

namespace Boids.Behaviours
{
    [CreateAssetMenu(menuName = "Boids/Behaviour/Composite")]
    public class CompositeBehaviour : FlockBehaviour
    {
        public FlockBehaviour[] behaviours;
        public float[] weights;
        
        public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
        {
            if (weights.Length != behaviours.Length)
            {
                Debug.LogWarning("Weights count mismatch");
                return Vector2.zero;
            }
            
            // Setup move
            var move = Vector2.zero;

            for (var i = 0; i < behaviours.Length; i++)
            {
                var partialMove = behaviours[i].CalculateMove(agent, context, flock) * weights[i];

                if (partialMove != Vector2.zero)
                {
                    if (partialMove.sqrMagnitude > weights[i] * weights[i] * weights[i])
                    {
                        partialMove.Normalize();
                        partialMove *= weights[i];
                    }
                    
                    move += partialMove;
                }
            }

            return move;
        }
    }
}