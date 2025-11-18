using System;
using UnityEngine;

namespace InGame.EnemyVisualBody
{
    public class EnemyBodyHitSpot : MonoBehaviour
    {
        public float range = 0.5f;

        public void DrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}