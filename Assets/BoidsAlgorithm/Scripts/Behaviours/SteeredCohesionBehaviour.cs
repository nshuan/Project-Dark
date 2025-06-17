using UnityEngine;
using System.Collections.Generic;

namespace Boids.Behaviours
{
    [CreateAssetMenu(menuName = "Boids/Behaviour/SteeredCohesion")]
    public class SteeredCohesionBehaviour : FilterFlockBehaviour
    {
        public float agentSmoothTime = 0.5f;
        private Vector2 currentVelocity;
        
        public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
        {
            // if no neighbours, return no adjustment
            if (context.Count == 0)
                return Vector2.zero;
            
            // add all points together and average
            var cohesionMove = Vector2.zero;
            var filteredContext = filter ? context : filter.Filter(agent, context);
            foreach (var item in filteredContext)
            {
                cohesionMove += (Vector2)item.position;
            }
            cohesionMove /= context.Count;
            
            // create offset from agent position
            cohesionMove -= (Vector2)agent.transform.position;
            cohesionMove = Vector2.SmoothDamp(agent.transform.up, cohesionMove, ref currentVelocity, agentSmoothTime);
            
            return cohesionMove;
        }
    }
}