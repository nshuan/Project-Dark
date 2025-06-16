using NeatBullets.Core.Scripts.Interfaces;
using UnityEngine;

namespace NeatBullets.ShadowSurvival.Scripts.EnemiesBehaviour.EnemyStates {

    public class ChasingPlayerState : IState {
        private EnemyController _enemy;

        public ChasingPlayerState(EnemyController enemy) {
            _enemy = enemy;
        }

        public void Enter() {
            _enemy.Animator.SetBool(_enemy.StateMachine.ChasingPlayerStateId, true);
            _enemy.Weapon.Animator.SetBool(_enemy.StateMachine.IdleStateId, true);
        }

        public void Update() {
        }

        public void FixedUpdate() {
            ChasePlayer();
        }

        private float _distanceToPlayer;

        public void LateUpdate() {
            if (_moveVec.x != 0)
                _enemy.SpriteRenderer.flipX = _moveVec.x < 0;

            if (_enemy.CommonVariables.IsPaused)
                _enemy.StateMachine.TransitionTo(_enemy.StateMachine.Pause);

            if (_enemy.HealthPoints <= 0)
                _enemy.StateMachine.TransitionTo(_enemy.StateMachine.Dead);

            _distanceToPlayer = Vector2.Distance(_enemy.Player.transform.position, _enemy.transform.position);
            if (_distanceToPlayer < _enemy.AttackDistance)
                _enemy.StateMachine.TransitionTo(_enemy.StateMachine.Shooting);
        }

        public void Exit() {
            _enemy.Animator.SetBool(_enemy.StateMachine.ChasingPlayerStateId, false);
            _enemy.Weapon.Animator.SetBool(_enemy.StateMachine.IdleStateId, false);
            _enemy.Rigidbody.velocity = Vector2.zero;
        }


        private Vector2 _moveVec;

        private void ChasePlayer() {
            _moveVec = _enemy.Player.transform.position - _enemy.transform.position;
            _moveVec = _moveVec.normalized * _enemy.MovementSpeed;
            _enemy.Rigidbody.velocity = _moveVec;
        }

    }
}