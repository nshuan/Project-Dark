using System.Collections.Generic;
using Core;
using UnityEngine;

namespace InGame
{
    public class EnemyBoidManager : Singleton<EnemyBoidManager>
    {
        public List<Transform> targets;
        public List<EnemyBoidAgent> boidAgents = new List<EnemyBoidAgent>();
        public EnemySpatialGrid grid;

        public EnemyBoidManager()
        {
            grid = new EnemySpatialGrid(100, 100, 5);
        }
        
        public void RegisterAgent(EnemyBoidAgent agent)
        {
            if (!boidAgents.Contains(agent))
                boidAgents.Add(agent);
        }
    }
}