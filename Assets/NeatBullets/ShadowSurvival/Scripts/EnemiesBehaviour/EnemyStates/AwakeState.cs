using NeatBullets.Core.Scripts.Interfaces;

namespace NeatBullets.ShadowSurvival.Scripts.EnemiesBehaviour.EnemyStates {

    public class AwakeState : IState {
        private EnemyController _enemy;

        public AwakeState(EnemyController enemy) {
            _enemy = enemy;
        }

        public void Enter() {
            float durationOfAwakeState = _enemy.Animator.GetCurrentAnimatorStateInfo(0).length;
            _enemy.StartCoroutine(_enemy.StateMachine.TransitionTo(_enemy.StateMachine.ChasingPlayer,
                durationOfAwakeState));
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