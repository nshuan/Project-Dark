using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace InGame
{
    [RequireComponent(typeof(EnemyMovementBehaviour))]
    public class EnemyEntity : MonoBehaviour
    {
        public Transform Target { get; set; }
        public EnemyBehaviour config;
        public float MaxHealth { get; private set; }
        private float CurrentHealth { get; set; }
        public bool IsDead => CurrentHealth <= 0;
        public Action OnDead { get; set; }
        public EnemyType Type { get; set; }
        
        // Elemental effect
        public bool IsInLightning { get; set; }
        
        [Space, Header("Visual")] 
        [SerializeField] private Transform uiHealth;

        private EnemyMovementBehaviour movementBehaviour;
        private Vector2 attackPosition;
        
        private bool inAttackRange;
        private Coroutine attackCoroutine;

        private void Awake()
        {
            movementBehaviour = GetComponent<EnemyMovementBehaviour>();
        }

        private void Start()
        {
            StartAttackCoroutine();
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }

        public void Init(float maxHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = MaxHealth;   
            movementBehaviour.Init();
            
            // Update health ui
            UIUpdateHealth();
        }

        public void SetTarget(Transform target)
        {
            Target = target;
            attackPosition = (Quaternion.Euler(0f, 0f, Random.Range(-30f, 30f)) *
                              (transform.position - target.position).normalized).normalized * (0.9f * config.attackRange)
                             + Target.position;
        }

        private void Update()
        {
            if (!Target) return;
            MoveTo(Target);
        }

        private void MoveTo(Transform target)
        {
            if (Vector3.Distance(transform.position, target.position) < config.attackRange)
                inAttackRange = true;
            else
                movementBehaviour.Move(target.position, config.moveSpeed);
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
                    yield return new WaitForSeconds(1 / config.attackSpeed);
                }
                else
                    yield return new WaitUntil(() => inAttackRange);
            }
        }

        private void Attack()
        {
            Debug.Log("Enemy Attack!!!");
        }

        public void OnHit(float damage)
        {
            if (IsDead) return;
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                OnDie();
            }
            else
            {
                UIUpdateHealth();
            }
        }
        
        private void OnDie()
        {
            IsInLightning = false;
            OnDead?.Invoke();
            OnDead = null;
            EnemyPool.Instance.Release(Type, this);
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