using System;
using System.Collections;
using System.Linq;
using NeatBullets.Core.Scripts.SODefinitions;
using NeatBullets.Core.Scripts.WeaponSystem;
using UnityEngine;

namespace NeatBullets.WeaponDemo.Scripts {
    public class CombinedWeapon : AbstractWeapon {
        
        [Header("Combined weapons")]
        [SerializeField] [HideInInspector] private int _id;
        [SerializeField] public WeaponParamsSO[] WeaponSoArr;
        public int WeaponCount => WeaponSoArr.Length;
        public string WeaponName => WeaponSoArr[_id].name;

        private void Awake() {
            if (ProjectilesParentTransform == null)
                ProjectilesParentTransform = GameObject.Find("TemporalObjects").transform;
            
            ProjectileSpawnPoint = transform.Find("ProjectileSpawnPoint");
        }
        
        private void SubscribeToEvents(WeaponParamsSO weaponSO) {
            weaponSO.UpdateParamsEvent += InitializeParams;
            weaponSO.EnableCoroutinesEvent += EnableCoroutines;
        }
        
        private void UnsubscribeFromEvents(WeaponParamsSO weaponSO) {
            weaponSO.UpdateParamsEvent -= InitializeParams;
            weaponSO.EnableCoroutinesEvent -= EnableCoroutines;
        }
        
        private void OnDestroy() {
            if (_weaponSO != null)
                UnsubscribeFromEvents(_weaponSO);
        }
        
        public void UpdateWeaponSO(int id) {
            _id = id;
            
            WeaponSoArr[_id].HasTrail = true;
            SubscribeToEvents(WeaponSoArr[_id]);
           
            if (_weaponSO != null) {
                UnsubscribeFromEvents(_weaponSO);
                _weaponSO.DestroyProjectilesEvent?.Invoke();
            }
            
            StopAllCoroutines();
            
            _weaponSO = WeaponSoArr[_id];
            base.InitializeParams();
            
            FireCoroutine = StartCoroutine(Fire());
        }
        

        public override IEnumerator Fire() {
            string methodName = "Fire" + _id;
            _shotsCount = 0;
            
            while (true) {
                yield return typeof(CombinedWeapon).GetMethod(methodName)!.Invoke(this, null);
            }
        }

        private int _shotsCount;
        
        // based on WP#68
        public IEnumerator Fire0() {
            
            base.FireMultiShot();
            yield return new WaitForSeconds(0.4f);
            
            CoordinateSystem lastSystem = CoordinateSystems.Last();
            
            lastSystem.IsMoving = true;
            lastSystem.MoveSpeed = 8f;
            lastSystem.MoveAcc = 12f;
            lastSystem.Direction = ProjectileSpawnPoint.up;
            
            yield return new WaitForSeconds(Mathf.Clamp01(_weaponParamsLocal.FireRate - 0.4f));
        }
        
        // based on WP#68
        public IEnumerator Fire1() {
            
            _weaponParamsLocal.HasLifespan = false;

            for (int i = 0; i < 6; i++) {
                base.FireMultiShot();
                yield return new WaitForSeconds(0.5f);
            }
            
            var systems = CoordinateSystems.TakeLast(6);

            foreach (var system in systems) {
                system.IsRotating = true;
                system.RotatingSpeed = 50f;
                system.RotationAcc = _shotsCount == 0 ? 25f : -25f;
            }
            
            yield return new WaitForSeconds(1f);
            
            _shotsCount += 6;
            if (_shotsCount > 11) 
                _weaponSO.EnableCoroutinesEvent.Invoke(false);
        }
        
        
        // based on WP#9
        public IEnumerator Fire2() {
            
            _weaponParamsLocal.ProjectilesInOneShot = 20;
            _weaponParamsLocal.Angle = 90f;
            base.FireMultiShot();
            yield return new WaitForSeconds(0.2f);
            
            _weaponParamsLocal.ProjectilesInOneShot = 10;
            _weaponParamsLocal.Angle = 45f;
            base.FireMultiShot();
            yield return new WaitForSeconds(0.4f);
            
            var systems = CoordinateSystems.TakeLast(2);

            foreach (var system in systems) {
                system.IsMoving = true;
                system.MoveSpeed = 6f;
                system.MoveAcc = 6f;
                system.Direction = ProjectileSpawnPoint.up;
            }
            
            yield return new WaitForSeconds(Mathf.Clamp01(_weaponParamsLocal.FireRate - 0.6f));
        }
        
        // based on WP#66
        public IEnumerator Fire3() {
            
            base.FireMultiShot();
            yield return new WaitForSeconds(0.2f);
            base.FireMultiShot();
            yield return new WaitForSeconds(0.2f);
            
            var systems = CoordinateSystems.TakeLast(2);

            foreach (var system in systems) {
                system.IsMoving = true;
                system.MoveSpeed = 6f;
                system.MoveAcc = 10f;
                system.Direction = ProjectileSpawnPoint.up;
            }
            
            yield return new WaitForSeconds(Mathf.Clamp01(_weaponParamsLocal.FireRate - 0.4f));
        }
        
        // based on WP#12
        public IEnumerator Fire4() {
            _weaponParamsLocal.HasLifespan = false;
            _weaponParamsLocal.SignY = 0f;
            _weaponParamsLocal.ForceRange = new Vector2(3f, 6f);
            
            base.FireMultiShot();
            yield return new WaitForSeconds(0.33f);
            
            var firstSystem = CoordinateSystems.Last();
            firstSystem.IsRotating = true;
            firstSystem.RotatingSpeed = 60f;
            firstSystem.RotationAcc = 30f;
            
            base.FireMultiShot();
            yield return new WaitForSeconds(0.33f);
            
            var secondSystem = CoordinateSystems.Last();
            secondSystem.IsRotating = true;
            secondSystem.RotatingSpeed = 60f;
            secondSystem.RotationAcc = -30f;
            
            _shotsCount += 2;
            if (_shotsCount > 10) 
                _weaponSO.EnableCoroutinesEvent.Invoke(false);
        }
        
        // based on WP#22
        public IEnumerator Fire5() {
            yield return base.FireBurst();
            yield return new WaitForSeconds(0.25f);
            foreach (var system in CoordinateSystems) {
                system.IsRotating = true;
                system.RotatingSpeed = 60f;
                system.RotationAcc = _shotsCount % 2 == 0 ? 30f : -30f;
            }
            _shotsCount += 1;
        }
        
        // based on WP#32
        public IEnumerator Fire6() {
            _weaponParamsLocal.HasLifespan = false;

            for (int i = 0; i < 6; i++) {
                base.FireMultiShot();
                yield return new WaitForSeconds(0.5f);
            }
            
            var systems = CoordinateSystems.TakeLast(6);

            foreach (var system in systems) {
                system.IsRotating = true;
                system.RotatingSpeed = 45f;
                system.RotationAcc = _shotsCount == 0 ? 20f : -20f;
            }
            
            yield return new WaitForSeconds(1f);
            
            _shotsCount += 6;
            if (_shotsCount > 11) 
                _weaponSO.EnableCoroutinesEvent.Invoke(false);
            
        }
        
        // based on WP#46
        public IEnumerator Fire7() {
            _weaponParamsLocal.HasLifespan = false;
            _weaponParamsLocal.SpeedRange = new Vector2(3f, 5f);
            
            base.FireMultiShot();
            yield return new WaitForSeconds(0.4f);
            
            var system = CoordinateSystems.Last();
            system.IsRotating = true;
            system.RotatingSpeed = 80f;
            system.RotationAcc = _shotsCount % 2 == 0 ? 40f : -40f;
            
            _shotsCount += 1;
            if (_shotsCount > 6) 
                _weaponSO.EnableCoroutinesEvent.Invoke(false);
            
        }
        
        // based on WP#46
        public IEnumerator Fire8() {
            _weaponParamsLocal.SpeedRange = new Vector2(2f, 5f);
            _weaponParamsLocal.NNControlDistance = 1.2f;
            _weaponParamsLocal.ProjectilesInOneShot = 12;
            
            base.FireMultiShot();
            yield return new WaitForSeconds(0.5f);
            
            var system = CoordinateSystems.Last();
            system.IsRotating = true;
            system.RotatingSpeed = 120f;
            system.RotationAcc = _shotsCount % 2 == 0 ? 80f : -80f;

            system.IsMoving = true;
            system.MoveSpeed = 6f;
            system.MoveAcc = 10f;
            system.Direction = ProjectileSpawnPoint.up;
            
            _shotsCount += 1;
        }
        
        // based on WP#51
        public IEnumerator Fire9() {
            _weaponParamsLocal.Lifespan = 5f;
            yield return base.FireBurst();
            foreach (var system in CoordinateSystems) {
                system.IsMoving = true;
                system.MoveSpeed = 6f;
                system.MoveAcc = 10f;
                system.Direction = ProjectileSpawnPoint.up;
            }
        }

        

    }
}
