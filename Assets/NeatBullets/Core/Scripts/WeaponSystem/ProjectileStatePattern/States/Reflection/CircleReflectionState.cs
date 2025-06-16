using NeatBullets.Core.Scripts.Interfaces;
using Unity.Mathematics;
using UnityEngine;

namespace NeatBullets.Core.Scripts.WeaponSystem.ProjectileStatePattern.States.Reflection {
    public class CircleReflectionState : IState {
        
        private Projectile _projectile;
        public CircleReflectionState(Projectile projectile) {
            _projectile = projectile;
        }
        
        private WeaponParams _weaponParams;
        private bool _reflected;
        public void Enter() {
            _weaponParams = _projectile.WeaponParamsLocal;
            _projectile.SpriteRenderer.color = Color.white;
            _reflected = false;
        }
        
        public void Update() { }
        
        private Vector2 _normal;
        public void FixedUpdate() {
            _normal = (_projectile.OriginTransform.position - _projectile.transform.position).normalized;
            
            if ( !_reflected && 
                 _projectile.DistanceFromOrigin > _weaponParams.NNControlDistance * _weaponParams.ReflectiveCircleRadius ) {
                
                _projectile.Rigidbody.velocity = Vector2.Reflect(_projectile.Rigidbody.velocity, _normal);
                
                if (_weaponParams.FlipXOnReflect)
                    _projectile.SignX *= -1f;
                if (_weaponParams.FlipYOnReflect)
                    _projectile.SignY *= -1f;
                
                _reflected = true;
            }
            
            if (_projectile.DistanceFromOrigin < _weaponParams.NNControlDistance * _weaponParams.ReflectiveCircleRadius)
                _reflected = false;
        }

        
        public void LateUpdate() {
            if (_projectile.DistanceFromOrigin < _weaponParams.NNControlDistance * math.SQRT2) 
                _projectile.StateMachine.TransitionTo(_projectile.StateMachine.Controlled);
        }

        public void Exit() {
        }
    }
}
