using System.Collections;
using Dark.Scripts.Utils;
using UnityEngine;

namespace InGame.Crow
{
    public class EnemyCrowEntity : EnemyEntity
    {
        [Space] [Header("Crow")] 
        [SerializeField] private float attackMoveSpeed = 5f;
        [SerializeField] private float distanceToExplode = 1f;
        [SerializeField] private float delayExplode = 0.5f;
        
        private bool hasAttack = false;
        
        protected override IEnumerator IEAttack()
        {
            hasAttack = false;
            
            while (true)
            {
                if (!inAttackRange)
                    yield return new WaitUntil(() => inAttackRange);
                else
                {
                    if (hasAttack) yield break;
                    
                    animController.PlayAttack();
                    StartCoroutine(IECrowAttack());
                    break;
                }
            }  
        }

        private IEnumerator IECrowAttack()
        {
            while (Vector3.Distance(transform.position, Target.position) > distanceToExplode)
            {
                config.moveBehaviour.MoveNonAlloc(transform, Target.position, Vector2.zero, distanceToExplode, attackMoveSpeed, ref direction);
                yield return new WaitForEndOfFrame();
            }

            hasAttack = true;
            yield return new WaitForSeconds(delayExplode);
            if (TargetTower.IsDestroyed) yield break;
            config.attackBehaviour.Attack(this, TargetTower, transform.position, LevelUtilityV2.ToInt(CurrentDamage * TempDmgScale));
        }
    }
}