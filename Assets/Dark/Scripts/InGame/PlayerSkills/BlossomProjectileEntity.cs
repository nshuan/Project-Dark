using UnityEngine;

namespace InGame
{
    public class BlossomProjectileEntity : ProjectileEntity
    {
        protected override void PlayVfxHit()
        {
            var hitVfx = BlossomProjectileHitVfxPool.Instance.Get(transform, true);
            hitVfx.transform.localPosition = Vector3.zero;
            hitVfx.transform.localRotation = Quaternion.identity;
            hitVfx.Activate((vfx) => BlossomProjectileHitVfxPool.Instance.Release(vfx));
        }

        protected override void PlayHitActions(EnemyEntity hit)
        {
            if (HitActions != null)
            {
                foreach (var action in HitActions)
                {
                    action.DoAction(this, transform.position, (p) =>
                    {
                        if (!hit) return;
                        p.collider.IgnoreEnemy(hit);
                    });
                }
            }
        }
    }
}