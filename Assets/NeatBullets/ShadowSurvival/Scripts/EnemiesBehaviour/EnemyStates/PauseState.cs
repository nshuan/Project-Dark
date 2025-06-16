using NeatBullets.Core.Scripts.Interfaces;
using UnityEngine;

namespace NeatBullets.ShadowSurvival.Scripts.EnemiesBehaviour.EnemyStates {
    public class PauseState : IState {

        private EnemyController _enemy;

        public PauseState(EnemyController enemy) {
            _enemy = enemy;
        }

        public void Enter() {
            _enemy.Rigidbody.velocity = Vector2.zero;
            _enemy.Animator.enabled = false;
            _enemy.Weapon.Animator.enabled = false;
        }

        public void Update() {
        }

        public void FixedUpdate() {
        }

        public void LateUpdate() {
            if (_enemy.CommonVariables.IsPaused == false)
                _enemy.StateMachine.TransitionTo(_enemy.StateMachine.ChasingPlayer);
        }

        public void Exit() {
            _enemy.Animator.enabled = true;
            _enemy.Weapon.Animator.enabled = true;
        }
    }
}