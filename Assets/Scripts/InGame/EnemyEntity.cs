using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame
{
    [RequireComponent(typeof(EnemyMovementBehaviour))]
    public class EnemyEntity : MonoBehaviour
    {
        public Transform Target { get; set; }
        public float AttackRange => GameStats.CalculateStat(LevelManager.Instance.GameStats.eBaseAttackRange); 
        public float MoveSpeed => GameStats.CalculateStat(LevelManager.Instance.GameStats.eBaseMoveSpeed);
        public float AttackCd => GameStats.CalculateStat(LevelManager.Instance.GameStats.eBaseAttackCd);
        public float MaxHealth { get; private set; }
        private float CurrentHealth { get; set; }
        public bool IsDead => CurrentHealth <= 0;
        public Action OnDead { get; set; }
        
        // Elemental effect
        public bool IsInLightning { get; set; }
        
        [Space, Header("Visual")] 
        [SerializeField] private Transform uiHealth;

        private EnemyMovementBehaviour movementBehaviour;
        
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
            
            // Update health ui
            UIUpdateHealth();
        }

        private void Update()
        {
            if (!Target) return;
            MoveTo(Target);
        }

        private void MoveTo(Transform target)
        {
            if (Vector3.Distance(transform.position, target.position) < AttackRange)
                inAttackRange = true;
            else
                movementBehaviour.Move(target, MoveSpeed, AttackRange);
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
                    yield return new WaitForSeconds(AttackCd);
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
            EnemyPool.Instance.Release(this);
        }
        
        private void UIUpdateHealth()
        {
            DOTween.Complete(this);
            var seq = DOTween.Sequence(this);
            seq.Append(transform.DOPunchScale(0.5f * Vector3.one, 0.2f))
                .Join(uiHealth.DOScaleY(Mathf.Clamp(CurrentHealth, 0f, MaxHealth) / MaxHealth, 0.2f));
            seq.Play();
        }
    }
}