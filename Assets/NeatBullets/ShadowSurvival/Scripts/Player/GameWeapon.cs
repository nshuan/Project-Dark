using NeatBullets.Core.Scripts.Interfaces;
using NeatBullets.Core.Scripts.WeaponSystem;
using UnityEngine;

namespace NeatBullets.ShadowSurvival.Scripts.Player {
    public class GameWeapon : AbstractWeapon
    {
        private void Awake() {
            base.OnAwakeFunc();
            _weaponSO.LoadParamsFromJson();
            _weaponSO.FireRate = 0.1f;
            _weaponSO.BurstRate = 0.1f;
        }
    
        private void OnDestroy() {
            base.OnDestroyFunc();
        }

        private void Start() {
            base.InitializeParams();
        }

        public void UpdateWeaponSO(IWeaponParams newParams) {
            WeaponParams.Copy(newParams, _weaponSO);
            _weaponParamsLocal = new WeaponParams(_weaponSO);
        }
    }
}