using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dark.Scripts.OutGame.Settings;
using Dark.Scripts.Utils;
using Data;
using DG.Tweening;
using InGame.EnemyEffect;
using InGame.UI;
using UnityEngine;

namespace InGame.Boss.BossWizard
{
    public class BossWizardEntity : EnemyEntity
    {
        [Space] [Header("Customize boss")] 
        [SerializeField] public EnemySpritesAnimationInfo summonAnimInfo;

        [SerializeField] private EnemySpritesAnimation swordAnim; 
        
        public EnemyBossWizardBehaviour configCasted;
        
        private bool isAttacking;
        private bool isChangingTower;
        private bool hasChangeTower;

        private Queue<EnemySpritesAnimation> swordAnimPool;
        
        public override void Init(EnemyBehaviour eConfig, TowerEntity target, WaveStatsScale statsScale, float levelExpRatio,
            float levelDarkRatio, int levelDarkUnitValue)
        {
            base.Init(eConfig, target, statsScale, levelExpRatio, levelDarkRatio, levelDarkUnitValue);

            swordAnimPool = new Queue<EnemySpritesAnimation>();
            if (LevelManager.Instance.Level.level != PlayerDataManager.Instance.Data.level + 1)
                BossPoint = 0;
            if (LevelManager.IsPlayingEndless)
                BossPoint = 0;

            configCasted = (EnemyBossWizardBehaviour)config;
            swordAnim.transform.SetParent(null);
            swordAnim.gameObject.SetActive(false);
            
            UISettingIconBoss.SetBossUnlocked(config.enemyId);
        }

        protected override IEnumerator IEAttack()
        {
            if (configCasted.wizardConfig.summonContinuously)
                StartCoroutine(IESummon());
            
            while (true)
            {
                if (inAttackRange)
                {
                    if (isChangingTower)
                        yield return new WaitUntil(() => !isChangingTower);
                    else
                    {
                        Attack();
                        yield return new WaitForSeconds(1 / (config.attackSpeed * TempAtkSpeedScale));
                    }
                }
                else
                    yield return new WaitUntil(() => inAttackRange);
            }
        }

        protected IEnumerator IESummon()
        {
            while (true)
            {
                yield return new WaitForSeconds(configCasted.wizardConfig.summonInterval);
                if (isChangingTower) yield return null;
                if (configCasted.summonIds is { Count: > 0 } &&
                    configCasted.summonAmount is { Count: > 0 })
                {
                    var summonIndex = RandomUtil.Range(0, configCasted.summonAmount.Count);
                    if (summonIndex >= 0 && summonIndex < configCasted.summonIds.Count &&
                        configCasted.summonAmount[summonIndex] > 0)
                    {
                        var randomTargetForCreeps =
                            RandomUtil.ShuffleIndex(0, LevelManager.Instance.Towers.Length - 1)
                                .Select((towerIndex) => LevelManager.Instance.Towers[towerIndex]).ToArray();
                        configCasted.summonBehaviour.Summon(this, randomTargetForCreeps,
                            configCasted.summonIds[summonIndex],
                            configCasted.summonAmount[summonIndex]);
                    }
                }
            }
        }

        protected override void Attack()
        {
            if (TargetTower.IsDestroyed) return;
            isAttacking = true;
            animController.PlayAttack();
            if (!swordAnimPool.TryDequeue(out var sword))
            {
                sword = Instantiate(swordAnim, null);
                sword.transform.localScale = Vector3.one;
            }
            sword.PlayIdle();
            sword.gameObject.SetActive(false);
            sword.transform.position =
                TargetTower.GetBaseCenter() + RandomUtil.InsideUnitSpan(new Vector3(0f, -1f), 240f);
            var delayTrigger = animController.GetAttackDelayTrigger();
            this.DelayCall(delayTrigger, () =>
            {
                if (TargetTower.IsDestroyed) return;
                sword.gameObject.SetActive(true);
                sword.PlayAttack();
                this.DelayCall(sword.GetAttackDelayTrigger(), () =>
                {
                    config.attackBehaviour.Attack(this, TargetTower, transform.position,
                        LevelUtilityV2.ToInt(CurrentDamage * TempDmgScale));
                });
            });
            this.DelayCall(delayTrigger + sword.GetAttackDuration(), () =>
            {
                sword.gameObject.SetActive(false);
                swordAnimPool.Enqueue(sword);
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

        private IEnumerator IEChangePhase2()
        {
            State = EnemyState.Freeze;
            yield return new WaitUntil(() => !isAttacking);

            var seq = DOTween.Sequence().SetTarget(this);
            
            if (configCasted.wizardConfig.listSummonIdsOnPhase2 is { Count: > 0 } && configCasted.wizardConfig.listSummonAmountOnPhase2 is { Count: > 0 })
            {
                var summonIndex = RandomUtil.Range(0, configCasted.wizardConfig.listSummonAmountOnPhase2.Count);
                if (summonIndex >= 0 && summonIndex < configCasted.wizardConfig.listSummonIdsOnPhase2.Count && configCasted.wizardConfig.listSummonAmountOnPhase2[summonIndex] > 0)
                {
                    seq.AppendCallback(() =>
                        {
                            animController.PlayCustomAnim(summonAnimInfo);
                        })
                        .AppendInterval(0.5f)
                        .AppendCallback(() =>
                        {
                            // var randomTargetForCreeps =
                            //     RandomUtil.ShuffleIndex(0, LevelManager.Instance.Towers.Length - 1)
                            //         .Select((towerIndex) => LevelManager.Instance.Towers[towerIndex]).ToArray();
                            configCasted.summonBehaviour.Summon(this, TargetTower,
                                configCasted.wizardConfig.listSummonIdsOnPhase2[summonIndex],
                                configCasted.wizardConfig.listSummonAmountOnPhase2[summonIndex]);
                        })
                        .AppendInterval(animController.GetCustomAnimDuration(summonAnimInfo));
                }
            }

            yield return seq.WaitForCompletion();
            
            TargetTower = LevelManager.Instance.Towers.FirstOrDefault((t) => t.Id == configCasted.wizardConfig.phase2TowerId);
            if (!TargetTower) TargetTower = LevelManager.Instance.CurrentTower;
            Target = TargetTower.transform;
            TempDmgScale = configCasted.wizardConfig.phase2ScaleDamage;
            TempSpeedScale = configCasted.wizardConfig.phase2ScaleSpeed;
            var dropDistanceToTower = AttackRange - 0.1f;
            // Nêu tele vào trụ 1 thì đổi hướng vị trí tele
            if (TargetTower.Id == 0)
                dropDistanceToTower = -dropDistanceToTower;
            attackPosition = Target.position + new Vector3(-dropDistanceToTower, -0.2f, 0f);
            animController.transform.localScale =
                new Vector3(Mathf.Sign(Target.position.x - transform.position.x), 1f, 1f);
            healthBar.transform.localScale = new Vector3(animController.transform.localScale.x,
                healthBar.transform.localScale.y, healthBar.transform.localScale.z);
            
            State = EnemyState.Move;
            animController.PlayRun();

            yield return new WaitForSeconds(configCasted.wizardConfig.phase2DelayTakeDamage);
            isChangingTower = false;
        }

        public override void Damage(int damage, Vector2 dealerPosition, float stagger, DamageType dmgType, bool instantKill = false)
        {
            if (isChangingTower) return;
            base.Damage(damage, dealerPosition, stagger, dmgType, instantKill);
            
            if (IsDestroyed) return;
            if (!hasChangeTower && PercentageHpLeft < configCasted.wizardConfig.phase2HpPercentage)
            {
                isChangingTower = true;
                hasChangeTower = true;
                StartCoroutine(IEChangePhase2());
            }
        }

        protected override IEnumerator IEDie(float delayRelease, EnemyDieReason reason)
        {
            if (LevelManager.IsPlayingEndless)
            {
                yield return base.IEDie(delayRelease, reason);
                yield break;
            }
            
            LevelManager.Instance.BlockDamageAllTowers();
            
            // Làm đen hết màn hình, tắt UI
            AllBackgroundInGame.Instance.CurrentBackground.SetActiveBlackBg(true);
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
            if (LevelManager.IsPlayingEndless)
            {
                base.DropResource();
            }
        }
    }
}