using NeatBullets.Core.Scripts.Interfaces;
using UnityEngine;

namespace NeatBullets.Core.Scripts.WeaponSystem.ProjectileStatePattern.States.Reflection {
    public class ReflectionPolarState : IState {
        private Projectile _projectile;
        
        
        public ReflectionPolarState(Projectile projectile) {
            _projectile = projectile;
        }

        private WeaponParams _weaponParams;
        private float _initialFlightRadius; 
        private float _borderDir_x, _borderDir_y;
        private Vector2 _upperBorder, _lowerBorder;
        public void Enter() {
            _weaponParams = _projectile.WeaponParamsLocal;
            _initialFlightRadius = _weaponParams.NNControlDistance * _weaponParams.InitialFlightRadius;
            
            _projectile.SpriteRenderer.color = Color.white;
            _reflectedByInitialCircle = false;
            _reflectedByBorder = false;
            
            _borderDir_x = Mathf.Sin(_weaponParams.MaxPolarAngleDeg * Mathf.Deg2Rad);
            _borderDir_y = Mathf.Cos(_weaponParams.MaxPolarAngleDeg * Mathf.Deg2Rad);

            _upperBorder= _projectile.OriginTransform.TransformVector(new Vector2(- _borderDir_x, _borderDir_y));
            _lowerBorder = _projectile.OriginTransform.TransformVector(new Vector2(_borderDir_x, _borderDir_y));
        }
        
        public void Update() {
            
        }
        
        
        private Vector2 _normal;
        private bool _reflectedByInitialCircle;
        private bool _reflectedByBorder;
        public void FixedUpdate() {
            
            if (!_reflectedByBorder && 
                _projectile.PhiRad > _weaponParams.MaxPolarAngleDeg * Mathf.Deg2Rad) {
                
                _normal = _projectile.RelativePos.x < 0 
                    ? - Vector2.Perpendicular(_upperBorder).normalized
                    : Vector2.Perpendicular(_lowerBorder).normalized;
                
                _projectile.Rigidbody.velocity = Vector2.Reflect(_projectile.Rigidbody.velocity, _normal);
                
                if (_weaponParams.FlipXOnReflect)
                    _projectile.SignX *= -1f;
                if (_weaponParams.FlipYOnReflect)
                    _projectile.SignY *= -1f;
                
                _reflectedByBorder = true;
            }

            if (_projectile.PhiRad < _weaponParams.MaxPolarAngleDeg * Mathf.Deg2Rad)
                _reflectedByBorder = false;
            
            
            if (!_reflectedByInitialCircle && 
                _projectile.DistanceFromOrigin < _initialFlightRadius) {
                
                _normal = (_projectile.OriginTransform.position - _projectile.transform.position).normalized;
                
                _projectile.Rigidbody.velocity = Vector2.Reflect(_projectile.Rigidbody.velocity, _normal);
                
                // always FlipY on reflect by InitialCircle
                _projectile.SignY *= -1f;
                
                _reflectedByInitialCircle = true;
            }
            
            if (_projectile.DistanceFromOrigin > _initialFlightRadius)
                _reflectedByInitialCircle = false;
            
        }

        
        public void LateUpdate() {
            if (_projectile.PhiRad < _weaponParams.MaxPolarAngleDeg * Mathf.Deg2Rad && 
                _projectile.DistanceFromOrigin < _weaponParams.NNControlDistance &&
                _projectile.DistanceFromOrigin > _initialFlightRadius)
                
                _projectile.StateMachine.TransitionTo(_projectile.StateMachine.Controlled);
        }

        public void Exit() {
        }
    }
}
