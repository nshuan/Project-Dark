using System;
using Unity.VisualScripting;
using UnityEngine;

namespace InGame
{
    public class ProjectileEntity : MonoBehaviour
    {
        private const float MaxLifeTime = 5f;
        private float speed = 5f;
        [SerializeField] private float damageRange = 0.1f;
        private Vector2 direction;
        private Vector2 target;

        private bool activated = false;
        private float lifeTime = 0f;

        private RaycastHit2D[] hits = new RaycastHit2D[1];
        
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
            if (Vector2.Distance(transform.position, target) < 0.1f) ProjectileHit(null);
            transform.position += (Vector3)(speed * Time.deltaTime * direction);
            lifeTime += Time.deltaTime;
            if (lifeTime > MaxLifeTime) Destroy(gameObject);
            
            // Check hit enemy
            var count = Physics2D.CircleCastNonAlloc(transform.position, damageRange, Vector2.zero, hits, 0f,
                LayerMask.GetMask("Entity"));
            if (count > 0)
                ProjectileHit(hits[0].transform);
        }

        protected virtual void ProjectileHit(Transform hitTransform)
        {
            if (hitTransform)
            {
                if (hitTransform.TryGetComponent<EnemyEntity>(out var enemy))
                    enemy.OnHit(LevelManager.Instance.GameStats.pDmgPerShot);
            }
            Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, damageRange);
        }
    }
}