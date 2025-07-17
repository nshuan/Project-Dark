using System;
using System.Collections;
using DG.Tweening;
using Economic;
using InGame.EnemyEffect;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace InGame
{
    public class EnemyEntity : MonoBehaviour, IDamageable, IEffectTarget
    {
        private static float StaggerMaxDuration = 0.5f; // Hit back 1f on 0.5s

        [SerializeField] private Collider2D collider2d;
        
        public Transform Target { get; set; }
        public IDamageable TargetDamageable { get; set; }
        private EnemyBehaviour config;

        #region Stats
        private int MaxHealth { get; set; }
        private int CurrentHealth { get; set; }
        private int CurrentDamage { get; set; }
        private int Exp { get; set; }
        private int Dark { get; set; }
        private float DarkRatio { get; set; }
        private int BossPoint { get; set; }

        #endregion

        public float PercentageHpLeft => CurrentHealth / MaxHealth * 100f;
        public Action<int> OnHit { get; set; }
        public Action OnDead { get; set; }
        public EnemyState State { get; set; }
        public int UniqueId { get; set; }
        private Vector3 direction = new Vector3();
        private Vector2 directionAddition = new Vector2();
        private float staggerDuration;
        private Vector2 staggerDirection;

        [Space, Header("Visual")] 
        [SerializeField] private EnemyBoidAgent boidAgent;
        [SerializeField] private Transform uiHealth;
        public EnemyAnimController animController;
        
        private bool inAttackRange;
        private Coroutine attackCoroutine;

        private Vector2 attackPosition;
        private Vector3 hitDirection = new Vector3();
        
        private float invisibleTimer;
        
        #region Initialize

        public void Init(EnemyBehaviour eConfig, TowerEntity target, float hpMultiplier, float dmgMultiplier, float levelExpRatio, float levelDarkRatio)
        {
            config = eConfig;
            
            // Set target and attack position
            Target = target.transform;
            TargetDamageable = target;

            collider2d.enabled = false;
            
            var myPos = transform.position;
            var targetPos = Target.position;
            attackPosition = ((Quaternion.Euler(0f, 0f, Random.Range(-75f, 75f)) *
                               (Vector2)(myPos - targetPos).normalized) * (0.9f * config.attackRange)
                              + targetPos);
            animController.transform.localScale =
                new Vector3(Mathf.Sign(attackPosition.x - myPos.x), 1f, 1f);
            
            MaxHealth = (int)(config.hp * hpMultiplier);
            CurrentHealth = MaxHealth;
            CurrentDamage = Mathf.RoundToInt(config.dmg * dmgMultiplier);
            Exp = Mathf.RoundToInt(config.exp * levelExpRatio);
            Dark = Mathf.RoundToInt(config.dark * levelDarkRatio);
            DarkRatio = config.darkRatio;
            BossPoint = config.bossPoint;
            
            State = EnemyState.Spawn;
            inAttackRange = false;
            IsDestroyed = false;
            config.Init(this);
        }

        #endregion

        #region Core function

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }
        
        public void Activate()
        {
            config.Spawn(this, () =>
            {
                StartAttackCoroutine();
                State = EnemyState.Move;
                animController.PlayRun();
                boidAgent.IsActive = true;
                collider2d.enabled = true;
            });
        }

        #endregion

        private void Update()
        {
            if (!Target) return;
            if (IsDestroyed) return;
            if (State == EnemyState.Spawn) return;
            if (State == EnemyState.Invisible)
            {
                invisibleTimer -= Time.deltaTime;
                if (invisibleTimer <= 0) State = EnemyState.Move;
            }

            if (staggerDuration > 0)
            {
                staggerDuration -= Time.deltaTime;
                var startPos = (Vector2)transform.position;
                transform.position = Vector2.MoveTowards(startPos, startPos + staggerDirection, 0.5f * Time.deltaTime);
            }
            else
            {
                boidAgent.GetBoidAdditionNonAlloc(ref directionAddition);
                MoveTo(Target);
            }
        }

        private void MoveTo(Transform target)
        {
            if (Vector3.Distance(transform.position, target.position) < config.attackRange)
            {
                inAttackRange = true;
                animController.SetDefaultRun(false);
            }
            else
            {
                config.moveBehaviour.MoveNonAlloc(transform, attackPosition, directionAddition, config.attackRange, config.moveSpeed, ref direction);
                animController.SetDefaultRun(true);
            }
        }

        private void StartAttackCoroutine()
        {
            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);

            attackCoroutine = StartCoroutine(IEAttack());
        }

        private IEnumerator IEAttack()
        {
            while (true)
            {
                if (inAttackRange)
                {
                    Attack();
                    yield return new WaitForSeconds(config.attackSpeed);
                }
                else
                    yield return new WaitUntil(() => inAttackRange);
            }
        }

        private void Attack()
        {
            if (TargetDamageable.IsDestroyed) return;
            config.attackBehaviour.Attack(TargetDamageable, CurrentDamage);
            animController.PlayAttack();
        }

        public void Damage(int damage, Vector2 attackerPos, float stagger)
        {
            if (IsDestroyed) return;
            if (State == EnemyState.Invisible) return;
            
            CurrentHealth -= damage;
            hitDirection.x = transform.position.x - attackerPos.x;
            hitDirection.y = transform.position.y - attackerPos.y;
            hitDirection.x /= hitDirection.magnitude;
            hitDirection.y /= hitDirection.magnitude;
            
            OnHit?.Invoke(damage);
            if (CurrentHealth <= 0)
            {
                OnDie();
            }
            else
            {
                if (stagger - config.staggerResist > 0)
                {
                    staggerDirection = (stagger - config.staggerResist) * hitDirection;
                    staggerDuration = (stagger - config.staggerResist) * StaggerMaxDuration;
                }
                
                animController.PlayHit();
                invisibleTimer = config.invisibleDuration;
                State = EnemyState.Invisible;
            }
        }

        public bool IsDestroyed { get; set; }

        private void OnDie()
        {
            collider2d.enabled = false;
            IsDestroyed = true;
            OnDead?.Invoke();
            OnDead = null;
            boidAgent.IsActive = false;
            StartCoroutine(IEDie(
                animController.PlayDie(), 0.5f
                ));
        }

        private IEnumerator IEDie(float delayAnim, float delayRelease)
        {
            yield return new WaitForSeconds(delayAnim);
            WealthManager.Instance.AddExp(Exp);
            if (Random.Range(0f, 0f) <= DarkRatio)
                WealthManager.Instance.AddDark(Dark);
            WealthManager.Instance.AddBossPoint(BossPoint);
            
            yield return new WaitForSeconds(delayRelease);
            EnemyPool.Instance.Release(this, config.enemyId);
            
        }

        #region Effect 

        public Transform TargetTransform => transform;
        public void Burn(float duration, float delayEachBurn, int damage)
        {
            StartCoroutine(IEBurn(duration, delayEachBurn, damage));
        }

        private IEnumerator IEBurn(float duration, float delayEachBurn, int damage)
        {
            var totalBurn = (int)(duration / delayEachBurn);

            while (totalBurn > 0)
            {
                yield return new WaitForSeconds(delayEachBurn);
                Damage(damage, transform.position, 0f);
                totalBurn -= 1;
            }
        }

        public void Kill()
        {
            Damage(CurrentHealth, transform.position, 0f);
        }
        #endregion
    }
}