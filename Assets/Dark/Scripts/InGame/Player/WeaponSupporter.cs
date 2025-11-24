using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;

namespace InGame
{
    public class WeaponSupporter : MonoBehaviour
    {
        [SerializeField] private LayerMask enemyLayer;
        
        private static RaycastHit2D[] enemiesInRange = new RaycastHit2D[30];
        public static RaycastHit2D[] EnemiesInRange => enemiesInRange;
        public static int EnemiesCountInRange { get; set; }
        public static int EnemyTargetingIndex { get; set; }
        
        public void GetAllEnemiesInRange(float radius)
        {
            EnemiesCountInRange = Physics2D.CircleCastNonAlloc(transform.position, radius, Vector2.zero, enemiesInRange,
                0f, enemyLayer);
            Array.Sort(enemiesInRange, 0, EnemiesCountInRange,
                Comparer<RaycastHit2D>.Create((x, y) =>
                    Vector2.Distance(x.point, transform.position)
                        .CompareTo(Vector2.Distance(y.point, transform.position))));
            // enemiesInRange.Sort((x, y) => x.distance.CompareTo(y.distance));
            EnemyTargetingIndex = 0;
        }
    }
}