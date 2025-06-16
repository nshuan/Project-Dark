using System.Collections.Generic;
using UnityEngine;

namespace Boids
{
    public abstract class FlockBehaviour : ScriptableObject
    {
        public abstract Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock);
    }
}