using System;
using UnityEngine;

namespace Boids
{
    [RequireComponent(typeof(Collider2D))]
    public class FlockAgent : MonoBehaviour
    {
        private Flock agentFlock;
        public Flock AgentFlock => agentFlock;
        
        private Collider2D agentCollider;
        public Collider2D AgentCollider => agentCollider;

        private void Awake()
        {
            agentCollider = GetComponent<Collider2D>();
        }

        public void Initialize(Flock flock)
        {
            agentFlock = flock;
        }

        public void Move(Vector2 velocity)
        {
            transform.up = velocity;
            transform.position += (Vector3)velocity * Time.deltaTime;
        }
    }
}