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
        private PlayerCharacter player;
        
        private void Awake()
        {
            CombatActions.OnResourceCollectorDamaged += OnCollectEntityDamaged;
            LevelManager.Instance.OnLevelLoaded += OnLevelLoaded;
            LevelManager.Instance.OnWin += OnWin;
            LevelManager.Instance.OnLose += OnLose;
            // LevelManager.Instance.onWaveEnded += OnWaveEnded;
        }

        private void OnWin()
        {
            EItemDropManager.Instance.CollectAll(player.transform);
            CombatActions.OnDropResource -= OnDropResource;
        }
        
        private void OnLose()
        {
            CombatActions.OnDropResource -= OnDropResource;
        }

        private void OnLevelLoaded(LevelConfig levelConfig)
        {
            player = LevelManager.Instance.Player;
            CombatActions.OnDropResource += OnDropResource;
        }
        
        private void OnDropResource(EnemyEntity enemy, bool hasVestige)
        {
            if (!player) player = LevelManager.Instance.Player;
            
            // Show text [+Exp] trên đầu nhân vật
            if (enemy.Exp > 0)
            {
                WealthManager.Instance.AddExp(enemy.Exp);
                UIKillCollectedPool.Instance.ShowCollected(WealthType.Exp, enemy.Exp, player.transform.position);
            }
            
            if (hasVestige)
                EItemDropManager.Instance.Drop(WealthType.Vestige, enemy.DarkUnitValue, enemy.Dark, enemy.transform.position);
            if (enemy.BossPoint > 0)
                EItemDropManager.Instance.DropOne(WealthType.Sigils, enemy.BossPoint, enemy.transform.position);
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