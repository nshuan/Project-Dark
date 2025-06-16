using System;
using UnityEngine;

namespace NeatBullets.Core.Scripts.WeaponSystem {
    
    public class CoordinateSystem : MonoBehaviour {
        
        public AbstractWeapon Weapon;
        
        private Transform _transform;
        
        public bool IsMoving;
        public float MoveSpeed;
        public float MoveAcc;
        public float CurMoveSpeed;
        
        public bool IsRotating;
        public float RotatingSpeed;
        public float RotationAcc;
        public float CurRotationSpeed;
        private void Awake() {
            _transform = transform;
            IsMoving = false;
            MoveSpeed = 10f;
            MoveAcc = 15f;
            CurMoveSpeed = 0f;
            
            IsRotating = false;
            RotatingSpeed = 45f;
            RotationAcc = 15f;
            CurRotationSpeed = 0f;
        }

        private void Start() {
            InvokeRepeating(nameof(DestroyItselfIfChildless), 0.2f, 0.2f);
        }
        
        private void OnDestroy() {
            Weapon.CoordinateSystems.Remove(this);
        }
        
        // if all child projectiles were destroyed => destroy this CoordinateSystem
        private void DestroyItselfIfChildless() {
            if (_transform.childCount == 0)
                Destroy(gameObject);
        }

        private void LateUpdate() {
            Move();
            Rotate();
        }
        
        
        
        public Vector3 Direction;
        private void Move() {
            if (!IsMoving) 
                return;
            
            if (CurMoveSpeed < MoveSpeed)
                CurMoveSpeed += MoveAcc * Time.deltaTime;
            
            _transform.position += Direction * (CurMoveSpeed * Time.deltaTime);
        }
        
        private void Rotate() {
            if (!IsRotating)
                return;
            
            if (Mathf.Abs(CurRotationSpeed) < Mathf.Abs(RotatingSpeed))
                CurRotationSpeed += RotationAcc * Time.deltaTime;
            
            _transform.rotation *= Quaternion.Euler(0f, 0f, 1f * CurRotationSpeed * Time.deltaTime);
        }
    }
}
