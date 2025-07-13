using System;
using System.Collections;
using UnityEngine;

namespace InGame.UI.HitShowDamage
{
    public class UIHitDameText : MonoBehaviour
    {
        private IDamageable target;

        private void Awake()
        {
            target = GetComponentInParent<IDamageable>();
            if (target != null)
            {
                target.OnHit += OnDamaged;
            }
        }

        private void OnDestroy()
        {
            if (target != null)
            {
                target.OnHit -= OnDamaged;
            }
        }

        private void OnDamaged(int damage)
        {
            UIHitDameTextPool.Instance.ShowDamage(damage, transform.position);
        }
    }
}