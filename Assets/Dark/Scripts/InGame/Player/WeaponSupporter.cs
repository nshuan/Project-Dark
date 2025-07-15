using UnityEngine;

namespace InGame
{
    public class WeaponSupporter : MonoBehaviour
    {
        [SerializeField] private LayerMask enemyLayer;
        
        private static RaycastHit2D[] enemiesInRange = new RaycastHit2D[30];
        public static RaycastHit2D[] EnemiesInRange => enemiesInRange;
        public static int EnemiesCountInRange { get; set; }
        public static int EnemyTargetingIndex { get; set; }
        
        public void GetAllEnemiesInRange(float radius)
        {
            EnemiesCountInRange = Physics2D.CircleCastNonAlloc(transform.position, radius, Vector2.zero, enemiesInRange,
                0f, enemyLayer);
            EnemyTargetingIndex = 0;
        }
    }
}