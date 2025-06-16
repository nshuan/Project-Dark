using System.Collections.Generic;
using UnityEngine;

namespace Boids.Behaviours
{
    [CreateAssetMenu(menuName = "Boids/Behaviour/Cohesion")]
    public class CohesionBehaviour : FlockBehaviour
    {
        public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
        {
            // if no neighbours, return no adjustment
            if (context.Count == 0)
                return Vector2.zero;
            
            // add all points together and average
            var cohesionMove = Vector2.zero;
            foreach (var item in context)
            {
                cohesionMove += (Vector2)item.position;
            }
            cohesionMove /= context.Count;
            
            // create offset from agent position
            cohesionMove -= (Vector2)agent.transform.position;
            
            return cohesionMove;
        }
    }
}