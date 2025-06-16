using System.Collections;
using NeatBullets.Core.Scripts.Interfaces;
using UnityEngine;

namespace NeatBullets.ShadowSurvival.Scripts.EnemiesBehaviour.EnemyStates {
    public class ShootingState : IState {
        private EnemyController _enemy;
        private const float _durationOfShootingState = 0.517f;
        private const float _durationOfAimState = 0.767f;

        public ShootingState(EnemyController enemy) {
            _enemy = enemy;
        }

        public void Enter() {
            _aimAndShootCoroutine = _enemy.StartCoroutine(AimAndShoot(_durationOfAimState, _durationOfShootingState));
        }

        public void Update() {
        }

        private Vector2 _playerDirection;

        public void FixedUpdate() {
            _enemy.Rigidbody.velocity = Vector2.zero;
            _playerDirection = _enemy.Player.transform.position - _enemy.transform.position;
            _distanceToPlayer = _playerDirection.magnitude;
        }


        private float _distanceToPlayer;

        public void LateUpdate() {
            if (_playerDirection.x != 0) {
                _enemy.SpriteRenderer.flipX = _playerDirection.x < 0;
                _enemy.Weapon.SpriteRenderer.flipY = _playerDirection.x < 0;
            }

            _enemy.Weapon.transform.right = _playerDirection.normalized;


            if (_enemy.HealthPoints <= 0)
                _enemy.StateMachine.TransitionTo(_enemy.StateMachine.Dead);

            if (_enemy.CommonVariables.IsPaused)
                _enemy.StateMachine.TransitionTo(_enemy.StateMachine.Pause);

            if (_distanceToPlayer > _enemy.AttackDistance * 1.3f)
                _enemy.StateMachine.TransitionTo(_enemy.StateMachine.ChasingPlayer);
        }

        public void Exit() {
            _enemy.Animator.SetBool(_enemy.StateMachine.ShootingStateId, false);
            _enemy.Animator.SetBool(_enemy.StateMachine.AimStateId, false);

            _enemy.Weapon.Animator.SetBool(_enemy.StateMachine.ShootingStateId, false);
            _enemy.Weapon.Animator.SetBool(_enemy.StateMachine.AimStateId, false);
            _enemy.StopCoroutine(_aimAndShootCoroutine);
        }


        private Coroutine _aimAndShootCoroutine;

        private IEnumerator AimAndShoot(float timeToAim, float timeToShoot) {
            while (true) {

                _enemy.Animator.SetBool(_enemy.StateMachine.AimStateId, true);
                _enemy.Weapon.Animator.SetBool(_enemy.StateMachine.AimStateId, true);

                // time delay to play aim animation
                yield return new WaitForSeconds(timeToAim);
                _enemy.Animator.SetBool(_enemy.StateMachine.AimStateId, false);
                _enemy.Weapon.Animator.SetBool(_enemy.StateMachine.AimStateId, false);
                // aim animation is over


                // shooting animation starts here
                _enemy.Animator.SetBool(_enemy.StateMachine.ShootingStateId, true);
                _enemy.Weapon.Animator.SetBool(_enemy.StateMachine.ShootingStateId, true);


                // shooting logic
                _enemy.Weapon.ShootProjectile(_playerDirection);


                // time delay to play shooting animation
                yield return new WaitForSeconds(timeToShoot);
                _enemy.Animator.SetBool(_enemy.StateMachine.ShootingStateId, false);
                _enemy.Weapon.Animator.SetBool(_enemy.StateMachine.ShootingStateId, false);
            }
        }


    }
}