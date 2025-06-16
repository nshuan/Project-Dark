using NeatBullets.Core.Scripts.SODefinitions;
using NeatBullets.Core.Scripts.WeaponSystem;
using UnityEngine;

namespace NeatBullets.SpaceShooter.Scripts.EnemyBehaviour {
    public class EnemyWeapon : AbstractWeapon
    {
        private void Awake() {
            ProjectilesParentTransform = GameObject.Find("TemporalObjects").transform;
            ProjectileSpawnPoint = transform.Find("ProjectileSpawnPoint");
        }
        private void Start() {
            base.InitializeParams();
        }
        
        public void UpdateWeaponSO(WeaponParamsSO weaponSo) {
            _weaponSO = weaponSo;
            base.InitializeParams();
        }

    }
}
