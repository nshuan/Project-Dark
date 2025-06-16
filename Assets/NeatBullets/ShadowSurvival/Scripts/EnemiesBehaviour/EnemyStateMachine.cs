using System;
using System.Collections;
using NeatBullets.Core.Scripts.Interfaces;
using NeatBullets.ShadowSurvival.Scripts.EnemiesBehaviour.EnemyStates;
using UnityEngine;

namespace NeatBullets.ShadowSurvival.Scripts.EnemiesBehaviour {
    public class EnemyStateMachine {
    
        public readonly int AimStateId;
        public readonly int ShootingStateId;
        public readonly int IdleStateId;
        public readonly int DeadStateId;
        public readonly int HurtStateId;
        public readonly int ChasingPlayerStateId;

        public IState CurrentState;
    
        public ChasingPlayerState ChasingPlayer;
        public DeadState Dead;
        public ShootingState Shooting;
        public PauseState Pause;
        public HurtState Hurt;
        public AwakeState Awake;

        public event Action<IState> StateChanged;
        public EnemyStateMachine(EnemyController enemy) {
        
            this.ChasingPlayer = new ChasingPlayerState(enemy);
            this.Dead = new DeadState(enemy);
            this.Shooting = new ShootingState(enemy);
            this.Pause = new PauseState(enemy);
            this.Hurt = new HurtState(enemy);
            this.Awake = new AwakeState(enemy);
        
            AimStateId = Animator.StringToHash("AimState");
            ShootingStateId = Animator.StringToHash("ShootingState");
            IdleStateId = Animator.StringToHash("IdleState");
            DeadStateId = Animator.StringToHash("DeadState");
            HurtStateId = Animator.StringToHash("HurtState");
            ChasingPlayerStateId = Animator.StringToHash("ChasingPlayerState");
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
