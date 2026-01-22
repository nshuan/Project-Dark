using System;
using UnityEngine;

namespace InGame
{
    public class GizmosTowerConnection : MonoBehaviour
    {
        public TowerEntity[] towers;

        private void OnDrawGizmos()
        {
            if (towers == null) return;

            for (var i = 0; i < towers.Length; i++)
            {
                for (var j = i + 1; j < towers.Length; j++)
                {
                    Gizmos.DrawLine(towers[i].GetBaseCenter(), towers[j].GetBaseCenter());
                }
            }
        }
    }
}