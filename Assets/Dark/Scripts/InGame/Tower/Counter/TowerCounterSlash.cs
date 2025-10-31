using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace InGame
{
    public class TowerCounterSlash : TowerCounter
    {
        public float baseRange;
        public float damageAngle = 75f;

        [SerializeField] private LayerMask hitLayer;

        private float Range => LevelUtility.GetTowerCounterRange(counterType, baseRange);

        private RaycastHit2D[] hits = new RaycastHit2D[20];
        private EnemyEntity cacheEnemy;

        public override void Counter(Vector2 towerAttackPos, Vector2 direction, int damage, float speedScale)
        {
            var hitCount = Physics2D.CircleCastNonAlloc(transform.position, Range, direction, hits, 0f, hitLayer);
            if (hitCount > 0)
            {
                var halfAngle = damageAngle / 2;
                for (var i = 0; i < hitCount; i++)
                {
                    var dirTo = (hits[i].point - (Vector2)transform.position).normalized;
                    // Check những enemy va chạm, nếu nằm trong góc damageAngle thì mới gây dame
                    if (Vector2.Angle(direction, dirTo) <= halfAngle)
                    {
                        if (hits[i].transform.TryGetComponent<EnemyEntity>(out cacheEnemy))
                        {
                            cacheEnemy.Damage(Damage, transform.position, 0f, DamageType.Normal);
                        }
                    }
                }
            }
        }
        

#if UNITY_EDITOR
        [Space]
        [Title("Debug (Editor)")]
        public bool showGizmos;
        [Header("Shape")]
        public float radius = 2f;
        
        [Tooltip("The center rotation of the fan in degrees (0 = +X axis, CCW positive).")]
        public float rotationAngle = 0f;

        [Header("Style")]
        public bool filled = true;
        [Range(0f, 1f)] public float fillAlpha = 0.2f;
        public Color lineColor = Color.yellow;
        public Color fillColor = Color.yellow;

        [Header("Radials")]
        public bool drawRadialEdges = true;
        
        private void OnDrawGizmos()
        {
            if (radius <= 0f) return;

            float halfAngle = damageAngle * 0.5f;

            // The "from" direction is rotationAngle - halfAngle
            Vector3 fromDir = DirFromDeg(rotationAngle - halfAngle);

            // Save old color
            var oldColor = UnityEditor.Handles.color;

            // Filled wedge
            if (filled && damageAngle > 0f)
            {
                UnityEditor.Handles.color = new Color(fillColor.r, fillColor.g, fillColor.b, fillAlpha);
                UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.forward, fromDir, damageAngle, radius);
            }

            // Outline arc
            UnityEditor.Handles.color = lineColor;
            UnityEditor.Handles.DrawWireArc(transform.position, Vector3.forward, fromDir, damageAngle, radius);

            // Radial edges
            if (drawRadialEdges && damageAngle < 360f)
            {
                Vector3 toDir = DirFromDeg(rotationAngle + halfAngle);
                UnityEditor.Handles.DrawLine(transform.position, transform.position + fromDir * radius);
                UnityEditor.Handles.DrawLine(transform.position, transform.position + toDir * radius);
            }

            UnityEditor.Handles.color = oldColor;
        }

        private static Vector3 DirFromDeg(float deg)
        {
            float rad = deg * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);
        }
#endif
    }
}