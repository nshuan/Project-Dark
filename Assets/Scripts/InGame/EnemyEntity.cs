using System;
using System.Collections;
using DG.Tweening;
using InGame.EnemyEffect;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace InGame
{
    public class EnemyEntity : MonoBehaviour, IDamageable
    {
        public Transform Target { get; set; }
        public IDamageable TargetDamageable { get; set; }
        private EnemyBehaviour config;
        private float MaxHealth { get; set; }
        private float CurrentHealth { get; set; }
        private int CurrentDamage { get; set; }
        public bool IsDead => CurrentHealth <= 0;
        public Action OnDead { get; set; }
        public EnemyState State { get; set; }
        public int UniqueId { get; set; }
        private Vector3 direction = new Vector3();
        [SerializeField] private EnemyActionEffectTrigger effectTrigger;

        [Space, Header("Visual")] 
        [SerializeField] private EnemyBoidAgent boidAgent;
        [SerializeField] private Transform uiHealth;
        [SerializeField] private AnimController animController;
        
        private bool inAttackRange;
        private Coroutine attackCoroutine;

        private Vector2 attackPosition;
        
        #region Initialize

        public void Init(EnemyBehaviour eConfig, TowerEntity target, float hpMultiplier, float dmgMultiplier)
        {
            config = eConfig;
            
            // Set target and attack position
            Target = target.transform;
            TargetDamageable = target;
            
            var myPos = transform.position;
            var targetPos = Target.position;
            attackPosition = ((Quaternion.Euler(0f, 0f, Random.Range(-75f, 75f)) *
                               (Vector2)(myPos - targetPos).normalized) * (0.9f * config.attackRange)
                              + targetPos);
            animController.transform.localScale =
                new Vector3(Mathf.Sign((attackPosition - (Vector2)myPos).x), 1f, 1f);
            
            MaxHealth = config.hp * hpMultiplier;
            CurrentHealth = MaxHealth;
            CurrentDamage = Mathf.RoundToInt(config.dmg * dmgMultiplier);
            State = EnemyState.Spawn;
            IsDestroyed = false;
            effectTrigger.Enemy = this;
            effectTrigger.Setup(config.effects);
            config.Init(transform);
            EnemyBoidManager.Instance.RegisterAgent(boidAgent);
            
            // Update health ui
            UIUpdateHealth();
        }

        #endregion

        #region Core function

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }
        
        public void Activate()
        {
            config.Spawn(transform, () =>
            {
                StartAttackCoroutine();
                State = EnemyState.Move;
                animController.PlayRun();
                boidAgent.IsActive = true;
            });
        }

        #endregion

        private void Update()
        {
            if (!Target) return;
            if (IsDead) return;
            if (State != EnemyState.Move) return;
            
            MoveTo(Target, boidAgent.GetBoidAddition());
        }

        private void MoveTo(Transform target, Vector2 directionAdder)
        {
            if (Vector3.Distance(transform.position, target.position) < config.attackRange)
                inAttackRange = true;
            else
                config.moveBehaviour.MoveNonAlloc(transform, attackPosition, directionAdder, config.attackRange, config.moveSpeed, ref direction);
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

        public void Damage(int damage)
        {
            if (IsDead) return;
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                OnDie();
            }
            else
            {
                // UIUpdateHealth();
                animController.PlayHit();
            }
        }

        public bool IsDestroyed { get; set; }

        private void OnDie()
        {
            IsDestroyed = true;
            OnDead?.Invoke();
            OnDead = null;
            animController.PlayDie();
            boidAgent.IsActive = false;
            StartCoroutine(IEDie());
        }

        private IEnumerator IEDie()
        {
            yield return new WaitForSeconds(1f);
            EnemyPool.Instance.Release(this, config.enemyId);
        }
        
        private void UIUpdateHealth()
        {
            DOTween.Complete(this);
            var seq = DOTween.Sequence(this);
            seq.Append(transform.DOPunchScale(0.5f * Vector3.one, 0.2f))
                .Join(uiHealth.DOScale(Mathf.Clamp(CurrentHealth, 0f, MaxHealth) / MaxHealth, 0.2f));
            seq.Play();
        }
    }
}