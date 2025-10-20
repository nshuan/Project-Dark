using System;
using InGame;
using Economic.InGame.DropItems;
using Economic.UI.KillShowCollected;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Economic.UI
{
    public class ECollector : MonoBehaviour
    {
        public int selectMethod = 1;

        private LevelManager levelManager;
        
        private void Awake()
        {
            levelManager = LevelManager.Instance;
            EnemyManager.Instance.OnOneEnemyDead += OnEnemyDead;
            LevelManager.Instance.onWaveEnded += OnWaveEnded;
        }
        
        private void OnEnemyDead(EnemyEntity enemy)
        {
            
            // Show text [+Exp] trên đầu nhân vật
            if (enemy.Exp > 0)
            {
                WealthManager.Instance.AddExp(enemy.Exp);
                UIKillCollectedPool.Instance.ShowCollected(WealthType.Exp, enemy.Exp, levelManager.Player.transform.position);
            }
            
            // TH0: Show text trên đầu con enemy vừa die
            if (selectMethod == 0)
            {
                if (Random.Range(0f, 1f) <= enemy.DarkRatio)
                    WealthManager.Instance.AddDark(enemy.Dark);
                if (enemy.BossPoint > 0)
                    WealthManager.Instance.AddBossPoint(enemy.BossPoint);    
                
                UIKillCollectedPool.Instance.ShowCollected(WealthType.Vestige, enemy.Dark, enemy.transform.position);
            }
            
            // TH1: Rớt item ra end wave thì tự động collect hết
            if (selectMethod == 1)
            {
                if (Random.Range(0f, 1f) <= enemy.DarkRatio && enemy.Dark > 0)
                    EItemDropManager.Instance.DropOne(WealthType.Vestige, enemy.Dark, enemy.transform.position);
                if (enemy.BossPoint > 0)
                    EItemDropManager.Instance.DropOne(WealthType.Sigils, enemy.BossPoint, enemy.transform.position);
            }
            
            // TH2: Giống 1, nhưng rớt 1 item cho 1 đơn vị resource
            if (selectMethod == 2)
            {
                if (Random.Range(0f, 1f) <= enemy.DarkRatio && enemy.Dark > 0)
                {
                    for (var i = 0; i < enemy.Dark; i++)
                    {
                        EItemDropManager.Instance.DropOne(WealthType.Vestige, 1, enemy.transform.position);                        
                    }
                }

                if (enemy.BossPoint > 0)
                {
                    for (var i = 0; i < enemy.Dark; i++)
                    {
                        EItemDropManager.Instance.DropOne(WealthType.Sigils, 1, enemy.transform.position);
                    }
                }
            }
            
            // TH3: Collect bằng cách di chuột qua item, cần tick vào enableCollectorMouse ở trong InputInGame
            if (selectMethod == 3)
            {
                if (Random.Range(0f, 1f) <= enemy.DarkRatio && enemy.Dark > 0)
                {
                    for (var i = 0; i < enemy.Dark; i++)
                    {
                        EItemDropManager.Instance.DropOne(WealthType.Vestige, 1, enemy.transform.position, true);                        
                    }
                }

                if (enemy.BossPoint > 0)
                {
                    for (var i = 0; i < enemy.Dark; i++)
                    {
                        EItemDropManager.Instance.DropOne(WealthType.Sigils, 1, enemy.transform.position, true);
                    }
                }
            }
        }

        private void OnWaveEnded(int waveIndex, WaveEndReason reason)
        {
            // TH1: Rớt item ra end wave thì tự động collect hết
            // TH2: Giống 1, nhưng rớt 1 item cho 1 đơn vị resource
            if (selectMethod == 1 ||  selectMethod == 2)
                EItemDropManager.Instance.CollectAll();
        }
    }
}