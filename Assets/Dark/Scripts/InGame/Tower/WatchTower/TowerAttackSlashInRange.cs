using UnityEngine;

namespace InGame.WatchTower
{
    public class TowerAttackSlashInRange : TowerAttackInRange
    {
        // [SerializeField] private LayerMask hitLayer;
        //
        // private float Range => LevelUtilityV2.GetCounterSlashRange();
        //
        // private RaycastHit2D[] hits = new RaycastHit2D[20];
        // private EnemyEntity cacheEnemy;
        //
        // public override void Counter(Vector2 towerAttackPos, Vector2 direction, int damage, float speedScale)
        // {
        //     var hitCount = Physics2D.CircleCastNonAlloc(transform.position, Range, direction, hits, 0f, hitLayer);
        //     if (hitCount > 0)
        //     {
        //         var halfAngle = LevelUtilityV2.StatsCounterSlash.size / 2;
        //         for (var i = 0; i < hitCount; i++)
        //         {
        //             var dirTo = (hits[i].point - (Vector2)transform.position).normalized;
        //             // Check những enemy va chạm, nếu nằm trong góc damageAngle thì mới gây dame
        //             if (Vector2.Angle(direction, dirTo) <= halfAngle)
        //             {
        //                 if (hits[i].transform.TryGetComponent<EnemyEntity>(out cacheEnemy))
        //                 {
        //                     cacheEnemy.Damage(Damage, transform.position, LevelUtilityV2.StatsCounterSlash.stagger, DamageType.Normal);
        //                     PassiveEffectManager.Instance.TriggerEffect(PassiveTriggerType.TowerTakeDame, cacheEnemy);
        //                 }
        //             }
        //         }
        //     }
        // }
    }
}