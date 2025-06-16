using NeatBullets.Core.Scripts.Interfaces;
using UnityEngine;

namespace NeatBullets.Core.Scripts.WeaponSystem.ProjectileStatePattern.States.Reflection {
    public class RectReflectionState : IState {
        
        private Projectile _projectile;
        public RectReflectionState(Projectile projectile) {
            _projectile = projectile;
        }

        private WeaponParams _weaponParams;
        private bool _reflectedByBorderX, _reflectedByBorderY;
        public void Enter() {
            _weaponParams = _projectile.WeaponParamsLocal;
            
            _projectile.SpriteRenderer.color = Color.white;
            _reflectedByBorderX = false;
            _reflectedByBorderY = false;
            
        }
        
        public void Update() { }
        
        
        private float _maxX, _maxY;
        private Vector2 _normal;
        public void FixedUpdate() {
            _maxX = _weaponParams.RectDimensions.x * _weaponParams.NNControlDistance;
            _maxY = _weaponParams.RectDimensions.y * _weaponParams.NNControlDistance;


            if (!_reflectedByBorderX &&
                Mathf.Abs(_projectile.RelativePos.x) > _maxX) {

                _normal =_projectile.OriginTransform.right.normalized;
                
                _projectile.Rigidbody.velocity = Vector2.Reflect(_projectile.Rigidbody.velocity, _normal);

                if (_weaponParams.FlipXOnReflect)
                    _projectile.SignX *= -1f;
                
                if (_weaponParams.FlipYOnReflect)
                    _projectile.SignY *= -1f;
                
                _reflectedByBorderX = true;
            }

            if (Mathf.Abs(_projectile.RelativePos.x) < _maxX)
                _reflectedByBorderX = false;

            if (!_reflectedByBorderY &&
                Mathf.Abs(_projectile.RelativePos.y) > _maxY) {

                _normal = _projectile.OriginTransform.up.normalized;
                
                _projectile.Rigidbody.velocity = Vector2.Reflect(_projectile.Rigidbody.velocity, _normal);

                if (_weaponParams.FlipXOnReflect)
                    _projectile.SignX *= -1f;
                
                if (_weaponParams.FlipYOnReflect)
                    _projectile.SignY *= -1f;
                
                _reflectedByBorderY = true;
            }
            
            if (Mathf.Abs(_projectile.RelativePos.y) < _maxY)
                _reflectedByBorderY = false;

        }

        
        public void LateUpdate() {
            if (Mathf.Abs(_projectile.RelativePos.x) < _maxX && 
                Mathf.Abs(_projectile.RelativePos.y) < _maxY) 
                
                _projectile.StateMachine.TransitionTo(_projectile.StateMachine.Controlled);
        }

        public void Exit() {
        }
    }
}