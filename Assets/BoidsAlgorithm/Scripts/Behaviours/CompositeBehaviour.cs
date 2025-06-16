using System;
using System.Collections.Generic;
using UnityEngine;

namespace Boids.Behaviours
{
    [CreateAssetMenu(menuName = "Boids/Behaviour/Composite")]
    public class CompositeBehaviour : FlockBehaviour
    {
        public FlockBehaviourInfo[] behaviours;
        
        public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
        {
            // Setup move
            var move = Vector2.zero;

            for (var i = 0; i < behaviours.Length; i++)
            {
                var partialMove = behaviours[i].behaviour.CalculateMove(agent, context, flock) * behaviours[i].weight;

                if (partialMove != Vector2.zero)
                {
                    if (partialMove.sqrMagnitude > behaviours[i].weight * behaviours[i].weight)
                    {
                        partialMove.Normalize();
                        partialMove *= behaviours[i].weight;
                    }
                    
                    move += partialMove;
                }
            }

            return move;
        }
    }

    [Serializable]
    public class FlockBehaviourInfo
    {
        public FlockBehaviour behaviour;
        public float weight;
    }
}