using System;
using UnityEngine;

namespace InGame
{
    public class ProjectileCollider : MonoBehaviour
    {
        public ProjectileEntity Projectile { get; set; }
        private EnemyEntity hitEnemy;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Tower")
            {
                Projectile.ProjectileHit(null);    
            }
            
            if (other.tag == "InGameBoundary")
            {
                Projectile.BlockSpawnDeadBody = true;
                return;
            }
            
            if (other.TryGetComponent<EnemyEntity>(out hitEnemy))
            {
                Projectile.ProjectileHit(hitEnemy);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            
        }
    }
}