using System;
using Core;
using Economic.InGame;
using InGame;
using Economic.InGame.DropItems;
using Economic.UI.KillShowCollected;
using UnityEngine;

namespace Economic.UI
{
    public class ECollector : MonoBehaviour
    {
        public int selectMethod = 1;

        private PlayerCharacter player;
        
        private void Awake()
        {
            CombatActions.OnResourceCollectorDamaged += OnCollectEntityDamaged;
            LevelManager.Instance.OnLevelLoaded += OnLevelLoaded;
            LevelManager.Instance.OnWin += OnLevelCompleted;
            LevelManager.Instance.OnLose += OnLevelCompleted;
            // LevelManager.Instance.onWaveEnded += OnWaveEnded;
        }

        private void OnLevelCompleted()
        {
            EItemDropManager.Instance.CollectAll(player.transform);
            CombatActions.OnCollectResource -= OnEnemyDead;
        }

        private void OnLevelLoaded(LevelConfig levelConfig)
        {
            player = LevelManager.Instance.Player;
            CombatActions.OnCollectResource += OnEnemyDead;
        }
        
        private void OnEnemyDead(EnemyEntity enemy)
        {
            if (!player) player = LevelManager.Instance.Player;
            
            // Show text [+Exp] trên đầu nhân vật
            if (enemy.Exp > 0)
            {
                WealthManager.Instance.AddExp(enemy.Exp);
                UIKillCollectedPool.Instance.ShowCollected(WealthType.Exp, enemy.Exp, player.transform.position);
            }
            
            // TH1: Rớt item ra end wave thì tự động collect hết
            if (selectMethod == 1)
            {
                if (RandomUtil.Range(0f, 1f) <= enemy.DarkRatio && enemy.Dark > 0)
                    EItemDropManager.Instance.Drop(WealthType.Vestige, enemy.DarkUnitValue, enemy.Dark, enemy.transform.position);
                if (enemy.BossPoint > 0)
                    EItemDropManager.Instance.DropOne(WealthType.Sigils, enemy.BossPoint, enemy.transform.position);
            }
        }

        private void OnCollectEntityDamaged(EItemDropCollector collector)
        {
            EItemDropManager.Instance.CollectAll(collector.transform);
        }

        // private void OnWaveEnded(int wave, WaveEndReason reason)
        // {
        //     EItemDropManager.Instance.CollectAll(player.transform);
        // }
    }
}