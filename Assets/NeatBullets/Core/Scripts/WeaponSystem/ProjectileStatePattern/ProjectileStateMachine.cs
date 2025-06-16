using System;
using System.Collections;
using NeatBullets.Core.Scripts.Interfaces;
using NeatBullets.Core.Scripts.WeaponSystem.ProjectileStatePattern.States;
using NeatBullets.Core.Scripts.WeaponSystem.ProjectileStatePattern.States.Reflection;
using UnityEngine;

namespace NeatBullets.Core.Scripts.WeaponSystem.ProjectileStatePattern {
    [Serializable]
    public class ProjectileStateMachine {
        public IState CurrentState { get; private set; }
        public IState PreviousState { get; private set; }
        
        public ControlledState Controlled;
        public InitialFlightState InitialFlight;
        public PauseState Pause;
        
        public CircleReflectionState CircleReflection;
        public ReflectionPolarState ConeReflection;
        public RectReflectionState RectReflection;
    
        public event Action<IState> StateChanged;
        
        public ProjectileStateMachine(Projectile projectile) {
            
            Controlled = new ControlledState(projectile);
            InitialFlight = new InitialFlightState(projectile);
            Pause = new PauseState(projectile);

            CircleReflection = new CircleReflectionState(projectile);
            ConeReflection = new ReflectionPolarState(projectile);
            RectReflection = new RectReflectionState(projectile);
        }
        
        public void Initialize(IState initialState) {
            CurrentState = initialState;
            initialState.Enter();
        
            StateChanged?.Invoke(initialState);
        }
    
        public void TransitionTo(IState nextState) {
            CurrentState.Exit();
            
            PreviousState = CurrentState;
            CurrentState = nextState;
            
            nextState.Enter();
        
            StateChanged?.Invoke(nextState);
        }
    
        // coroutine for delayed transition
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
