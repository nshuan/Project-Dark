using System.Collections;
using NeatBullets.Core.Scripts.Interfaces;
using NeatBullets.SpaceShooter.Scripts.Managers;
using UnityEngine;

namespace NeatBullets.SpaceShooter.Scripts.EnemyBehaviour.States {
    public class ShootingState : IState {
        
        private EnemyController _enemy;
        public ShootingState(EnemyController enemy) { _enemy = enemy; }

        private const float _durationOfShootingState = 0.5f;
        private const float _durationOfAimState = 0.5f;
        public void Enter() {
            _aimAndShootCoroutine = _enemy.StartCoroutine(AimAndShoot(_durationOfAimState, _durationOfShootingState));
        }

        public void Update() { }

        private Vector2 _playerDirection;
        public void FixedUpdate() {
            _enemy.Rigidbody.velocity = Vector2.zero;
            _playerDirection = _enemy.Player.transform.position - _enemy.transform.position;
            _distanceToPlayer = _playerDirection.magnitude;
        }


        private float _distanceToPlayer;
        public void LateUpdate() {
            _enemy.Weapon.transform.up = _playerDirection.normalized;
            
            if (_enemy.HealthPoints <= 0)
                _enemy.StateMachine.TransitionTo(_enemy.StateMachine.Dead);
        
            if (_enemy.GlobalVariables.IsPaused)
                _enemy.StateMachine.TransitionTo(_enemy.StateMachine.Pause);
        
            if (_distanceToPlayer > _enemy.AttackDistance * 1.3f)
                _enemy.StateMachine.TransitionTo(_enemy.StateMachine.ChasingPlayer);
        }
        
        
        public void Exit() {
            _enemy.StopCoroutine(_aimAndShootCoroutine);
        }
        
        
        private Coroutine _aimAndShootCoroutine;
        private IEnumerator AimAndShoot(float timeToAim, float timeToShoot) {
            while (true) {
                yield return new WaitForSeconds(timeToAim);
                
                // shooting logic
                AudioManager.Instance.PlayAudioEffect(_enemy.AudioSource, AudioManager.Instance.EnemyShoot);
                _enemy.Weapon.FireMultiShot();
                
                yield return new WaitForSeconds(timeToShoot);
               
            }
        }
        
        
    }
}