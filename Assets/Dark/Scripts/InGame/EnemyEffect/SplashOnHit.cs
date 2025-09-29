using System;
using UnityEngine;

namespace InGame.EnemyEffect
{
    public class SplashOnHit : MonoBehaviour
    {
        private EnemyEntity enemy;

        private void Awake()
        {
            enemy = GetComponentInParent<EnemyEntity>();
        }

        private void Start()
        {
            enemy.OnHit += OnHit;
        }

        private void OnHit(int damage)
        {
            if (enemy.HitDirectionX is < 0.5f and > -0.5f) return;
            EnemySplashPool.Instance.GetAndRelease(null, enemy.transform.position, enemy.HitDirectionX > 0, 0f, 1f);
        }
    }
}