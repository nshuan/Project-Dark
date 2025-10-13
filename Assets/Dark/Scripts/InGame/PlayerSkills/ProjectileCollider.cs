using System;
using UnityEngine;

namespace InGame
{
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class ProjectileCollider : MonoBehaviour
    {
        [SerializeField] private LayerMask hitLayer;
        
        private ProjectileEntity projectile;

        public ProjectileEntity Projectile
        {
            get => projectile;
            set
            {
                projectile = value;
                if (projectile != null)
                    lastPosition = projectile.transform.position;
            }
        }

        public bool CanTrigger { get; set; }
        
        private CapsuleCollider2D capsuleCollider;
        private EnemyEntity hitEnemy;
        private Vector2 lastPosition;
        private Vector2 direction;
        private RaycastHit2D[] hits = new RaycastHit2D[10];
        
        // private void OnTriggerEnter2D(Collider2D other)
        // {
        //     if (other.CompareTag("Tower"))
        //     {
        //         Projectile.ProjectileHit(null);    
        //     }
        //     
        //     if (other.CompareTag("InGameBoundary"))
        //     {
        //         Projectile.BlockSpawnDeadBody = true;
        //         return;
        //     }
        //     
        //     if (other.TryGetComponent<EnemyEntity>(out hitEnemy))
        //     {
        //         Projectile.ProjectileHit(hitEnemy);
        //         DebugUtility.Log($"Hit enemy {hitEnemy.name}");
        //     }
        // }

        private void Awake()
        {
            capsuleCollider = GetComponent<CapsuleCollider2D>();
        }

        private void FixedUpdate()
        {
            if (!Projectile) return;
            if (!CanTrigger) return;
            
            direction.x = projectile.transform.position.x - lastPosition.x;
            direction.y = projectile.transform.position.y - lastPosition.y;
            // Lấy y làm radius nếu ảnh viên đạn trong prefab nằm ngang
            var hitCount = Physics2D.CircleCastNonAlloc(lastPosition, capsuleCollider.size.y, direction, hits, direction.magnitude, hitLayer);
            if (hitCount > 0)
            {
                // Chỉ check hit 1 object đầu tiên va chạm
                for (var i = 0; i < 1; i++)
                {
                    if (hits[i].transform.TryGetComponent<EnemyEntity>(out hitEnemy))
                    {
                        Projectile.ProjectileHit(hitEnemy);
                        DebugUtility.Log($"Hit enemy {hitEnemy.name}");
                    }
                    
                    if (hits[i].transform.CompareTag("Tower"))
                    {
                        if (hits[i].transform.TryGetComponent<TowerEntity>(out var towerEntity))
                        {
                            if (towerEntity.Id != LevelManager.Instance.CurrentTower.Id) Projectile.ProjectileHit(null);    
                        }
                    }
                    
                    if (hits[i].transform.CompareTag("InGameBoundary"))
                    {
                        Projectile.BlockSpawnDeadBody = true;
                        continue;
                    }
                }
            }
            lastPosition.x = projectile.transform.position.x;
            lastPosition.y = projectile.transform.position.y;
        }
        
        public void UpdateLastPosition(Vector2 position)
        {
            lastPosition.x = position.x;
            lastPosition.y = position.y;
        }
    }
}