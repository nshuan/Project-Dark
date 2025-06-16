using NeatBullets.Core.Scripts.Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NeatBullets.ShadowSurvival.Scripts.EnemiesBehaviour.EnemyStates {
    public class DeadState : IState {
        private EnemyController _enemy;

        public DeadState(EnemyController enemy) {
            _enemy = enemy;
        }

        public void Enter() {
            _enemy.Animator.SetBool(_enemy.StateMachine.DeadStateId, true);
            _enemy.Weapon.Animator.SetBool(_enemy.StateMachine.IdleStateId, true);
            _enemy.Rigidbody.velocity = Vector2.zero;
            _enemy.Collider.enabled = false;
            Object.Destroy(_enemy.gameObject, 2f);
        }

        public void Update() {
        }

        public void FixedUpdate() {
        }

        public void LateUpdate() {
        }

        public void Exit() {
        }


    }
}