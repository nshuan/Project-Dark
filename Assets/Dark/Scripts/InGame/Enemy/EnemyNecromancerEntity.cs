using System;
using Dark.Scripts.Settings;
using Dark.Scripts.Utils;
using UnityEngine;

namespace InGame
{
    public class EnemyNecromancerEntity : EnemyEntity
    {
        [Space] [Header("Necromancer")] 
        [SerializeField] private float delayMove = 1f;

        private Action actionDelayMove;
        
        public override void Activate(float delayStartAttack = 0)
        {
            actionDelayMove = () =>
            {
                State = EnemyState.Move;
                animController.PlayRun();
            };
                
            config.Spawn(this, delayStartAttack, () =>
            {
                StartAttackCoroutine();
                State = EnemyState.Freeze;
                boidAgent.IsActive = true;
                collider2d.enabled = true;
                Activated = true;
                healthBar.gameObject.SetActive(!IsBoss ? GameSettings.ShowEnemyHealth : GameSettings.ShowBossHealth);

                this.DelayCall(delayMove, () =>
                {
                    actionDelayMove?.Invoke();
                });
            });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            actionDelayMove = null;
        }
    }
}