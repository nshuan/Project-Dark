using System.Collections.Generic;
using Core;
using UnityEngine;

namespace InGame
{
    public class EnemyBoidManager : MonoSingleton<EnemyBoidManager>
    {
        public EnemySpatialGrid grid;

        protected override void Awake()
        {
            base.Awake();
            gameObject.name = "Enemy Boid Manager";
            
            grid = new EnemySpatialGrid(100, 100, 5);
            LevelManager.Instance.OnLevelPreLoaded += (level) =>
            {
                grid.Clear();
            };
        }
    }
}