using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
        private List<Transform> allHitEnemiesInCurrentShot;
        private int totalHitCountInCurrentShot;
        
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

        public void Init()
        {
            // Lưu lại ref đến những enemy đã va chạm, mỗi con chỉ dính dame 1 lần đối với 1 lần bắn
            allHitEnemiesInCurrentShot ??= new List<Transform>();
            for (var i = 0; i < allHitEnemiesInCurrentShot.Count; i++)
            {
                allHitEnemiesInCurrentShot[i] = null;
            }
            totalHitCountInCurrentShot = 0;
        }
        
        public ProjectileHitStatus CheckCollision(ref Vector3 targetPosition)
        {
            if (!Projectile) return ProjectileHitStatus.None;
            if (!CanTrigger) return ProjectileHitStatus.None;
            
            direction.x = targetPosition.x - lastPosition.x;
            direction.y = targetPosition.y - lastPosition.y;
            // Lấy y làm radius nếu ảnh viên đạn trong prefab nằm ngang
            var hitCount = Physics2D.CircleCastNonAlloc(lastPosition, capsuleCollider.size.y, direction, hits, direction.magnitude, hitLayer);
            if (hitCount > 0)
            {
                // Chỉ check hit 1 object đầu tiên va chạm, nếu là enemy thì trước đấy phải chưa va chạm lần nào
                for (var i = 0; i < hitCount; i++)
                {
                    if (allHitEnemiesInCurrentShot.Any((hit) => ReferenceEquals(hit, hits[i].transform))) continue;
                    if (hits[i].transform.TryGetComponent<EnemyEntity>(out hitEnemy))
                    {
                        if (totalHitCountInCurrentShot < allHitEnemiesInCurrentShot.Count)
                            allHitEnemiesInCurrentShot[totalHitCountInCurrentShot] = hits[i].transform;
                        else
                            allHitEnemiesInCurrentShot.Add(hits[i].transform);
                        totalHitCountInCurrentShot += 1;
                        
                        DebugUtility.Log($"Hit enemy {hitEnemy.name}");
                        Projectile.transform.position = hitEnemy.transform.position;
                        Projectile.ProjectileHit(hitEnemy);
                        if (!hitEnemy.IsDestroyed)
                        {
                            var deadProjectile = ProjectileDeadPool.Instance.Get(direction);
                            deadProjectile.position = hitEnemy.transform.position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(0f, 0.2f), 0f);
                            deadProjectile.SetParent(hitEnemy.transform);
                        }
                        return ProjectileHitStatus.Enemy;
                    }
                    
                    if (hits[i].transform.CompareTag("Tower"))
                    {
                        if (hits[i].transform.TryGetComponent<TowerEntity>(out var towerEntity))
                        {
                            if (towerEntity.Id != LevelManager.Instance.CurrentTower.Id)
                            {
                                Projectile.ProjectileHit(null);
                                return ProjectileHitStatus.Tower;
                            }    
                            break;
                        }
                    }
                    
                    if (hits[i].transform.CompareTag("InGameBoundary"))
                    {
                        Projectile.BlockSpawnDeadBody = true;
                        return ProjectileHitStatus.Boundary;
                    }
                }
            }
            lastPosition.x = projectile.transform.position.x;
            lastPosition.y = projectile.transform.position.y;

            return ProjectileHitStatus.None;
        }
        
        public void UpdateLastPosition(Vector2 position)
        {
            lastPosition.x = position.x;
            lastPosition.y = position.y;
        }

        public void CheckHitEnemiesOnInit(float radius = 1f)
        {
            // Lấy y làm radius nếu ảnh viên đạn trong prefab nằm ngang
            var hitCount = Physics2D.CircleCastNonAlloc(projectile.transform.position, radius, direction, hits, 0f, hitLayer);
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
                }
            }
        }

        public enum ProjectileHitStatus
        {
            None,
            Enemy,
            Tower,
            Boundary
        }
    }
}