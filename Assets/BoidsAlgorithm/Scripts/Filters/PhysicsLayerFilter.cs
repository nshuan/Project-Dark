using System.Collections.Generic;
using UnityEngine;

namespace Boids.Filters
{
    [CreateAssetMenu(menuName = "Boids/Filter/Physics Layer")]
    public class PhysicsLayerFilter : ContextFilter
    {
        public LayerMask mask;
        
        public override List<Transform> Filter(FlockAgent agent, List<Transform> original)
        {
            var filtered = new List<Transform>();
            foreach (var item in original)
            {
                if (mask == (mask | (1 << item.gameObject.layer)))
                {
                    filtered.Add(item);
                }
            }
            
            return filtered;
        }
    }
}