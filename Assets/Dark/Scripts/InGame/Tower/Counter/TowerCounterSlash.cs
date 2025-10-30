using System.Collections;
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
            direction = direction / direction.magnitude * Range;

            var hitCount = Physics2D.CircleCastNonAlloc(transform.position, Range, direction, hits, 0f, hitLayer);
            if (hitCount > 0)
            {
                var halfAngle = damageAngle / 2;
                for (var i = 0; i < hitCount; i++)
                {
                    var dirTo = (hits[i].point - (Vector2)transform.position).normalized;
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
    }
}