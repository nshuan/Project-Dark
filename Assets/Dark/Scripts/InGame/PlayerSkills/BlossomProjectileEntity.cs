using System;
using UnityEngine;

namespace InGame
{
    public class BlossomProjectileEntity : ProjectileEntity
    {
        private void Start()
        {
            hits = new RaycastHit2D[10];
        }

        protected override void PlayVfxHit()
        {
            var hitVfx = BlossomProjectileHitVfxPool.Instance.Get(transform, true);
            hitVfx.transform.localPosition = Vector3.zero;
            hitVfx.transform.localRotation = Quaternion.identity;
            hitVfx.Activate((vfx) => BlossomProjectileHitVfxPool.Instance.Release(vfx));
        }

        protected override void PlayHitActions(EnemyEntity hit)
        {
            var explodeCenter = hit ? hit.transform.position : transform.position;
            var explodeHitCount = Physics2D.CircleCastNonAlloc(explodeCenter, Size, Vector2.zero, hits, 0f,
                collider.hitLayer);
            if (explodeHitCount > 0)
            {
                for (var i = 0; i < explodeHitCount; i++)
                {
                    if (hits[i].transform)
                    {
                        if (hit && ReferenceEquals(hits[i].transform, hit.transform)) continue;
                        if (hits[i].transform.TryGetComponent<EnemyEntity>(out var hitEnemy))
                        {
                            hitEnemy.HitDirectionX = hitEnemy.transform.position.x - explodeCenter.x;
                            hitEnemy.HitDirectionY = hitEnemy.transform.position.y - explodeCenter.y;
                            hitEnemy.Damage(Damage, explodeCenter, Stagger, InGame.DamageType.Normal);
                        }
                    }
                }
            }
            
            if (HitActions != null)
            {
                foreach (var action in HitActions)
                {
                    action.DoAction(this, transform.position, (p) =>
                    {
                        if (hit) p.collider.IgnoreEnemy(hit);
                        if (explodeHitCount > 0)
                        {
                            for (var i = 0; i < explodeHitCount; i++)
                            {
                                if (hits[i].transform)
                                {
                                    if (hit && ReferenceEquals(hits[i].transform, hit.transform)) continue;
                                    if (hits[i].transform.TryGetComponent<EnemyEntity>(out var hitEnemy))
                                    {
                                        p.collider.IgnoreEnemy(hitEnemy);
                                    }
                                }
                            }
                        }
                    });
                }
            }
        }
    }
}