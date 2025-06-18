using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Boids
{
    public abstract class FlockBehaviour : SerializedScriptableObject
    {
        public abstract Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock);
    }
}