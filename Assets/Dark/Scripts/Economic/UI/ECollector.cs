using Data;
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
        
        private const int EnsureVestige = 10;
        private int droppedEnsureVestige;
        
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
            EItemDropManager.Instance.CollectAll(player.transform, true);
            CombatActions.OnDropResource -= OnDropResource;
        }
        
        private void OnLose()
        {
            CombatActions.OnDropResource -= OnDropResource;
        }

        private void OnLevelLoaded(LevelConfig levelConfig)
        {
            player = LevelManager.Instance.Player;
            droppedEnsureVestige = PlayerDataManager.Instance.Data.passedDay == 1 ? 0 : EnsureVestige;
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

            if (TryDropFirstDayVestige(out var dropAmount))
            {
                EItemDropManager.Instance.Drop(WealthType.Vestige, 1, dropAmount, enemy.transform.position);
            }
            else
            {
                if (hasVestige)
                    EItemDropManager.Instance.Drop(WealthType.Vestige, enemy.DarkUnitValue, enemy.Dark, enemy.transform.position);
            }
            
            if (enemy.BossPoint > 0)
                EItemDropManager.Instance.DropOne(WealthType.Sigils, enemy.BossPoint, enemy.transform.position);
        }

        private void OnCollectEntityDamaged(EItemDropCollector collector)
        {
            EItemDropManager.Instance.CollectAll(collector.transform, false);
        }
        
        /// <summary>
        /// If this is first play in run, ensure that 10 vestige are dropped
        /// </summary>
        /// <returns></returns>
        private bool TryDropFirstDayVestige(out int dropAmount)
        {
            if (droppedEnsureVestige >= EnsureVestige)
            {
                dropAmount = 0;
                return false;
            }

            dropAmount = RandomUtil.Range(0, 5);
            if (dropAmount > EnsureVestige - droppedEnsureVestige) dropAmount = EnsureVestige - droppedEnsureVestige;
            droppedEnsureVestige += dropAmount;
            if (dropAmount > 0)
                return true;

            return false;
        }
        
        // private void OnWaveEnded(int wave, WaveEndReason reason)
        // {
        //     EItemDropManager.Instance.CollectAll(player.transform);
        // }
    }
}