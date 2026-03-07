using System.Collections;
using Dark.Scripts.OutGame.Settings;
using Dark.Scripts.Utils;
using Data;
using InGame.BossConfig;
using InGame.EnemyEffect;
using InGame.UI;
using Steamworks.NET;
using UnityEngine;

namespace InGame.Boss
{
    public class BossSummonerKingEntity : EnemyNecromancerEntity
    {
        [Space] [Header("Customize boss")]
        [SerializeField] public EnemySpritesAnimationInfo buffAnim;
        [SerializeField] public float delayTriggerBuff;

        public EnemyBossSummonerKingBehaviour configCasted;
        private bool isAttacking;
        
        public override void Init(EnemyBehaviour eConfig, TowerEntity target, WaveStatsScale statsScale, float levelExpRatio,
            float levelDarkRatio, int levelDarkUnitValue)
        {
            base.Init(eConfig, target, statsScale, levelExpRatio, levelDarkRatio, levelDarkUnitValue);

            if (LevelManager.Instance.Level.level != PlayerDataManager.Instance.Data.level + 1)
                BossPoint = 0;

            configCasted = (EnemyBossSummonerKingBehaviour)config;
            
            UISettingIconBoss.SetBossUnlocked(config.enemyId);
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
                config.attackBehaviour.Attack(this, TargetTower, transform.position, LevelUtilityV2.ToInt(CurrentDamage * TempDmgScale));
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
            SteamStats.Instance.TryClaimAchievement(
                LevelManager.Instance.PlayerClass == CharacterClass.CharacterClass.Knight
                    ? SteamAchievementsAPIName.KNIGHT_KILL_SUMMONER_KING
                    : SteamAchievementsAPIName.ARCHER_KILL_SUMMONER_KING);
            
            LevelManager.Instance.BlockDamageAllTowers();
            
            // Làm đen hết màn hình, tắt UI
            BackgroundInGame.Instance.SetActiveBlackBg(true);
            CanvasInGame.Instance.HideUI();
            
            CombatActions.OnBossKilled?.Invoke(this, transform.position);
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

        public void UseBuffSkill()
        {
            foreach (var enemy in EnemyManager.Instance.Enemies)
            {
                if (enemy.Value == this) continue;
                
                if (enemy.Value.gameObject.activeInHierarchy &&
                    enemy.Value.IsDestroyed == false)
                {
                    enemy.Value.TempDmgScale = configCasted.summonerKingConfig.buffSpawn.scaleDmg;
                    enemy.Value.TempSpeedScale = configCasted.summonerKingConfig.buffSpawn.scaleSpeed;
                    enemy.Value.TempAtkSpeedScale = configCasted.summonerKingConfig.buffSpawn.scaleAtkSpeed;
                }
            }
        }
    }
}