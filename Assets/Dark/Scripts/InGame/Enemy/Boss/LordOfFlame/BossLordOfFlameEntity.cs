using System.Collections;
using Dark.Scripts.Utils;
using InGame.EnemyEffect;
using UnityEngine;

namespace InGame.Boss
{
    public class BossLordOfFlameEntity : EnemyEntity
    {
        [SerializeField] protected EnemySpritesAnimationInfo attack2Anim;
        [SerializeField] protected EnemySpritesAnimationInfo recoverAnim;

        [Space] [Header("Config")]
        [Tooltip("After play attack animation, delay these seconds before doing attack logic")]
        [SerializeField] private float delayAttackAnim = 0.4f;
        [Tooltip("After play attack animation, delay these seconds before doing attack logic")]
        [SerializeField] private float delayAttack2Anim = 0.5f;
        [Tooltip("Hp reach down to this percentage, buff recover hp")]
        [SerializeField] private float hpPercentageToRecover = 0.3f;
        [Tooltip("Hp to recover once")]
        [SerializeField] private float hpPercentageRecover = 0.3f;
        [Tooltip("Delay before recovering animation and recovering logic")] 
        [SerializeField] private float delayRecover = 1f;
        
        // Attack [X] lần thì đổi anim attack
        private int minAttackTurnToSwitchType = 1;
        private int maxAttackTurnToSwitchType = 2;
        private int currentAttackCountdown = 2;
        private bool hasRecoverOnce = false;
        private bool isRecovering = false;
        private Coroutine coroutineRecover;
        
        public override void Init(EnemyBehaviour eConfig, TowerEntity target, WaveStatsScale statsScale, float levelExpRatio,
            float levelDarkRatio, int levelDarkUnitValue)
        {
            base.Init(eConfig, target, statsScale, levelExpRatio, levelDarkRatio, levelDarkUnitValue);

            currentAttackCountdown = maxAttackTurnToSwitchType;
            hasRecoverOnce = false;
            isRecovering = false;
        }

        protected override IEnumerator IEAttack()
        {
            // Đợi qua vài frame của anim attack rồi mới xử logic để cho đẹp
            while (true)
            {
                if (inAttackRange)
                {
                    var attackDuration = 0f;
                    var delayAttack = 0f;
                    if (currentAttackCountdown > 0)
                    {
                        attackDuration = animController.PlayAttack();
                        currentAttackCountdown -= 1;
                        delayAttack = Mathf.Min(delayAttackAnim, 1 / config.attackSpeed);
                    }
                    else
                    {
                        attackDuration = animController.PlayCustomAnim(attack2Anim);
                        currentAttackCountdown = RandomUtil.Range(minAttackTurnToSwitchType, maxAttackTurnToSwitchType + 1);
                        delayAttack = Mathf.Min(delayAttack2Anim, 1 / config.attackSpeed);
                    }
                    
                    yield return new WaitForSeconds(delayAttack);
                    Attack();
                    if (!hasRecoverOnce && PercentageHpLeft < hpPercentageToRecover)
                    {
                        yield return StartCoroutine(IERecover(Mathf.Max(attackDuration - delayAttack, 0f)));
                    }
                    else
                    {
                        yield return new WaitForSeconds(1 / config.attackSpeed - delayAttack);
                    }
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
        }

        private IEnumerator IERecover(float delay)
        {
            isRecovering = true;
            hasRecoverOnce = true;
            yield return new WaitForSeconds(delay);
            yield return new WaitForSeconds(0.5f);
            CurrentHealth += (int)(MaxHealth * hpPercentageRecover);
            var animDuration = animController.PlayCustomAnim(recoverAnim);
            yield return new WaitForSeconds(animDuration);
            yield return new WaitForSeconds(1f);
            isRecovering = false;
        }
    }
}