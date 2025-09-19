using System;
using Economic;
using InGame.Economic.DropItems;
using InGame.UI.Economic.KillShowCollected;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame.Economic
{
    public class ECollector : MonoBehaviour
    {
        public int selectMethod = 1;
        
        private void Awake()
        {
            EnemyManager.Instance.OnOneEnemyDead += OnEnemyDead;
        }

        private void OnEnemyDead(EnemyEntity enemy)
        {
            WealthManager.Instance.AddExp(enemy.Exp);
            
            // TH1: Show text trên đầu con enemy vừa die
            if (selectMethod == 0)
            {
                if (Random.Range(0f, 0f) <= enemy.DarkRatio)
                    WealthManager.Instance.AddDark(enemy.Dark);
                if (enemy.BossPoint > 0)
                    WealthManager.Instance.AddBossPoint(enemy.BossPoint);    
                
                UIKillCollectedPool.Instance.ShowCollected(WealthType.Vestige, enemy.Dark, enemy.transform.position);
            }
            
            // TH2: Rớt item ra end wave thì tự động collect hết
            if (selectMethod == 1)
            {
                if (Random.Range(0f, 0f) <= enemy.DarkRatio && enemy.Dark > 0)
                    EItemDropManager.Instance.DropOne(WealthType.Vestige, enemy.Dark, enemy.transform.position);
                if (enemy.BossPoint > 0)
                    EItemDropManager.Instance.DropOne(WealthType.Sigils, enemy.BossPoint, enemy.transform.position);
            }
        }
    }
}