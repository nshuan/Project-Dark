using System.Collections;
using UnityEngine;

namespace InGame
{
    public class KnightAutoAimProjectile : AutoAimProjectileEntity
    {
        protected override IEnumerator IEActivate(float delay)
        {
            Size = 1f;
            transform.localScale = Vector3.one;
            return base.IEActivate(delay);
        }
    }
}