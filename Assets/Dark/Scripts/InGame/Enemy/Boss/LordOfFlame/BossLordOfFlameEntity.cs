using System.Collections;
using System.Linq;
using Dark.Scripts.Utils;
using DG.Tweening;
using InGame.BossConfig;
using InGame.EnemyEffect;
using InGame.UI;
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
        private float damageScale = 1f;
        
        [Tooltip("After play attack animation, delay these seconds before doing attack logic")]
        [SerializeField] private float delayAttackAnim = 0.4f;
        [Tooltip("After play attack animation, delay these seconds before doing attack logic")]
        [SerializeField] private float delayAttack2Anim = 0.5f;
        [Tooltip("Delay before recovering animation and recovering logic")] 
        [SerializeField] private float delayRecover = 1f;
        
        private bool hasRecoverOnce = false;
        private bool isRecovering = false;
        private bool isAttacking = false;
        private Coroutine coroutineRecover;
        private EnemyBossLordOfFlameBehaviour configCasted;
        private SpriteRenderer shadowSprite;
        private float shadowOriginalAlpha;
        
        public override void Init(EnemyBehaviour eConfig, TowerEntity target, WaveStatsScale statsScale, float levelExpRatio,
            float levelDarkRatio, int levelDarkUnitValue)
        {
            base.Init(eConfig, target, statsScale, levelExpRatio, levelDarkRatio, levelDarkUnitValue);

            configCasted = (EnemyBossLordOfFlameBehaviour)config;
            hasRecoverOnce = false;
            isRecovering = false;
            isAttacking = false;
            shadowSprite = shadow.GetComponent<SpriteRenderer>();
            shadowOriginalAlpha = shadowSprite.color.a;
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

                    var allSkill = hasRecoverOnce
                        ? configCasted.lordOfFlameConfig.attackPhase2Info
                        : configCasted.lordOfFlameConfig.attackPhase1Info;

                    var attackSkillId = RandomUtil.RangeWithOwnRate(allSkill
                        .Select((skill) => skill.chance).ToArray());
                    damageScale = allSkill[attackSkillId].dmgScale;

                    switch (attackSkillId)
                    {
                        // Normal attack 2
                        case 1:
                            attackDuration = animController.PlayCustomAnim(attack2Anim);
                            delayAttack = Mathf.Min(delayAttack2Anim, 1 / config.attackSpeed);
                            break;
                        // Spear attack
                        case 2:
                            attackDuration = animController.PlayCustomAnim(spearAttackAnim);
                            delayAttack = Mathf.Min(delayAttack2Anim, 1 / config.attackSpeed); // số frame delay attack của spear bằng attack2
                            break;
                        // Normal attack 1
                        case 0:
                        default:
                            attackDuration = animController.PlayAttack();
                            delayAttack = Mathf.Min(delayAttackAnim, 1 / config.attackSpeed);
                            break;
                    }
                        
                    if (delayAttack > attackDuration) delayAttack = attackDuration;
                    
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
            config.attackBehaviour.Attack(this, TargetTower, transform.position, LevelUtilityV2.ToInt(CurrentDamage * damageScale));
        }

        public override void Damage(int damage, Vector2 dealerPosition, float stagger, DamageType dmgType)
        {
            if (isRecovering) return;
            base.Damage(damage, dealerPosition, stagger, dmgType);
             
            if (IsDestroyed) return;
            if (!hasRecoverOnce && PercentageHpLeft < configCasted.lordOfFlameConfig.percentageToHeal)
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
            CurrentHealth += (int)(MaxHealth * configCasted.lordOfFlameConfig.percentageHealed);
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
            DOTween.Kill(shadowSprite);
            shadowSprite?.DOFade(0f, teleDuration).SetEase(Ease.InQuad).SetTarget(shadowSprite);
            yield return new WaitForSeconds(teleDuration);
            
            yield return new WaitForEndOfFrame();
            // Change tower
            TargetTower = LevelManager.Instance.Towers.FirstOrDefault((t) => t.Id == configCasted.lordOfFlameConfig.phase2TowerId);
            if (!TargetTower) TargetTower = LevelManager.Instance.CurrentTower;
            Target = TargetTower.transform;
            AttackRange = configCasted.lordOfFlameConfig.phase2AtkRange;
            // Mặc định rớt trong tầm đánh luôn
            var dropDistanceToTower = AttackRange - 0.1f;
            // Nêu tele vào trụ 1 thì đổi hướng vị trí tele
            if (TargetTower.Id == 0)
                dropDistanceToTower = -dropDistanceToTower;
            transform.position = Target.position + new Vector3(-dropDistanceToTower, -0.2f, 0f);
            attackPosition = transform.position;
            animController.transform.localScale =
                new Vector3(Mathf.Sign(Target.position.x - transform.position.x), 1f, 1f);
            
            // appear
            teleDuration = animController.PlayCustomAnim(appearAnim);
            DOTween.Kill(shadowSprite);
            shadowSprite?.DOFade(shadowOriginalAlpha, 0.5f).SetEase(Ease.InQuad).SetTarget(shadowSprite);
            yield return new WaitForSeconds(teleDuration);
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.5f);
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
    }
}