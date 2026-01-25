using System;
using System.Collections.Generic;
using System.Linq;
using Dark.Scripts.ForDemo;
using Economic.InGame;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame
{
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class ProjectileCollider : MonoBehaviour
    {
        public LayerMask hitLayer;
        public LayerMask collectibleLayer;
        
        private ProjectileEntity projectile;

        public ProjectileEntity Projectile
        {
            get => projectile;
            set => projectile = value;
        }

        public bool CanTrigger { get; set; }
        
        private CapsuleCollider2D capsuleCollider;
        private RaycastHit2D[] hits = new RaycastHit2D[10];
        private List<Transform> allHitEnemiesInCurrentShot;
        private int totalHitCountInCurrentShot;

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
        
        public ProjectileHitStatus CheckCollision(ref Vector3 direction, ref HitEnemyInfo hitEnemyInfo)
        {
            if (!Projectile) return ProjectileHitStatus.None;
            if (!CanTrigger) return ProjectileHitStatus.None;

            // Lấy y làm radius nếu ảnh viên đạn trong prefab nằm ngang
            var hitCount = Physics2D.CircleCastNonAlloc(transform.position, capsuleCollider.size.y, direction, hits, direction.magnitude, hitLayer);
            if (hitCount > 0)
            {
                // Chỉ check hit 1 object đầu tiên va chạm, nếu là enemy thì trước đấy phải chưa va chạm lần nào
                for (var i = 0; i < hitCount; i++)
                {
                    if (allHitEnemiesInCurrentShot.Any((hit) => ReferenceEquals(hit, hits[i].transform))) continue;
                    if (hits[i].transform.TryGetComponent<EnemyEntity>(out hitEnemyInfo.hitEnemy))
                    {
                        if (hitEnemyInfo.hitEnemy.Activated)
                        {
                            hitEnemyInfo.hit = hits[i];
                            if (totalHitCountInCurrentShot < allHitEnemiesInCurrentShot.Count)
                                allHitEnemiesInCurrentShot[totalHitCountInCurrentShot] = hits[i].transform;
                            else
                                allHitEnemiesInCurrentShot.Add(hits[i].transform);
                            totalHitCountInCurrentShot += 1;
                            
                            return ProjectileHitStatus.Enemy;
                        }
                    }

                    if (hits[i].transform.CompareTag("Collectible"))
                    {
                        if (hits[i].transform.TryGetComponent<EItemDropCollectorCollider>(out var damageable))
                        {
                            Projectile.ProjectileHit(null);
                            damageable.Break();
                            return ProjectileHitStatus.None;
                        }
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

            return ProjectileHitStatus.None;
        }

        public bool TryHit(Transform hitToCache)
        {
            if (allHitEnemiesInCurrentShot.Any((hit) => ReferenceEquals(hit, hitToCache))) return false;
            
            if (totalHitCountInCurrentShot < allHitEnemiesInCurrentShot.Count)
                allHitEnemiesInCurrentShot[totalHitCountInCurrentShot] = hitToCache;
            else
                allHitEnemiesInCurrentShot.Add(hitToCache);
            totalHitCountInCurrentShot += 1;

            return true;
        }
        
        public void CheckHitEnemiesOnInit(float radius = 1f)
        {

        }

        public bool CheckCollectibleOnWay(Vector2 direction)
        {
            if (DemoConfig.CollectLogicType == 2) return false;
            
#if UNITY_EDITOR
            gizmosDirection = direction;
#endif
            var collectibleHits = new RaycastHit2D[1];
            var hitCount = Physics2D.LinecastNonAlloc(transform.position, transform.position + (Vector3)direction * 10f,
                collectibleHits, collectibleLayer);
            if (hitCount > 0)
                return collectibleHits.Any((hit) => hit && hit.transform.CompareTag("Collectible"));
            return false;
        }

        public void IgnoreEnemy(EnemyEntity enemy)
        {
            allHitEnemiesInCurrentShot ??= new List<Transform>();
            if (totalHitCountInCurrentShot < allHitEnemiesInCurrentShot.Count)
                allHitEnemiesInCurrentShot[totalHitCountInCurrentShot] = enemy.transform;
            else
                allHitEnemiesInCurrentShot.Add(enemy.transform);
            totalHitCountInCurrentShot += 1;
        }

#if UNITY_EDITOR
        private Vector2 gizmosDirection = Vector2.zero;
        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)gizmosDirection * 10f);
        }
#endif

        public enum ProjectileHitStatus
        {
            None,
            Enemy,
            Tower,
            Boundary
        }
        
        public struct HitEnemyInfo
        {
            public EnemyEntity hitEnemy;
            public RaycastHit2D hit;
        }
    }
}