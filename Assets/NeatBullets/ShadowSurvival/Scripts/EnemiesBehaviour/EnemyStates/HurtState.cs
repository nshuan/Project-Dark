using NeatBullets.Core.Scripts.Interfaces;

namespace NeatBullets.ShadowSurvival.Scripts.EnemiesBehaviour.EnemyStates {
    public class HurtState : IState {

        private EnemyController _enemy;

        public HurtState(EnemyController enemy) {
            _enemy = enemy;
        }


        public void Enter() {
            _enemy.Animator.SetBool(_enemy.StateMachine.HurtStateId, true);
            _enemy.Weapon.Animator.SetBool(_enemy.StateMachine.HurtStateId, true);

            _enemy.StartCoroutine(_enemy.StateMachine.TransitionTo(_enemy.StateMachine.Shooting, 0.15f));
        }

        public void Update() {
        }

        public void FixedUpdate() {
        }

        public void LateUpdate() {
        }

        public void Exit() {
            _enemy.Weapon.Animator.SetBool(_enemy.StateMachine.HurtStateId, false);
            _enemy.Animator.SetBool(_enemy.StateMachine.HurtStateId, false);
        }



    }
}