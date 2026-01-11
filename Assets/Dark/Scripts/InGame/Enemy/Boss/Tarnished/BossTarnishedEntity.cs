using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using InGame.BossConfig;
using InGame.CameraController;
using InGame.EnemyEffect;
using InGame.UI;
using UnityEngine;

namespace InGame.Boss
{
    public class BossTarnishedEntity : EnemyEntity
    {
        private bool isAttacking = false;
        private bool isChangingTower = false;

        public override void Init(EnemyBehaviour eConfig, TowerEntity target, WaveStatsScale statsScale, float levelExpRatio,
            float levelDarkRatio, int levelDarkUnitValue)
        {
            base.Init(eConfig, target, statsScale, levelExpRatio, levelDarkRatio, levelDarkUnitValue);

            if (LevelManager.Instance.Level.level != PlayerDataManager.Instance.Data.level + 1)
                BossPoint = 0;
        }

        public override void ActivateELite(bool active)
        {
            base.ActivateELite(active);

            thresholdChangeTower = tarnishedConfig.GetHpThreshold();
            startDistanceEachPhase = tarnishedConfig.GetStartDistance();
        }

        protected override IEnumerator IEDie(float delayRelease, EnemyDieReason reason)
        {
            // Làm đen hết màn hình, tắt UI
            BackgroundInGame.Instance.SetActiveBlackBg(true);
            CanvasInGame.Instance.HideUI();
            
            CombatActions.OnBossKilled?.Invoke(config, transform.position);
            var dropVestige = Dark > 0;
            CombatActions.OnDropResource?.Invoke(this, dropVestige);
            OnDead?.Invoke(reason);
            OnDead = null;
            yield return new WaitForSeconds(delayRelease);
            EnemyPool.Instance.Release(this, config.enemyId);
        }

        protected override void DropResource()
        {
           
        }

        protected override IEnumerator IEAttack()
        {
            while (true)
            {
                if (inAttackRange)
                {
                    var delayAttack = 0f;
                    isAttacking = true;
                    delayAttack = Mathf.Min(delayAttackAnim, 1 / config.attackSpeed);
                    var attackDuration = animController.PlayAttack();
                    yield return new WaitForSeconds(delayAttack);
                    Attack();
                    if (IsChangeTower())
                    {
                        currentThresholdChangeTowerIndex += 1;
                        State = EnemyState.Freeze;
                        yield return new WaitForSeconds(Mathf.Max(attackDuration - delayAttack, 0f));
                        isAttacking = false;
                        yield return StartCoroutine(IEChangeTower(0f));
                    }
                    else
                    {
                        isAttacking = false;
                        yield return new WaitForSeconds(1 / config.attackSpeed - delayAttack);
                    }

                }
                else
                    yield return new WaitUntil(() => inAttackRange);
            }
        }

        protected override void Attack()
        {
            if (TargetTower.IsDestroyed) return;
            config.attackBehaviour.Attack(this, TargetTower, transform.position, CurrentDamage);
        }

        private bool IsChangeTower()
        {
            if (!config.elite) return false;
                
            if (CurrentHealth <= 0) return false;

            for (var i = thresholdChangeTower.Count - 1; i > currentThresholdChangeTowerIndex; i--)
            {
                if (PercentageHpLeft <= thresholdChangeTower[i])
                {
                    currentThresholdChangeTowerIndex = i;
                    break;
                }
            }

            return currentThresholdChangeTowerIndex < thresholdChangeTower.Count &&
                   PercentageHpLeft <= thresholdChangeTower[currentThresholdChangeTowerIndex];
        }

        public override void Damage(int damage, Vector2 dealerPosition, float stagger, DamageType dmgType)
        {
            if (State == EnemyState.Freeze) return;
            base.Damage(damage, dealerPosition, stagger, dmgType);

            if (!isAttacking && !isChangingTower && IsChangeTower())
            {
                currentThresholdChangeTowerIndex += 1;
                State = EnemyState.Freeze;
                StartCoroutine(IEChangeTower(0f));
            }
        }

        #region Change tower

        [Space] [Header("Elite boss behaviour")] 
        [SerializeField] private BossTarnishedConfig tarnishedConfig;
        [SerializeField] protected EnemySpritesAnimationInfo jumpUpAnim;
        [SerializeField] protected EnemySpritesAnimationInfo jumpDownAnim;
        [Tooltip("After play attack animation, delay these seconds before doing attack logic")]
        [SerializeField] private float delayAttackAnim = 0.4f;
        
        private List<float> thresholdChangeTower = new List<float>() { 0.75f, 0.5f, 0.25f };
        private List<int> orderChangeTower = new List<int>() { }; // Nếu null hoặc ko có phần tử thì random
        private List<float> startDistanceEachPhase = new List<float>();
        private int currentThresholdChangeTowerIndex = 0;

        private IEnumerator IEChangeTower(float delay)
        {
            isChangingTower = true;
            yield return new WaitForSeconds(delay);
            animController.Pause();
            yield return new WaitForEndOfFrame();
            
            // Jump up
            var jumpDuration = animController.PlayCustomAnim(jumpUpAnim);
            animController.Resume();
            BurnVfxParent.gameObject.SetActive(false);
            yield return new WaitForSeconds(jumpDuration);
            
            if (orderChangeTower is { Count: > 0 })
            {
                // Check next tower in order
                var nextTowerId = orderChangeTower[currentThresholdChangeTowerIndex - 1];
                TargetTower = LevelManager.Instance.Towers.FirstOrDefault((t) => t.Id == nextTowerId);
                if (!TargetTower) TargetTower = LevelManager.Instance.CurrentTower;
                Target = TargetTower.transform;
            }
            else
            {
                // Get random tower
                var listTargetToRandom = new List<TowerEntity>();
                foreach (var tower in LevelManager.Instance.Towers)
                {
                    if (tower.Id != TargetTower.Id) listTargetToRandom.Add(tower);
                }
                
                TargetTower = listTargetToRandom[RandomUtil.Range(0, listTargetToRandom.Count)];
                Target = TargetTower.transform;
            }

            // Mặc định rớt trong tầm đánh luôn
            var dropDistanceToTower = config.attackRange * 0.9f;
            if (startDistanceEachPhase != null && startDistanceEachPhase.Count > currentThresholdChangeTowerIndex - 1)
            {
                dropDistanceToTower = startDistanceEachPhase[currentThresholdChangeTowerIndex - 1];
            }
                
            transform.position = Target.position + 
                                 (Quaternion.Euler(0f, 0f, RandomUtil.Range(-20f, 20f)) * (transform.position - Target.position).normalized) * dropDistanceToTower;
            var myPos = transform.position;
            var targetPos = Target.position;
            attackPosition = ((Quaternion.Euler(0f, 0f, RandomUtil.Range(-75f, 75f)) *
                               (Vector2)(myPos - targetPos).normalized) * (0.9f * config.attackRange)
                              + targetPos);
            animController.transform.localScale =
                new Vector3(Mathf.Sign(attackPosition.x - myPos.x), 1f, 1f);
            
            // Jump down
            jumpDuration = animController.PlayCustomAnim(jumpDownAnim);
            yield return new WaitForSeconds(jumpDuration);
            BurnVfxParent.gameObject.SetActive(true);
            State = EnemyState.Move;
            isChangingTower = false;
        }
        
        #endregion
    }
}