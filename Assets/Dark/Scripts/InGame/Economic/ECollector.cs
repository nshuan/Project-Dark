using System;
using Economic;
using InGame.UI.Economic.KillShowCollected;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame.Economic
{
    public class ECollector : MonoBehaviour
    {
        private void Awake()
        {
            EnemyManager.Instance.OnOneEnemyDead += OnEnemyDead;
        }

        private void OnEnemyDead(EnemyEntity enemy)
        {
            WealthManager.Instance.AddExp(enemy.Exp);
            if (Random.Range(0f, 0f) <= enemy.DarkRatio)
                WealthManager.Instance.AddDark(enemy.Dark);
            WealthManager.Instance.AddBossPoint(enemy.BossPoint);    
            
            // TH1: Show text trên đầu con enemy vừa die
            UIKillCollectedPool.Instance.ShowCollected(enemy.Dark, enemy.transform.position);
        }
    }
}