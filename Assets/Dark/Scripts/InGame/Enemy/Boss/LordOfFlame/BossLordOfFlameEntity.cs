using System.Collections;
using System.Linq;
using Dark.Scripts.Utils;
using InGame.BossConfig;
using InGame.EnemyEffect;
using UnityEngine;

namespace InGame.Boss
{
    public class BossLordOfFlameEntity : EnemyEntity
    {
        [Space] [Header("Customize boss")]
        [SerializeField] protected EnemySpritesAnimationInfo attack2Anim;
        [SerializeField] protected EnemySpritesAnimationInfo recoverAnim;
        [SerializeField] protected EnemySpritesAnimationInfo disappearAnim;
        [SerializeField] protected EnemySpritesAnimationInfo appearAnim;
        [SerializeField] protected EnemySpritesAnimationInfo spearAttackAnim;
        [SerializeField] public EnemySpritesAnimationInfo comboAttackAnim;

        [Space] [Header("Config")]
        [Tooltip("Config that store exclusive variable for this boss")] 
        [SerializeField] private BossLordOfFlameConfig lordOfFlameConfig;
        
        [Tooltip("After play attack animation, delay these seconds before doing attack logic")]
        [SerializeField] private float delayAttackAnim = 0.4f;
        [Tooltip("After play attack animation, delay these seconds before doing attack logic")]
        [SerializeField] private float delayAttack2Anim = 0.5f;
        [Tooltip("Delay before recovering animation and recovering logic")] 
        [SerializeField] private float delayRecover = 1f;
        
        // Attack [X] lần thì đổi anim attack
        private int minAttackTurnToSwitchType = 1;
        private int maxAttackTurnToSwitchType = 2;
        private int currentAttackCountdown = 2;
        private bool hasRecoverOnce = false;
        private bool isRecovering = false;
        private bool isAttacking = false;
        private Coroutine coroutineRecover;
        
        public override void Init(EnemyBehaviour eConfig, TowerEntity target, WaveStatsScale statsScale, float levelExpRatio,
            float levelDarkRatio, int levelDarkUnitValue)
        {
            base.Init(eConfig, target, statsScale, levelExpRatio, levelDarkRatio, levelDarkUnitValue);

            currentAttackCountdown = maxAttackTurnToSwitchType;
            hasRecoverOnce = false;
            isRecovering = false;
            isAttacking = false;
        }

        protected override IEnumerator IEAttack()
        {
            // Đợi qua vài frame của anim attack rồi mới xử lý logic để cho đẹp
            while (true)
            {
                if (inAttackRange)
                {
                    if (isRecovering) yield return new WaitUntil(() => !isRecovering);
                    var attackDuration = 0f;
                    var delayAttack = 0f;
                    isAttacking = true;
                    if (currentAttackCountdown > 0)
                    {
                        currentAttackCountdown -= 1;
                        // phase 1 dùng attack thường, phase 2 dùng spear
                        if (hasRecoverOnce)
                        {
                            attackDuration = animController.PlayCustomAnim(spearAttackAnim);
                            delayAttack = Mathf.Min(delayAttack2Anim, 1 / config.attackSpeed); // số frame delay attack của spear bằng attack2
                        }
                        else
                        {
                            attackDuration = animController.PlayAttack();
                            delayAttack = Mathf.Min(delayAttackAnim, 1 / config.attackSpeed);
                        }
                        if (delayAttack > attackDuration) delayAttack = attackDuration;
                    }
                    else
                    {
                        // phase 1 dùng attack thường, phase 2 dùng spear
                        if (hasRecoverOnce) attackDuration = animController.PlayCustomAnim(spearAttackAnim);
                        else attackDuration = animController.PlayCustomAnim(attack2Anim);
                        currentAttackCountdown = RandomUtil.Range(minAttackTurnToSwitchType, maxAttackTurnToSwitchType + 1);
                        delayAttack = Mathf.Min(delayAttack2Anim, 1 / config.attackSpeed);
                        if (delayAttack > attackDuration) delayAttack = attackDuration;
                    }
                    
                    yield return new WaitForSeconds(delayAttack);
                    Attack();
                    yield return new WaitForSeconds(attackDuration - delayAttack);
                    isAttacking = false;
                    yield return new WaitForSeconds(1 / config.attackSpeed - attackDuration);
                }
                else
                    yield return new WaitUntil(() => inAttackRange);
            }
        }

        protected override void Attack()
        {
            if  (TargetTower.IsDestroyed) return;
            config.attackBehaviour.Attack(this, TargetTower, transform.position, CurrentDamage);
        }

        public override void Damage(int damage, Vector2 dealerPosition, float stagger, DamageType dmgType)
        {
            if (isRecovering) return;
            base.Damage(damage, dealerPosition, stagger, dmgType);
             
            if (IsDestroyed) return;
            if (!hasRecoverOnce && PercentageHpLeft < lordOfFlameConfig.percentageToHeal)
            {
                isRecovering = true;
                hasRecoverOnce = true;
                StartCoroutine(IERecover(0.5f));
            }
        }

        private IEnumerator IERecover(float delay)
        {
            State = EnemyState.Freeze;
            animController.PlayIdle();
            yield return new WaitUntil(() => isAttacking == false);
            yield return new WaitForSeconds(delay);
            yield return new WaitForSeconds(0.5f);
            CurrentHealth += (int)(MaxHealth * lordOfFlameConfig.percentageHealed);
            var animDuration = animController.PlayCustomAnim(recoverAnim);
            yield return new WaitForSeconds(animDuration);
            yield return StartCoroutine(IEChangeTower(0f));
            animController.PlayRun();
            isRecovering = false;
            State = EnemyState.Move;
        }

        private IEnumerator IEChangeTower(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            // disappear
            var teleDuration = animController.PlayCustomAnim(disappearAnim);
            yield return new WaitForSeconds(teleDuration);
            
            yield return new WaitForEndOfFrame();
            // Change tower
            TargetTower = LevelManager.Instance.Towers.FirstOrDefault((t) => t.Id == lordOfFlameConfig.phase2TowerId);
            if (!TargetTower) TargetTower = LevelManager.Instance.CurrentTower;
            Target = TargetTower.transform;
            AttackRange = lordOfFlameConfig.phase2AtkRange;
            // Mặc định rớt trong tầm đánh luôn
            var dropDistanceToTower = AttackRange - 0.1f;
            transform.position = Target.position + 
                                 (Quaternion.Euler(0f, 0f, RandomUtil.Range(-20f, 20f)) * (transform.position - Target.position).normalized) * dropDistanceToTower;
            attackPosition = transform.position;
            animController.transform.localScale =
                new Vector3(Mathf.Sign(Target.position.x - transform.position.x), 1f, 1f);
            
            // appear
            teleDuration = animController.PlayCustomAnim(appearAnim);
            yield return new WaitForSeconds(teleDuration);
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.5f);
        }
    }
}