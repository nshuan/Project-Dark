using System.Collections;
using Dark.Scripts.Utils;
using Data;
using InGame.UI;
using UnityEngine;

namespace InGame.Boss
{
    public class BossSummonerKingEntity : EnemyNecromancerEntity
    {
        private bool isAttacking;
        
        public override void Init(EnemyBehaviour eConfig, TowerEntity target, WaveStatsScale statsScale, float levelExpRatio,
            float levelDarkRatio, int levelDarkUnitValue)
        {
            base.Init(eConfig, target, statsScale, levelExpRatio, levelDarkRatio, levelDarkUnitValue);

            if (LevelManager.Instance.Level.level != PlayerDataManager.Instance.Data.level + 1)
                BossPoint = 0;
        }

        protected override IEnumerator IEAttack()
        {
            while (true)
            {
                yield return new WaitForSeconds(1 / config.attackSpeed);
                Attack();
            }
        }

        protected override void Attack()
        {
            if (TargetTower.IsDestroyed) return;
            isAttacking = true;
            animController.PlayAttack();
            this.DelayCall(animController.GetAttackDelayTrigger(), () =>
            {
                if (TargetTower.IsDestroyed) return;
                config.attackBehaviour.Attack(this, TargetTower, transform.position, CurrentDamage);
            });
            this.DelayCall(animController.GetAttackDuration(), () =>
            {
                isAttacking = false;
            });
        }

        protected override void Update()
        {
            if (isAttacking) return;
            base.Update();
        }

        protected override IEnumerator IEDie(float delayRelease, EnemyDieReason reason)
        {
            // Làm đen hết màn hình, tắt UI
            BackgroundInGame.Instance.SetActiveBlackBg(true);
            CanvasInGame.Instance.HideUI();
            
            CombatActions.OnBossKilled?.Invoke(config, transform.position);
            var dropVestige = Dark > 0;
            CombatActions.OnDropResource?.Invoke(this, dropVestige);
            OnDead?.Invoke(this, reason);
            OnDead = null;
            yield return new WaitForSeconds(delayRelease);
            EnemyPool.Instance.Release(this, config.enemyId);
        }

        protected override void DropResource()
        {
           
        }
    }
}