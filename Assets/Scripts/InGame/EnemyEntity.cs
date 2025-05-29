using System;
using System.Collections;
using UnityEngine;

namespace InGame
{
    public class EnemyEntity : MonoBehaviour
    {
        public Transform Target { get; set; }
        [field: SerializeField] public float AttackRange { get; set; } = 2f; 
        [field: SerializeField] public float MoveSpeed { get; set; } = 10f;
        [field: SerializeField] public float AttackCd { get; set; } = 2f;

        private bool inAttackRange;
        private Coroutine attackCoroutine;

        private void Start()
        {
            StartAttackCoroutine();
        }

        private void Update()
        {
            if (Target == null) return;
            MoveTo(Target);
        }

        private void MoveTo(Transform target)
        {
            if (Vector3.Distance(transform.position, target.position) < AttackRange)
                inAttackRange = true;
            else
            {
                transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * MoveSpeed);
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

        public void OnHit()
        {
            gameObject.SetActive(false);
        }
    }
}