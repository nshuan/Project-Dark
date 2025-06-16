using NeatBullets.Core.Scripts.SODefinitions;
using NeatBullets.Core.Scripts.WeaponSystem;
using UnityEngine;

namespace NeatBullets.WeaponDemo.Scripts {
    public class DemoWeapon : AbstractWeapon {
        
        private void Awake() {
            if (ProjectilesParentTransform == null)
                ProjectilesParentTransform = GameObject.Find("TemporalObjects").transform;
            
            ProjectileSpawnPoint = transform.Find("ProjectileSpawnPoint");
        }

        private void SubscribeToEvents(WeaponParamsSO weaponSO) {
            weaponSO.UpdateParamsEvent += InitializeParams;
            weaponSO.LaunchCoordinateSystemsEvent += LaunchCoordinateSystems;
            weaponSO.EnableCoroutinesEvent += EnableCoroutines;
        }
        
        private void UnsubscribeFromEvents(WeaponParamsSO weaponSO) {
            weaponSO.UpdateParamsEvent -= InitializeParams;
            weaponSO.LaunchCoordinateSystemsEvent -= LaunchCoordinateSystems;
            weaponSO.EnableCoroutinesEvent -= EnableCoroutines;
        }

        private void OnDestroy() {
            UnsubscribeFromEvents(_weaponSO);
        }

        public void UpdateWeaponSO(WeaponParamsSO weaponSO) {
            SubscribeToEvents(weaponSO);
           
            if (_weaponSO != null) {
                UnsubscribeFromEvents(_weaponSO);
                _weaponSO.DestroyProjectilesEvent?.Invoke();
            }
            
            StopAllCoroutines();
            
            _weaponSO = weaponSO;
            base.InitializeParams();
            
            FireCoroutine = StartCoroutine(Fire());
        }
        
        public void LaunchForward() {
            foreach (CoordinateSystem system in CoordinateSystems) {
                system.IsMoving = true;
                system.MoveSpeed = 8f;
                system.MoveAcc = 15f;
                system.Direction = ProjectileSpawnPoint.up;

                system.IsRotating = false;
            }
        }
        
        
    }
}
