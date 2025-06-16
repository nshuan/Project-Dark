using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Boids
{
    public class Flock : MonoBehaviour
    {
        public FlockAgent agentPrefab;
        private List<FlockAgent> agents = new List<FlockAgent>();
        public FlockBehaviour behaviour;

        [Range(10, 500)] 
        public int startingCount = 250;
        private const float AgentDensity = 0.08f;

        [Range(1f, 100f)] 
        public float driveFactor = 10f;

        [Range(1f, 100f)] 
        public float maxSpeed = 5f;

        [Range(1f, 10f)] 
        public float neighborRadius = 1.5f;
        
        [Range(0f, 1f)]
        public float avoidanceRadiusMultiplier = 0.5f;

        private float squareMaxSpeed;
        private float squareNeighborRadius;
        private float squareAvoidanceRadius;
        public float SquareAvoidanceRadius => squareAvoidanceRadius;

        private void Start()
        {
            squareMaxSpeed = maxSpeed * maxSpeed;
            squareNeighborRadius = neighborRadius * neighborRadius;
            squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

            for (var i = 0; i < startingCount; i++)
            {
                var newAgent = Instantiate(
                    agentPrefab, 
                    Random.insideUnitCircle * startingCount * AgentDensity,
                    Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)),
                    transform
                    );
                newAgent.name = "Agent " + i;
                agents.Add(newAgent);
            }
        }

        private void Update()
        {
            foreach (var agent in agents)
            {
                var context = GetNearbyObjects(agent);
                var move = behaviour.CalculateMove(agent, context, this);
                move *= driveFactor;
                if (move.sqrMagnitude > squareMaxSpeed)
                {
                    move = move.normalized * maxSpeed;
                }
                agent.Move(move);
            }
        }

        private List<Transform> GetNearbyObjects(FlockAgent agent)
        {
            var context = new List<Transform>();
            var contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighborRadius);
            foreach (var c in contextColliders)
            {
                if (c != agent.AgentCollider)
                {
                    context.Add(c.transform);
                }
            }

            return context;
        }
    }
}