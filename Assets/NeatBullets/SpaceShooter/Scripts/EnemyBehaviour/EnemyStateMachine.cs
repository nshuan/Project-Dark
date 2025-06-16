using System;
using System.Collections;
using NeatBullets.Core.Scripts.Interfaces;
using NeatBullets.SpaceShooter.Scripts.EnemyBehaviour.States;
using UnityEngine;

namespace NeatBullets.SpaceShooter.Scripts.EnemyBehaviour {
    public class EnemyStateMachine {
        
        public IState CurrentState;
    
        public ChasingPlayerState ChasingPlayer;
        public DeadState Dead;
        public ShootingState Shooting;
        public PauseState Pause;
        public HurtState Hurt;

        public event Action<IState> StateChanged;
        public EnemyStateMachine(EnemyController enemy) {
        
            this.ChasingPlayer = new ChasingPlayerState(enemy);
            this.Dead = new DeadState(enemy);
            this.Shooting = new ShootingState(enemy);
            this.Pause = new PauseState(enemy);
            this.Hurt = new HurtState(enemy);
        }
        
        public void Initialize(IState state) {
            CurrentState = state;
            state.Enter();
        
            StateChanged?.Invoke(state);
        }
    
        public void TransitionTo(IState nextState) {
            CurrentState.Exit();
            CurrentState = nextState;
            nextState.Enter();
        
            StateChanged?.Invoke(nextState);
        }
    
        public IEnumerator TransitionTo(IState nextState, float delay) {
            yield return new WaitForSeconds(delay);
            TransitionTo(nextState);
        }
    
        public void Update() {
            CurrentState?.Update();
        }

        public void FixedUpdate() {
            CurrentState?.FixedUpdate();
        }

        public void LateUpdate() {
            CurrentState?.LateUpdate();
        }
    }
}
