using System.Collections;
using System.Linq;
using Dark.Scripts.Settings;
using Data;
using DG.Tweening;
using InGame.EnemyEffect;
using InGame.UI;
using UnityEngine;

namespace InGame.Boss.BossSkillSystem
{
    public class BossEntity : EnemyEntity
    {
        [Space] [Header("Skill")] 
        [SerializeField] protected BossSkillComponent skillComponent;
        
        [Space] [Header("Customize boss")]
        [SerializeField] protected EnemySpritesAnimationInfo attack2Anim;
        
        public override void Init(EnemyBehaviour eConfig, TowerEntity target, WaveStatsScale statsScale, float levelExpRatio,
            float levelDarkRatio, int levelDarkUnitValue)
        {
            base.Init(eConfig, target, statsScale, levelExpRatio, levelDarkRatio, levelDarkUnitValue);

            skillComponent.BossScript = this;
            
            if (LevelManager.Instance.Level.level != PlayerDataManager.Instance.Data.level + 1)
                BossPoint = 0;
        }
  

        protected override IEnumerator IEAttack()
        {
            // Ngay sau khi chạy xong anim spawn
            skillComponent?.TriggerSpawn(0f);
            skillComponent?.StartInterval();
            yield break;
        }

        protected override void Attack()
        {
            if  (TargetTower.IsDestroyed) return;
            config.attackBehaviour.Attack(this, TargetTower, transform.position, LevelUtilityV2.ToInt(CurrentDamage));
        }

        public override void Damage(int damage, Vector2 dealerPosition, float stagger, DamageType dmgType, bool instantKill)
        {
            base.Damage(damage, dealerPosition, stagger, dmgType, instantKill);
             
            if (IsDestroyed) return;
            skillComponent?.TriggerHit(0f);
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