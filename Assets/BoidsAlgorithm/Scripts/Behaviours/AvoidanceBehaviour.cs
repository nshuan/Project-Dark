using System.Collections.Generic;
using UnityEngine;

namespace Boids.Behaviours
{
    [CreateAssetMenu(menuName = "Boids/Behaviour/Avoidance")]
    public class AvoidanceBehaviour : FilterFlockBehaviour
    {
        public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
        {
            // if no neighbours, return no adjustment
            if (context.Count == 0)
                return Vector2.zero;
            
            // add all points together and average
            var avoidanceMove = Vector2.zero;
            var nAvoid = 0;
            var filteredContext = filter ? context : filter.Filter(agent, context);
            foreach (var item in filteredContext)
            {
                if (Vector2.SqrMagnitude(item.position - agent.transform.position) < flock.SquareAvoidanceRadius)
                {
                    nAvoid += 1;
                    avoidanceMove += (Vector2)(agent.transform.position - item.position);
                }
            }

            if (nAvoid > 0)
                avoidanceMove /= nAvoid;
            
            return avoidanceMove;
        }
    }
}