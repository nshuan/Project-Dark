using NeatBullets.Core.Scripts.WeaponSystem;

namespace NeatBullets.ShadowSurvival.Scripts.EvolutionSystem {
    public class CanvasWeapon : AbstractWeapon
    {
        private void Awake() {
            base.OnAwakeFunc();
        }
        
        private void OnDestroy() {
            base.OnDestroyFunc();
        }

        public void Initialize() {
            base.InitializeParams();
        }

        public void RestartCoroutine() {
            StopAllCoroutines();
            _weaponSO.DestroyProjectilesEvent?.Invoke();
            FireCoroutine = StartCoroutine(Fire());
        }

    }
}
