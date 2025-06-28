using System;
using UnityEngine;

namespace InGame
{
    public class EnemySpatialGridGizmos : MonoBehaviour
    {
        private EnemySpatialGrid grid;
        
        private void Start()
        {
            grid = EnemyBoidManager.Instance.grid;
        }

        void OnDrawGizmos()
        {
            grid?.DrawGizmos();
        }
    }
}