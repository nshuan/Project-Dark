using UnityEngine;

namespace InGame
{
    public class TowerRegenerateOnKill : MonoBehaviour
    {
        private TowerEntity tower;
        private int regenAmount;
        
        public void Initialize(TowerEntity targetTower, int amount)
        {
            tower = targetTower;
            regenAmount = amount;

            EnemyManager.Instance.OnOneEnemyDead += OnOneEnemyKilled;
        }

        private void OnOneEnemyKilled()
        {
            tower.Regenerate(regenAmount);
        }
    }
}