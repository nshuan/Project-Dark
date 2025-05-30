using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame
{
    public class EnemyEntity : MonoBehaviour
    {
        public Transform Target { get; set; }
        public float AttackRange => GameStats.CalculateStat(LevelManager.Instance.GameStats.eBaseAttackRange); 
        public float MoveSpeed => GameStats.CalculateStat(LevelManager.Instance.GameStats.eBaseMoveSpeed);
        public float AttackCd => GameStats.CalculateStat(LevelManager.Instance.GameStats.eBaseAttackCd);
        public float MaxHealth { get; private set; }
        private float CurrentHealth { get; set; }
        
        [FormerlySerializedAs("health")]
        [Space, Header("Visual")] 
        [SerializeField] private Transform uiHealth;
        
        private bool inAttackRange;
        private Coroutine attackCoroutine;

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
            {
                transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * MoveSpeed);
                transform.rotation = Quaternion.Euler(Vector3.Lerp(transform.eulerAngles,
                    transform.eulerAngles + new Vector3(0f, 0f, 180f), Time.deltaTime * MoveSpeed));
                inAttackRange = false;
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
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                OnDie();
                UIDie();
            }
            else
            {
                UIUpdateHealth();
            }
        }
        
        private void OnDie()
        {
            
        }
        
        private void UIUpdateHealth()
        {
            DOTween.Complete(this);
            var seq = DOTween.Sequence(this);
            seq.Append(transform.DOPunchScale(0.5f * Vector3.one, 0.2f))
                .Join(uiHealth.DOScaleY(Mathf.Clamp(CurrentHealth, 0f, MaxHealth) / MaxHealth, 0.2f));
            seq.Play();
        }

        private void UIDie()
        {
            Destroy(gameObject);
        }

    }
}