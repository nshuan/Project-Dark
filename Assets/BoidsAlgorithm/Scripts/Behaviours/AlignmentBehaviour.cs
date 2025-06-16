using System.Collections.Generic;
using UnityEngine;

namespace Boids.Behaviours
{
    [CreateAssetMenu(menuName = "Boids/Behaviour/Alignment")]
    public class AlignmentBehaviour : FlockBehaviour
    {
        public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
        {
            // if no neighbours, maintain current alignment
            if (context.Count == 0)
                return agent.transform.up;
            
            // add all points together and average
            var alignmentMove = Vector2.zero;
            foreach (var item in context)
            {
                alignmentMove += (Vector2)item.transform.up;
            }
            alignmentMove /= context.Count;

            return alignmentMove;
        }
    }
}