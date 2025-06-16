using NeatBullets.Core.Scripts.Interfaces;
using UnityEngine;

namespace NeatBullets.Core.Scripts.WeaponSystem.ProjectileStatePattern.States {
    
public class PauseState : IState {
    private Projectile _projectile;
    
    public PauseState(Projectile projectile) {
        _projectile = projectile;
    }
    
    private Vector2 _prevVelocity;
    public void Enter() {
        _prevVelocity = _projectile.Rigidbody.velocity;
        _projectile.Rigidbody.velocity = Vector2.zero;
        
        // Do not count lifetime for paused projectile
        if (_projectile.WeaponParamsLocal.HasLifespan)
            _projectile.StopCoroutine(_projectile.DestroyItselfCoroutine);
    }

    public void Update() {
    }

    public void FixedUpdate() {
    }

    
    public void LateUpdate() {
        if (!_projectile.GlobalVariables.IsPaused)
            _projectile.StateMachine.TransitionTo(_projectile.StateMachine.PreviousState);
    }

    public void Exit() {
        _projectile.Rigidbody.velocity = _prevVelocity;

        if (!_projectile.WeaponParamsLocal.HasLifespan)
            return;
        
        float remainedLifespan = _projectile.WeaponParamsLocal.Lifespan -
                                 (_projectile.GlobalVariables.PauseTime - _projectile.BirthTime);
        _projectile.DestroyItselfCoroutine = _projectile.StartCoroutine(_projectile.DestroyItself(remainedLifespan));

        _projectile.BirthTime = Time.time;
    }
}

}