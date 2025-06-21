using System;
using UnityEngine;

namespace InGame
{
    public class ProjectileEntity : MonoBehaviour
    {
        private const float MaxLifeTime = 5f;
        private float speed = 5f;
        private float damageRange = 1f;
        private Vector2 direction;
        private Vector2 target;

        private bool activated = false;
        private float lifeTime = 0f;

        private void OnDisable()
        {
            activated = false;
        }

        public void Init(float spe, Vector2 targetPos)
        {
            speed = spe;
            target = targetPos;
            direction = (target - (Vector2)transform.position).normalized;
            lifeTime = 0f;
        }

        public void Activate()
        {
            activated = true;
            gameObject.SetActive(true);
        }

        private void Update()
        {
            if (!activated) return;
            if (Vector2.Distance(transform.position, target) < 0.1f) ProjectileHit();
            transform.position += (Vector3)(speed * Time.deltaTime * direction);
            lifeTime += Time.deltaTime;
            if (lifeTime > MaxLifeTime) Destroy(gameObject);
        }

        private void ProjectileHit()
        {
            // Check hit enemy, all enemies are hit
            var hits = Physics2D.CircleCastAll(transform.position, damageRange, Vector2.zero, 0f, LayerMask.GetMask("Entity"));
            var damage = LevelManager.Instance.GameStats.pDmgPerShot;
            foreach (var hit in hits)
            {
                if (hit.collider && hit.transform.TryGetComponent<EnemyEntity>(out var enemyEntity))
                {
                    enemyEntity.OnHit(damage);
                }
            }
            Destroy(gameObject);
        }
    }
}