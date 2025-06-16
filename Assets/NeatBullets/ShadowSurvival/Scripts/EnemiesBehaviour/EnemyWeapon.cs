using UnityEngine;

namespace NeatBullets.ShadowSurvival.Scripts.EnemiesBehaviour {
    public class EnemyWeapon : MonoBehaviour
    {
        public Animator Animator;
        public SpriteRenderer SpriteRenderer;
        public EnemyController Enemy;
        public float ProjectileSpeed;
    
        // assigned from editor
        public Transform ProjectileSpawnPoint;
    
        private void Awake() {
            Animator = GetComponent<Animator>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
            Enemy = GetComponentInParent<EnemyController>();
        }

        private void Start() {
            ProjectileSpeed = Enemy.MovementSpeed * 1.8f;
        }
    
        public void ShootProjectile(Vector2 direction) {
            EnemyProjectile projectile = Instantiate(Enemy.ProjectilePrefab, ProjectileSpawnPoint.position, transform.rotation, Enemy.transform);
            projectile.Direction = direction.normalized;
            projectile.ProjectileSpeed = ProjectileSpeed;
        }
    }
}
