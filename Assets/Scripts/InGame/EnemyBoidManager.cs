using System.Collections.Generic;
using Core;
using UnityEngine;

namespace InGame
{
    public class EnemyBoidManager : Singleton<EnemyBoidManager>
    {
        public EnemySpatialGrid grid;

        public EnemyBoidManager()
        {
            grid = new EnemySpatialGrid(100, 100, 5);
        }
    }
}