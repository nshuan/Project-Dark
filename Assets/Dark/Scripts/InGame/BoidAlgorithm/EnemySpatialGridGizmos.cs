using System;
using UnityEngine;

namespace InGame
{
    public class EnemySpatialGridGizmos : MonoBehaviour
    {
        private EnemySpatialGridWithObstacles grid;
        
        private void Start()
        {
            grid = EnemyBoidManagerWithObstacles.Instance.grid;
        }

        void OnDrawGizmos()
        {
            grid?.DrawGizmos();
        }
    }
}